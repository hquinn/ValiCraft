using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class EnsureRuleTests
{
    // =========================================================================
    // OrderBasicValidator - Extension method rules
    // =========================================================================

    [Fact]
    public void OrderBasicValidator_ValidOrder_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderBasicValidator_NullOrderNumber_FailsWithNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public void OrderBasicValidator_WhitespaceOrderNumber_FailsWithNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "   ";
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("   ");
    }

    [Fact]
    public void OrderBasicValidator_ShortOrderNumber_FailsWithMinLength()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "AB";
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "MinLength")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("AB");
    }

    [Fact]
    public void OrderBasicValidator_LongOrderNumber_FailsWithMaxLength()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = new string('X', 21);
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "MaxLength")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(order.OrderNumber);
    }

    [Fact]
    public void OrderBasicValidator_ZeroOrderTotal_FailsWithGreaterThan()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0m);
    }

    [Fact]
    public void OrderBasicValidator_NegativeOrderTotal_FailsWithGreaterThan()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = -10m;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(-10m);
    }

    [Fact]
    public void OrderBasicValidator_ZeroQuantity_FailsWithGreaterThan()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 0;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "GreaterThan")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0);
    }

    [Fact]
    public void OrderBasicValidator_QuantityAtLimit_FailsWithLessThan()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 1000;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "LessThan")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(1000);
    }

    [Fact]
    public void OrderBasicValidator_QuantityAboveLimit_FailsWithLessThan()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 5000;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "LessThan")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(5000);
    }

    // =========================================================================
    // OrderBasicValidator - Multiple rules on same property
    // =========================================================================

    [Fact]
    public void OrderBasicValidator_MultipleRulesOnQuantity_AllPass()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 500;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderBasicValidator_MultipleRulesOnQuantity_FirstFails()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 0;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "GreaterThan")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0);
    }

    [Fact]
    public void OrderBasicValidator_MultipleRulesOnQuantity_OnlyFirstFails_WhenNegative()
    {
        var order = TestData.ValidOrder();
        order.Quantity = -5;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        // -5 fails IsGreaterThan(0) but passes IsLessThan(1000) since -5 < 1000
        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "GreaterThan")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(-5);
    }

    [Fact]
    public void OrderBasicValidator_MultipleRulesOnOrderNumber_AllThreeFail()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        // null fails all three: NotNullOrWhiteSpace, MinLength (null has no length >= 3), MaxLength
        // In Continue mode, all rules are evaluated
        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public void OrderBasicValidator_MultipleRulesOnOrderNumber_AllPass()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "ORD-123";
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderBasicValidator_MultipleRulesOnOrderNumber_FirstFails()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public void OrderBasicValidator_MultipleRulesOnOrderNumber_MiddleFails()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "AB";
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "MinLength")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("AB");
        errors.ShouldNotContainErrorForPath("OrderTotal");
        errors.ShouldNotContainErrorForPath("Quantity");
    }

    [Fact]
    public void OrderBasicValidator_MultiplePropertiesInvalid_ReturnsErrorsForAll()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        order.OrderTotal = 0m;
        order.Quantity = 0;
        var validator = new OrderBasicValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithAttemptedValue(null);
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithTargetName("Order Total")
            .WithAttemptedValue(0m);
        errors.ShouldContainError("Quantity", "GreaterThan")
            .WithTargetName("Quantity")
            .WithAttemptedValue(0);
    }

    // =========================================================================
    // OrderIsRuleValidator - Is() rules
    // =========================================================================

    [Fact]
    public void OrderIsRuleValidator_ValidOrder_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderIsRuleValidator_ExpressionLambda_ZeroOrderTotal_Fails()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderTotal", "Is")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0m);
    }

    [Fact]
    public void OrderIsRuleValidator_ExpressionLambda_NegativeOrderTotal_Fails()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = -50m;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderTotal", "Is")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(-50m);
    }

    [Fact]
    public void OrderIsRuleValidator_BlockLambda_ZeroQuantity_Fails()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 0;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "Is")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0);
    }

    [Fact]
    public void OrderIsRuleValidator_BlockLambda_NegativeQuantity_Fails()
    {
        var order = TestData.ValidOrder();
        order.Quantity = -1;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "Is")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(-1);
    }

    [Fact]
    public void OrderIsRuleValidator_BlockLambda_QuantityAtUpperBound_Fails()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 1000;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Quantity", "Is")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(1000);
    }

    [Fact]
    public void OrderIsRuleValidator_MethodGroup_NullOrderNumber_Fails()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "IsValidOrderNumber")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public void OrderIsRuleValidator_MethodGroup_WhitespaceOrderNumber_Fails()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "   ";
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "IsValidOrderNumber")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("   ");
    }

    [Fact]
    public void OrderIsRuleValidator_MethodGroup_ShortOrderNumber_Fails()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "AB";
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "IsValidOrderNumber")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("AB");
    }

    [Fact]
    public void OrderIsRuleValidator_MethodGroup_ValidOrderNumber_Passes()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "ORD";
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderIsRuleValidator_MultipleIsRulesFail_ReturnsAllErrors()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;
        order.Quantity = 0;
        order.OrderNumber = null;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(3);
        errors.ShouldContainError("OrderTotal", "Is")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0m);
        errors.ShouldContainError("Quantity", "Is")
            .WithTargetName("Quantity")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0);
        errors.ShouldContainError("OrderNumber", "IsValidOrderNumber")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public void OrderIsRuleValidator_BlockLambda_BoundaryValid_Passes()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 999;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderIsRuleValidator_BlockLambda_LowerBoundaryValid_Passes()
    {
        var order = TestData.ValidOrder();
        order.Quantity = 1;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderIsRuleValidator_ExpressionLambda_SmallPositiveValue_Passes()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0.01m;
        var validator = new OrderIsRuleValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }
}
