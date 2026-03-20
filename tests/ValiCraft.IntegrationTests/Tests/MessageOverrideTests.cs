using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class MessageOverrideTests
{
    private readonly OrderMessageOverrideValidator _validator = new();

    [Fact]
    public void ValidOrder_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();

        var result = _validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void NullOrderNumber_ReturnsCustomMessageCodeTargetNameAndSeverity()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;

        var result = _validator.Validate(order);

        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderNumber", "MISSING_ORDER_NUMBER")
            .WithTargetName("PO Number")
            .WithMessage("Order number is mandatory")
            .WithSeverity(ErrorSeverity.Critical);
    }

    [Fact]
    public void ZeroOrderTotal_ReturnsCustomMessageAndSeverityWithDefaultCode()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;

        var result = _validator.Validate(order);

        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithMessage("Total must be positive")
            .WithSeverity(ErrorSeverity.Warning);
    }

    [Fact]
    public void BothInvalid_ReturnsBothCustomErrors()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        order.OrderTotal = 0m;

        var result = _validator.Validate(order);

        var errors = result.ShouldFailWith(2);

        errors.ShouldContainError("OrderNumber", "MISSING_ORDER_NUMBER")
            .WithTargetName("PO Number")
            .WithMessage("Order number is mandatory")
            .WithSeverity(ErrorSeverity.Critical);

        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithMessage("Total must be positive")
            .WithSeverity(ErrorSeverity.Warning);
    }
}
