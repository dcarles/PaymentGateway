using AutoMapper;
using Moq;
using PaymentGateway.BankProcessor;
using PaymentGateway.BankProcessor.Models;
using PaymentGateway.Data;
using PaymentGateway.Data.Entities;
using PaymentGateway.PaymentsCore.Handlers;
using PaymentGateway.PaymentsCore.Tests.Helpers;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PaymentGateway.PaymentsCore.Tests
{
	public class CardPaymentHandlerTests
	{

		[Fact]
		public async Task HandleAsync_NullCardPaymentPassed_ReturnsArgumentNullException()
		{
			// Arrange
			var gatewayDbContextMock = GatewayDbContextHelper.GetGatewayContext();
			var transactionRepositoryMock = new Mock<Repository<Transaction>>(gatewayDbContextMock);
			var paymentProcessorMock = new Mock<IPaymentProcessor>();
			var mapperMock = new Mock<IMapper>();
			var processPaymentCommandHandlerUnderTest =
				new CardPaymentHandler(transactionRepositoryMock.Object, paymentProcessorMock.Object, mapperMock.Object);

			// Act 
			var exceptionThrown = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
				await processPaymentCommandHandlerUnderTest.Handle(null, CancellationToken.None));


			// Assert
			Assert.IsType<ArgumentNullException>(exceptionThrown);
		}

		[Fact]
		public async Task HandleAsync_ValidCardPaymentPassed_ReturnsSuccess()
		{
			// Arrange
			var gatewayDbContextMock = GatewayDbContextHelper.GetGatewayContext();
			var transactionRepositoryMock = new Mock<Repository<Data.Entities.Transaction>>(gatewayDbContextMock);

			transactionRepositoryMock.Setup(tr => tr.AddAsync(It.IsAny<Transaction>()))
				.Returns(() => Task.CompletedTask);
			transactionRepositoryMock.Setup(tr => tr.UpdateAsync(It.IsAny<Transaction>()))
				.Returns(() => Task.CompletedTask);

			var paymentProcessorMock = new Mock<IPaymentProcessor>();

			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(mapper => mapper.Map<Transaction>(It.IsAny<CardPayment>())).Returns(new Transaction());
			mapperMock.Setup(mapper => mapper.Map<PaymentProcessingRequest>(It.IsAny<CardPayment>())).Returns(new PaymentProcessingRequest());

			var paymentProcessingResponseFake = new PaymentProcessingResponse("test",
				true,
				"1000", "message");
			var paymentProcessingCommandFake = new CardPayment() { MerchantTransactionId = Guid.NewGuid().ToString(), ExpiryMonth = 3, ExpiryYear = 2022, Cvv = "123", Amount = 10.00m, Currency = "EUR" };


			paymentProcessorMock.Setup(processor => processor.ProcessAsync(It.IsAny<PaymentProcessingRequest>()))
				.ReturnsAsync(paymentProcessingResponseFake);


			var processPaymentCommandHandlerUnderTest =
				new CardPaymentHandler(transactionRepositoryMock.Object, paymentProcessorMock.Object, mapperMock.Object);


			// Act 
			var paymentResult = await processPaymentCommandHandlerUnderTest.Handle(paymentProcessingCommandFake, CancellationToken.None);

			// Assert

			//verify that transaction added to db before it is being processed
			transactionRepositoryMock.Verify(tr => tr.AddAsync(It.IsAny<Data.Entities.Transaction>()), Times.Once);
			transactionRepositoryMock.Verify(tr => tr.UpdateAsync(It.IsAny<Data.Entities.Transaction>()), Times.Once);
			Assert.IsType<PaymentResult.Success>(paymentResult);
			Assert.Equal(paymentProcessingResponseFake.ResponseCode, paymentResult.ResponseCode);
		}


		[Fact]
		public async Task HandleAsync_DuplicatedCardPaymentPassed_ApprovedTransaction_ReturnsExistingTransactionDataInPaymentResultSuccess()
		{
			// Arrange
			var gatewayDbContextMock = GatewayDbContextHelper.GetGatewayContext();
			var transactionRepositoryMock = new Mock<Repository<Transaction>>(gatewayDbContextMock);
			var merchantTransactionId = Guid.NewGuid().ToString();

			var existingTransaction = new Transaction
			{
				MerchantTransactionId = merchantTransactionId,
				TransactionId = Guid.NewGuid(),
				Status = TransactionStatus.Approved
			};

			transactionRepositoryMock.Setup(tr => tr.AddAsync(It.IsAny<Transaction>()))
				.Returns(() => Task.CompletedTask);
			transactionRepositoryMock.Setup(tr => tr.GetSingleByQueryAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
		 .ReturnsAsync(existingTransaction);
			transactionRepositoryMock.Setup(tr => tr.UpdateAsync(It.IsAny<Transaction>()))
				.Returns(() => Task.CompletedTask);

			var paymentProcessorMock = new Mock<IPaymentProcessor>();

			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(mapper => mapper.Map<Transaction>(It.IsAny<CardPayment>())).Returns(new Transaction());
			mapperMock.Setup(mapper => mapper.Map<PaymentProcessingRequest>(It.IsAny<CardPayment>())).Returns(new PaymentProcessingRequest());

		
			var paymentProcessingCommandFake = new CardPayment() { MerchantTransactionId = merchantTransactionId, ExpiryMonth = 3, ExpiryYear = 2022, Cvv = "123", Amount = 10.00m, Currency = "EUR" };


			var processPaymentCommandHandlerUnderTest =
				new CardPaymentHandler(transactionRepositoryMock.Object, paymentProcessorMock.Object, mapperMock.Object);


			// Act 
			var paymentResult = await processPaymentCommandHandlerUnderTest.Handle(paymentProcessingCommandFake, CancellationToken.None);

			// Assert

			//verify that transaction added to db before it is being processed
			transactionRepositoryMock.Verify(tr => tr.AddAsync(It.IsAny<Transaction>()), Times.Never);
			transactionRepositoryMock.Verify(tr => tr.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
			paymentProcessorMock.Verify(tr => tr.ProcessAsync(It.IsAny<PaymentProcessingRequest>()), Times.Never);
			Assert.IsType<PaymentResult.Success>(paymentResult);
			Assert.Equal(existingTransaction.TransactionId.ToString(), paymentResult.GatewayReferenceId);
			Assert.Equal("1000", paymentResult.ResponseCode);
		}


		[Fact]
		public async Task HandleAsync_DuplicatedCardPaymentPassed_DeclinedTransaction_ReturnsExistingTransactionDataInPaymentResultDeclined()
		{
			// Arrange
			var gatewayDbContextMock = GatewayDbContextHelper.GetGatewayContext();
			var transactionRepositoryMock = new Mock<Repository<Transaction>>(gatewayDbContextMock);
			var merchantTransactionId = Guid.NewGuid().ToString();

			var existingTransaction = new Transaction
			{
				MerchantTransactionId = merchantTransactionId,
				TransactionId = Guid.NewGuid(),
				Status = TransactionStatus.Declined,
				ErrorMessage = "Some error message"
			};

			transactionRepositoryMock.Setup(tr => tr.AddAsync(It.IsAny<Transaction>()))
				.Returns(() => Task.CompletedTask);
			transactionRepositoryMock.Setup(tr => tr.GetSingleByQueryAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
		 .ReturnsAsync(existingTransaction);
			transactionRepositoryMock.Setup(tr => tr.UpdateAsync(It.IsAny<Transaction>()))
				.Returns(() => Task.CompletedTask);

			var paymentProcessorMock = new Mock<IPaymentProcessor>();

			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(mapper => mapper.Map<Transaction>(It.IsAny<CardPayment>())).Returns(new Transaction());
			mapperMock.Setup(mapper => mapper.Map<PaymentProcessingRequest>(It.IsAny<CardPayment>())).Returns(new PaymentProcessingRequest());


			var paymentProcessingCommandFake = new CardPayment() { MerchantTransactionId = merchantTransactionId, ExpiryMonth = 3, ExpiryYear = 2022, Cvv = "123", Amount = 10.00m, Currency = "EUR" };


			var processPaymentCommandHandlerUnderTest =
				new CardPaymentHandler(transactionRepositoryMock.Object, paymentProcessorMock.Object, mapperMock.Object);


			// Act 
			var paymentResult = await processPaymentCommandHandlerUnderTest.Handle(paymentProcessingCommandFake, CancellationToken.None);

			// Assert

			//verify that transaction added to db before it is being processed
			transactionRepositoryMock.Verify(tr => tr.AddAsync(It.IsAny<Transaction>()), Times.Never);
			transactionRepositoryMock.Verify(tr => tr.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
			paymentProcessorMock.Verify(tr => tr.ProcessAsync(It.IsAny<PaymentProcessingRequest>()), Times.Never);
			Assert.IsType<PaymentResult.Declined>(paymentResult);

			var declinedResult = paymentResult as PaymentResult.Declined;
			Assert.Equal(existingTransaction.TransactionId.ToString(), paymentResult.GatewayReferenceId);
			Assert.Equal(existingTransaction.ErrorMessage, declinedResult?.ResponseMessage);
			Assert.Equal("2000", paymentResult.ResponseCode);
		}

	}
}