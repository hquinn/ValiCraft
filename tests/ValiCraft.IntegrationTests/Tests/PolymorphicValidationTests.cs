using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class PolymorphicValidationTests
{
    // =========================================================================
    // OrderPolymorphicValidator - WhenType with ValidateWith and Allow
    // =========================================================================

    [Fact]
    public void OrderPolymorphicValidator_ValidCreditCardPayment_Passes()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CreditCardPayment { CardNumber = "4111111111111111", Amount = 50m };
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderPolymorphicValidator_CreditCardWithNullCardNumber_FailsWithCardNumberError()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CreditCardPayment { CardNumber = null, Amount = 50m };
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Payment.CardNumber", "NotNullOrWhiteSpace");
    }

    [Fact]
    public void OrderPolymorphicValidator_CreditCardWithZeroAmount_FailsWithAmountError()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CreditCardPayment { CardNumber = "4111111111111111", Amount = 0m };
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Payment.Amount", "GreaterThan");
    }

    [Fact]
    public void OrderPolymorphicValidator_CreditCardWithBothInvalid_FailsWith2Errors()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CreditCardPayment { CardNumber = null, Amount = 0m };
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("Payment.CardNumber", "NotNullOrWhiteSpace");
        errors.ShouldContainError("Payment.Amount", "GreaterThan");
    }

    [Fact]
    public void OrderPolymorphicValidator_CryptoPaymentAllow_PassesRegardlessOfState()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CryptoPayment { WalletAddress = null, Amount = 0m };
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderPolymorphicValidator_NullPaymentSkipBehavior_NoPolymorphicErrors()
    {
        var order = TestData.ValidOrder();
        order.Payment = null;
        order.OrderNumber = null;
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
        errors.ShouldNotContainErrorForPath("Payment");
        errors.ShouldNotContainErrorForPath("Payment.CardNumber");
        errors.ShouldNotContainErrorForPath("Payment.Amount");
    }

    [Fact]
    public void OrderPolymorphicValidator_NullPaymentWithValidOrderNumber_PassesCompletely()
    {
        var order = TestData.ValidOrder();
        order.Payment = null;
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderPolymorphicValidator_UnmatchedBankTransferPayment_PassesWithNoValidation()
    {
        var order = TestData.ValidOrder();
        order.Payment = new BankTransferPayment { AccountNumber = null, Amount = 0m };
        var validator = new OrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    // =========================================================================
    // OrderPolymorphicFailOnNullValidator - PolymorphicNullBehavior.Fail
    // =========================================================================

    [Fact]
    public void OrderPolymorphicFailOnNullValidator_NullPayment_FailsWithPaymentIsNullError()
    {
        var order = TestData.ValidOrder();
        order.Payment = null;
        var validator = new OrderPolymorphicFailOnNullValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("Payment", "PaymentIsNull");
    }

    [Fact]
    public void OrderPolymorphicFailOnNullValidator_CreditCardPaymentAllow_Passes()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CreditCardPayment { CardNumber = "4111111111111111", Amount = 50m };
        var validator = new OrderPolymorphicFailOnNullValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderPolymorphicFailOnNullValidator_UnmatchedCryptoPayment_Passes()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CryptoPayment { WalletAddress = null, Amount = 0m };
        var validator = new OrderPolymorphicFailOnNullValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }
}
