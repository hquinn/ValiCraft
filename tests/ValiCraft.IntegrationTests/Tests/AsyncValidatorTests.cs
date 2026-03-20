using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class AsyncValidatorTests
{
    // =========================================================================
    // AsyncOrderBasicValidator - Basic async validation
    // =========================================================================

    [Fact]
    public async Task AsyncBasic_ValidOrder_Passes()
    {
        var order = TestData.ValidOrder();
        var validator = new AsyncOrderBasicValidator();

        var result = await validator.ValidateAsync(order);

        result.ShouldPass();
    }

    [Fact]
    public async Task AsyncBasic_NullOrderNumber_FailsWithNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new AsyncOrderBasicValidator();

        var result = await validator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public async Task AsyncBasic_ShortOrderNumber_FailsWithMinLength()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "AB";
        var validator = new AsyncOrderBasicValidator();

        var result = await validator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "MinLength")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("AB");
    }

    [Fact]
    public async Task AsyncBasic_ZeroOrderTotal_FailsWithGreaterThan()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;
        var validator = new AsyncOrderBasicValidator();

        var result = await validator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0m);
    }

    // =========================================================================
    // AsyncOrderConditionalValidator - Conditional async validation
    // =========================================================================

    [Fact]
    public async Task AsyncConditional_ExpressWithNullNotes_FailsWithNotesError()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.Notes = null;
        var validator = new AsyncOrderConditionalValidator();

        var result = await validator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Notes");
    }

    [Fact]
    public async Task AsyncConditional_NotExpressWithNullNotes_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = false;
        order.Notes = null;
        var validator = new AsyncOrderConditionalValidator();

        var result = await validator.ValidateAsync(order);

        result.ShouldPass();
    }

    // =========================================================================
    // AsyncOrderCollectionValidator - Collection async validation (sync child)
    // =========================================================================

    [Fact]
    public async Task AsyncCollection_ValidLineItems_Passes()
    {
        var order = TestData.ValidOrder();
        var validator = new AsyncOrderCollectionValidator();

        var result = await validator.ValidateAsync(order);

        result.ShouldPass();
    }

    [Fact]
    public async Task AsyncCollection_InvalidLineItems_ProducesIndexedErrors()
    {
        var order = TestData.ValidOrder();
        order.LineItems = [new LineItem { Code = null, Price = 0m, Quantity = 1 }];
        var validator = new AsyncOrderCollectionValidator();

        var result = await validator.ValidateAsync(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[0].Price", "GreaterThan");
    }

    [Fact]
    public async Task AsyncCollection_SecondItemInvalid_FailsAtCorrectIndex()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            TestData.ValidLineItem(),
            new LineItem { Code = null, Price = 0m, Quantity = 1 }
        ];
        var validator = new AsyncOrderCollectionValidator();

        var result = await validator.ValidateAsync(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[1].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[1].Price", "GreaterThan");
        errors.ShouldNotContainErrorForPath("LineItems[0].Code");
        errors.ShouldNotContainErrorForPath("LineItems[0].Price");
    }

    // =========================================================================
    // AsyncOrderPolymorphicValidator - Polymorphic async validation (sync child)
    // =========================================================================

    [Fact]
    public async Task AsyncPolymorphic_CreditCardWithNullCardNumber_FailsAtPaymentCardNumber()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CreditCardPayment { CardNumber = null, Amount = 99.99m };
        var validator = new AsyncOrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = await validator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Payment.CardNumber", "NotNullOrWhiteSpace");
    }

    [Fact]
    public async Task AsyncPolymorphic_CryptoPayment_AllowedAndPasses()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CryptoPayment { WalletAddress = null, Amount = 50m };
        var validator = new AsyncOrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = await validator.ValidateAsync(order);

        result.ShouldPass();
    }

    [Fact]
    public async Task AsyncPolymorphic_ValidCreditCard_Passes()
    {
        var order = TestData.ValidOrder();
        order.Payment = new CreditCardPayment { CardNumber = "4111111111111111", Amount = 99.99m };
        var validator = new AsyncOrderPolymorphicValidator(new CreditCardPaymentValidator());

        var result = await validator.ValidateAsync(order);

        result.ShouldPass();
    }
}
