using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class StaticValidatorTests
{
    // =========================================================================
    // StaticOrderBasicValidator - Basic static validation
    // =========================================================================

    [Fact]
    public void StaticBasic_ValidOrder_Passes()
    {
        var order = TestData.ValidOrder();

        var result = StaticOrderBasicValidator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void StaticBasic_NullOrderNumber_FailsWithNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;

        var result = StaticOrderBasicValidator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(null);
    }

    [Fact]
    public void StaticBasic_ShortOrderNumber_FailsWithMinLength()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = "AB";

        var result = StaticOrderBasicValidator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "MinLength")
            .WithTargetName("Order Number")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue("AB");
    }

    [Fact]
    public void StaticBasic_ZeroOrderTotal_FailsWithGreaterThan()
    {
        var order = TestData.ValidOrder();
        order.OrderTotal = 0m;

        var result = StaticOrderBasicValidator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithTargetName("Order Total")
            .WithSeverity(ErrorSeverity.Error)
            .WithAttemptedValue(0m);
    }

    [Fact]
    public void StaticBasic_MultipleInvalid_ReturnsErrorsForAll()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        order.OrderTotal = 0m;

        var result = StaticOrderBasicValidator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace")
            .WithAttemptedValue(null);
        errors.ShouldContainError("OrderTotal", "GreaterThan")
            .WithAttemptedValue(0m);
    }

    // =========================================================================
    // StaticCoordinateValidator - Struct target static validation
    // =========================================================================

    [Fact]
    public void StaticStruct_ValidCoordinate_Passes()
    {
        var coord = TestData.ValidCoordinate();

        var result = StaticCoordinateValidator.Validate(coord);

        result.ShouldPass();
    }

    [Fact]
    public void StaticStruct_InvalidLatitude_FailsWithGreaterThan()
    {
        var coord = new Coordinate { Latitude = -91.0, Longitude = -93.0 };

        var result = StaticCoordinateValidator.Validate(coord);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Latitude", "GreaterThan");
    }

    [Fact]
    public void StaticStruct_InvalidLongitude_FailsWithLessThan()
    {
        var coord = new Coordinate { Latitude = 45.0, Longitude = 181.0 };

        var result = StaticCoordinateValidator.Validate(coord);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Longitude", "LessThan");
    }

    [Fact]
    public void StaticStruct_BothInvalid_ReturnsMultipleErrors()
    {
        var coord = new Coordinate { Latitude = -91.0, Longitude = 181.0 };

        var result = StaticCoordinateValidator.Validate(coord);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Latitude", "GreaterThan");
        errors.ShouldContainError("Longitude", "LessThan");
    }

    // =========================================================================
    // StaticOrderConditionalValidator - Conditional static validation
    // =========================================================================

    [Fact]
    public void StaticConditional_ExpressWithNullNotes_FailsWithNotesError()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.Notes = null;

        var result = StaticOrderConditionalValidator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Notes");
    }

    [Fact]
    public void StaticConditional_NotExpressWithNullNotes_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = false;
        order.Notes = null;

        var result = StaticOrderConditionalValidator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void StaticConditional_ExpressWithValidNotes_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.Notes = "Rush delivery";

        var result = StaticOrderConditionalValidator.Validate(order);

        result.ShouldPass();
    }

    // =========================================================================
    // StaticOrderCollectionValidator - Collection static validation
    // =========================================================================

    [Fact]
    public void StaticCollection_ValidLineItems_Passes()
    {
        var order = TestData.ValidOrder();

        var result = StaticOrderCollectionValidator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void StaticCollection_InvalidLineItems_ProducesIndexedErrors()
    {
        var order = TestData.ValidOrder();
        order.LineItems = [new LineItem { Code = null, Price = 0m, Quantity = 1 }];

        var result = StaticOrderCollectionValidator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[0].Price", "GreaterThan");
    }

    [Fact]
    public void StaticCollection_SecondItemInvalid_FailsAtCorrectIndex()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            TestData.ValidLineItem(),
            new LineItem { Code = null, Price = 0m, Quantity = 1 }
        ];

        var result = StaticOrderCollectionValidator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[1].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[1].Price", "GreaterThan");
        errors.ShouldNotContainErrorForPath("LineItems[0].Code");
        errors.ShouldNotContainErrorForPath("LineItems[0].Price");
    }
}
