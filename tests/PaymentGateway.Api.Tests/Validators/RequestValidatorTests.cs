using AutoFixture;
using FluentValidation.TestHelper;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Validators;
using System;
using Xunit;

namespace PaymentGateway.Api.Tests.Validators
{
	public class PaymentRequestValidatorTests : IDisposable
	{
		private PaymentRequestValidator _validator;
		private readonly IFixture _fixture;
		private PaymentRequest _paymentRequest;

		/// <summary>
		/// Setup validator to be tested
		/// </summary>
		public PaymentRequestValidatorTests()
		{
			_validator = new PaymentRequestValidator();
			_fixture = new Fixture();
			_paymentRequest = _fixture.Create<PaymentRequest>();
		}

		#region Card Number 
		[Fact]
		public void CardNumber_IsNull_ShouldHaveError()
		{
			_paymentRequest.CardNumber = null;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.CardNumber);
		}

		[Fact]
		public void CardNumber_IsEmpty_ShouldHaveError()
		{
			_paymentRequest.CardNumber = string.Empty;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.CardNumber);
		}

		//[Fact]
		//public void CardNumber_IsNotValid_ShouldHaveError()
		//{
		//	_paymentRequest.CardNumber = "1234567890123452"; // simple
		//	var result = _validator.TestValidate(_paymentRequest);
		//	result.ShouldNotHaveValidationErrorFor(request => request.CardNumber);

		//	_paymentRequest.CardNumber = "1234 5678 9012 3452";// With spaces
		//	result = _validator.TestValidate(_paymentRequest);
		//	result.ShouldNotHaveValidationErrorFor(request => request.CardNumber);

		//	_paymentRequest.CardNumber = "1234-5678-9012-3452";// With Dashes
		//	result = _validator.TestValidate(_paymentRequest);
		//	result.ShouldNotHaveValidationErrorFor(request => request.CardNumber);

		//	_paymentRequest.CardNumber = "123456789012345";// 15 digits
		//	result = _validator.TestValidate(_paymentRequest);
		//	result.ShouldHaveValidationErrorFor(request => request.CardNumber);		
		//}

		#endregion

		#region Cvv 
		[Fact]
		public void Cvv_IsNull_ShouldHaveError()
		{

			_paymentRequest.Cvv = null;
			 var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Cvv);
		}
		[Fact]
		public void Cvv_IsEmpty_ShouldHaveError()
		{
			_paymentRequest.Cvv = string.Empty;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Cvv);
		}

		[Fact]
		public void Cvv_Is3DigitsForAmex_ShouldHaveError()
		{
			_paymentRequest.CardNumber = "3734567890123452";
			_paymentRequest.Cvv = "123";
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Cvv);
		}

		[Fact]
		public void Cvv_Is4DigitsForNonAmex_ShouldHaveError()
		{
			_paymentRequest.CardNumber = "4534567890123452";
			_paymentRequest.Cvv = "1235";
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Cvv);
		}

		[Fact]
		public void Cvv_Is4DigitsForAmex_ShouldNotHaveError()
		{
			_paymentRequest.CardNumber = "3456789012345223";
			_paymentRequest.Cvv = "1235";		
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.Cvv);
		}

		[Fact]
		public void Cvv_Is3DigitsForNonAmex_ShouldNotHaveError()
		{
			_paymentRequest.CardNumber = "5456789012345223";
			_paymentRequest.Cvv = "123";
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.Cvv);
		}
		#endregion

		#region Expiry Tests
		[Fact]
		public void ExpiryMonth_Is0_ShouldHaveError()
		{
			_paymentRequest.ExpiryMonth = 0;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.ExpiryMonth);
		}

		[Fact]
		public void ExpiryMonth_IsNotValid_ShouldHaveError()
		{
			_paymentRequest.ExpiryMonth = 1;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.ExpiryMonth);

			_paymentRequest.ExpiryMonth = 5;
			result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.ExpiryMonth);

			_paymentRequest.ExpiryMonth = 12;
			result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.ExpiryMonth);

			_paymentRequest.ExpiryMonth = 13;
			result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.ExpiryMonth);

			_paymentRequest.ExpiryMonth = -1;
			result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.ExpiryMonth);
		}

		[Fact]
		public void ExpiryYear_Is0_ShouldHaveError()
		{
			_paymentRequest.ExpiryYear = 0;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.ExpiryYear);
		}

		[Fact]
		public void ExpiryYear_IsInPast_ShouldHaveError()
		{
			_paymentRequest.ExpiryYear = 2018;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.ExpiryYear);

			_paymentRequest.ExpiryYear = DateTime.Today.Year;
			result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.ExpiryYear);	

		}

		[Fact]
		public void ExpiryMonth_IsInPast_ShouldHaveError()
		{
			_paymentRequest.ExpiryYear = DateTime.Today.Year;
			_paymentRequest.ExpiryMonth = 1;
			var result = _validator.TestValidate(_paymentRequest);			
			result.ShouldHaveValidationErrorFor(request => request.ExpiryMonth);
		}
		#endregion

		#region Amount

		[Fact]
		public void Amount_IsNegative_ShouldHaveError()
		{
			_paymentRequest.Amount = -1;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Amount);
		}


		#endregion

		#region Currency

		[Fact]
		public void Currency_IsNull_ShouldHaveError()
		{
			_paymentRequest.Currency = null;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Currency);
		}
		[Fact]
		public void Currency_IsEmpty_ShouldHaveError()
		{
			_paymentRequest.Currency = string.Empty;
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Currency);
		}

		[Fact]
		public void Currency_LongerThan3Character_ShouldHaveError()
		{
			_paymentRequest.Currency = "EURR";
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Currency);

			_paymentRequest.Currency = "EUR";
			result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.Currency);	
		}

		[Fact]
		public void Currency_HasNumericCharacter_ShouldHaveError()
		{
			_paymentRequest.Currency = "3UR";
			var result = _validator.TestValidate(_paymentRequest);
			result.ShouldHaveValidationErrorFor(request => request.Currency);

			_paymentRequest.Currency = "EUR";
			result = _validator.TestValidate(_paymentRequest);
			result.ShouldNotHaveValidationErrorFor(request => request.Currency);
		}


		#endregion

		public void Dispose()
		{
			_validator = null;
		}
	}
}
