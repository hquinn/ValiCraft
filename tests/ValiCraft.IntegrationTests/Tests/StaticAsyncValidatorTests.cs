using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class StaticAsyncValidatorTests
{
    // =========================================================================
    // StaticAsyncOrderBasicValidator - Basic static async validation
    // =========================================================================

    [Fact]
    public async Task StaticAsyncBasic_ValidOrder_Passes()
    {
        var order = TestData.ValidOrder();

        var result = await StaticAsyncOrderBasicValidator.ValidateAsync(order);

        result.ShouldPass();
    }

    [Fact]
    public async Task StaticAsyncBasic_NullOrderNumber_FailsWithNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;

        var result = await StaticAsyncOrderBasicValidator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public async Task StaticAsyncBasic_ShortOrderNumber_FailsWithMinLength()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "AB";

        var result = await StaticAsyncOrderBasicValidator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "MinLength")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("AB");
    }

    [Fact]
    public async Task StaticAsyncBasic_ZeroOrderTotal_FailsWithGreaterThan()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;

        var result = await StaticAsyncOrderBasicValidator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0m);
    }

    // =========================================================================
    // StaticAsyncOrderConditionalValidator - Conditional static async validation
    // =========================================================================

    [Fact]
    public async Task StaticAsyncConditional_ExpressWithNullNotes_FailsWithNotesError()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.Notes = null;

        var result = await StaticAsyncOrderConditionalValidator.ValidateAsync(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Notes");
    }

    [Fact]
    public async Task StaticAsyncConditional_NotExpressWithNullNotes_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = false;
        order.Notes = null;

        var result = await StaticAsyncOrderConditionalValidator.ValidateAsync(order);

        result.ShouldPass();
    }

    [Fact]
    public async Task StaticAsyncConditional_ExpressWithValidNotes_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.Notes = "Rush delivery";

        var result = await StaticAsyncOrderConditionalValidator.ValidateAsync(order);

        result.ShouldPass();
    }
}
