using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class OnFailureModeTests
{
    // =========================================================================
    // OrderHaltValidator - Halt mode on individual chain
    // =========================================================================

    [Fact]
    public void OrderHaltValidator_ValidOrder_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderHaltValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderHaltValidator_NullOrderNumber_HaltsAfterNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderHaltValidator();

        var result = validator.Validate(order);

        // Halt mode: only NotNullOrWhiteSpace fires; MinLength and MaxLength are NOT evaluated
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
    }

    [Fact]
    public void OrderHaltValidator_ShortOrderNumber_HaltsAfterMinLength()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "AB";
        var validator = new OrderHaltValidator();

        var result = validator.Validate(order);

        // "AB" passes NotNullOrWhiteSpace but fails MinLength(3); Halt stops before MaxLength
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderNumber", "MinLength");
    }

    [Fact]
    public void OrderHaltValidator_InvalidQuantity_ContinueEvaluatesAllRules()
    {
        var order = TestData.ValidOrder();
        order.Quantity = -1;
        var validator = new OrderHaltValidator();

        var result = validator.Validate(order);

        // Continue mode (default): quantity = -1 fails GreaterThan(0) but passes LessThan(1000)
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("Quantity", "GreaterThan");
    }

    [Fact]
    public void OrderHaltValidator_QuantityExceedsMax_FailsLessThan()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 1001;
        var validator = new OrderHaltValidator();

        var result = validator.Validate(order);

        // Continue mode: quantity = 1001 passes GreaterThan(0) but fails LessThan(1000)
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("Quantity", "LessThan");
    }

    [Fact]
    public void OrderHaltValidator_NullOrderNumberAndInvalidQuantity_BothChainsEvaluate()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        order.Quantity = -1;
        var validator = new OrderHaltValidator();

        var result = validator.Validate(order);

        // Halt stops OrderNumber chain after first failure, but Quantity is a separate chain
        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
        errors.ShouldContainError("Quantity", "GreaterThan");
    }

    // =========================================================================
    // OrderWithOnFailureValidator - Group-level Halt via WithOnFailure
    // =========================================================================

    [Fact]
    public void OrderWithOnFailureValidator_ValidOrder_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderWithOnFailureValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderWithOnFailureValidator_NullOrderNumber_HaltsEntireGroup()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderWithOnFailureValidator();

        var result = validator.Validate(order);

        // Group-level Halt: OrderNumber fails, so OrderTotal is NOT evaluated
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
        errors.ShouldNotContainErrorForPath("OrderTotal");
    }

    [Fact]
    public void OrderWithOnFailureValidator_ValidOrderNumberButZeroTotal_FailsOrderTotal()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;
        var validator = new OrderWithOnFailureValidator();

        var result = validator.Validate(order);

        // OrderNumber passes, so OrderTotal is evaluated and fails
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderTotal", "GreaterThan");
    }

    [Fact]
    public void OrderWithOnFailureValidator_BothInvalid_HaltsAfterFirstError()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        order.OrderTotal = 0m;
        var validator = new OrderWithOnFailureValidator();

        var result = validator.Validate(order);

        // Group-level Halt: first chain (OrderNumber) fails, entire group stops
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
        errors.ShouldNotContainErrorForPath("OrderTotal");
    }
}
