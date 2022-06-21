using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PaymentGateway.BankProcessor;
using PaymentGateway.BankProcessor.Models;
using PaymentGateway.Data;
using PaymentGateway.Data.Entities;

namespace PaymentGateway.PaymentsCore.Handlers
{
	public class CardPaymentHandler : IRequestHandler<CardPayment, PaymentResult>
	{
		private readonly IRepository<Transaction> _transactionRepository;
		private readonly IPaymentProcessor _paymentProcessor;
		private readonly IMapper _mapper;
		private const string FailedCode = "3000";
		private const string PendingCode = "4000";
		private const string SuccessCode = "1000";
		private const string DeclinedCode = "2000";

		public CardPaymentHandler(IRepository<Transaction> transactionRepository,
			IPaymentProcessor paymentProcessor, IMapper mapper)
		{
			_transactionRepository = transactionRepository;
			_paymentProcessor = paymentProcessor;
			_mapper = mapper;
		}

		public async Task<PaymentResult> Handle(CardPayment command, CancellationToken cancellationToken)
		{
			if (command == null) throw new ArgumentNullException(nameof(command));
			// save transaction
			var transaction = _mapper.Map<Transaction>(command);

			//Naive implementation to avoid duplicates and guarantee Idempotency when Creating payment transactions
			var existingTransaction = await _transactionRepository.GetSingleByQueryAsync(tr => tr.MerchantTransactionId == command.MerchantTransactionId
																				   && tr.MerchantId == command.MerchantId);
			if (existingTransaction != null)
			{
				if (existingTransaction.Status == TransactionStatus.Approved)
				{
					return new PaymentResult.Success(existingTransaction.TransactionId.ToString(), SuccessCode);
				}
				else if (existingTransaction.Status == TransactionStatus.Declined)
				{
					return new PaymentResult.Declined(existingTransaction.TransactionId.ToString(), DeclinedCode)
					{
						ResponseMessage = existingTransaction.ErrorMessage ?? ""
					};
				}
				else if (existingTransaction.Status == TransactionStatus.Pending)
				{
					//In case transaction is pending still for some reason we will let them know so they can check the result later if they dont get response of previous request
					return new PaymentResult.Declined(existingTransaction.TransactionId.ToString(), PendingCode)
					{
						ResponseMessage = $"A payment request with same MerchantTransactionId '{command.MerchantTransactionId}' is being processed at the moment, please query this" +
						$"transaction later for the result if you dont receive a response on the pending transaction"
					};
				}

			}

			await _transactionRepository.AddAsync(transaction);
			// request transaction via client
			// construct transaction processing request
			var paymentProcessingRequest = _mapper.Map<PaymentProcessingRequest>(command);
			var paymentProcessingResponse = await _paymentProcessor.ProcessAsync(paymentProcessingRequest);

			// Update transaction;
			await UpdateTransaction(transaction, paymentProcessingResponse);
			if (paymentProcessingResponse.Approved)
				return new PaymentResult.Success(command.TransactionId.ToString(), paymentProcessingResponse.ResponseCode);
			return new PaymentResult.Declined(command.TransactionId.ToString(), paymentProcessingResponse.ResponseCode)
			{
				ResponseMessage = paymentProcessingResponse.ResponseMessage
			};
		}

		private async Task UpdateTransaction(Transaction transaction,
			PaymentProcessingResponse paymentProcessingResponse)
		{
			transaction.Status = paymentProcessingResponse.Approved
				? TransactionStatus.Approved
				: TransactionStatus.Declined;
			transaction.BankReferenceId = paymentProcessingResponse.BankReferenceId;
			transaction.ErrorMessage = paymentProcessingResponse.ResponseMessage;
			await _transactionRepository.UpdateAsync(transaction);
		}
	}
}