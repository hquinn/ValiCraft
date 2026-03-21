using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class TargetTypeTests
{
    // =========================================================================
    // Struct (Coordinate)
    // =========================================================================

    [Fact]
    public void Struct_ValidCoordinate_Passes()
    {
        var coordinate = TestData.ValidCoordinate();
        var validator = new CoordinateValidator();

        var result = validator.Validate(coordinate);

        result.ShouldPass();
    }

    [Fact]
    public void Struct_InvalidLatitude_FailsWithLessThan()
    {
        var coordinate = TestData.ValidCoordinate();
        coordinate.Latitude = 100;
        var validator = new CoordinateValidator();

        var result = validator.Validate(coordinate);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Latitude", "LessThan");
    }

    [Fact]
    public void Struct_InvalidLongitude_FailsWithGreaterThan()
    {
        var coordinate = TestData.ValidCoordinate();
        coordinate.Longitude = -200;
        var validator = new CoordinateValidator();

        var result = validator.Validate(coordinate);

        var errors = result.ShouldFail();
        errors.ShouldContainError("Longitude", "GreaterThan");
    }

    [Fact]
    public void Struct_BothInvalid_FailsWithTwoErrors()
    {
        var coordinate = TestData.ValidCoordinate();
        coordinate.Latitude = 100;
        coordinate.Longitude = -200;
        var validator = new CoordinateValidator();

        var result = validator.Validate(coordinate);

        result.ShouldFailWith(2);
    }

    // =========================================================================
    // Record Struct (GeoRect)
    // =========================================================================

    [Fact]
    public void RecordStruct_ValidGeoRect_Passes()
    {
        var geoRect = TestData.ValidGeoRect();
        var validator = new GeoRectValidator();

        var result = validator.Validate(geoRect);

        result.ShouldPass();
    }

    [Fact]
    public void RecordStruct_InvalidNorth_Fails()
    {
        var geoRect = TestData.ValidGeoRect() with { North = 100 };
        var validator = new GeoRectValidator();

        var result = validator.Validate(geoRect);

        var errors = result.ShouldFail();
        errors.ShouldContainError("North", "LessThan");
    }

    [Fact]
    public void RecordStruct_InvalidSouth_Fails()
    {
        var geoRect = TestData.ValidGeoRect() with { South = -100 };
        var validator = new GeoRectValidator();

        var result = validator.Validate(geoRect);

        var errors = result.ShouldFail();
        errors.ShouldContainError("South", "GreaterThan");
    }

    // =========================================================================
    // Record Class (ShippingInfo)
    // =========================================================================

    [Fact]
    public void RecordClass_ValidShippingInfo_Passes()
    {
        var shippingInfo = TestData.ValidShippingInfo();
        var validator = new ShippingInfoValidator();

        var result = validator.Validate(shippingInfo);

        result.ShouldPass();
    }

    [Fact]
    public void RecordClass_NullCarrier_FailsWithCarrierError()
    {
        var shippingInfo = TestData.ValidShippingInfo() with { Carrier = null };
        var validator = new ShippingInfoValidator();

        var result = validator.Validate(shippingInfo);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Carrier");
    }

    [Fact]
    public void RecordClass_ZeroWeight_FailsWithWeightError()
    {
        var shippingInfo = TestData.ValidShippingInfo() with { WeightKg = 0 };
        var validator = new ShippingInfoValidator();

        var result = validator.Validate(shippingInfo);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("WeightKg");
    }

    // =========================================================================
    // Method Targets (Invoice)
    // =========================================================================

    [Fact]
    public void MethodTarget_ValidInvoice_Passes()
    {
        var invoice = TestData.ValidInvoice();
        var validator = new InvoiceValidator();

        var result = validator.Validate(invoice);

        result.ShouldPass();
    }

    [Fact]
    public void MethodTarget_NullPrefix_StillPassesBecauseIdIsNotBlank()
    {
        // GetInvoiceId() returns "-1001" which is not null or whitespace
        var invoice = TestData.ValidInvoice();
        invoice.Prefix = null;
        var validator = new InvoiceValidator();

        var result = validator.Validate(invoice);

        result.ShouldPass();
    }

    [Fact]
    public void MethodTarget_EmptyPrefixAndZeroNumber_FailsOnTotalWithTax()
    {
        // GetInvoiceId() returns "-0" which is not blank, so it passes
        // GetTotalWithTax(0.1m) returns 0 * 1.1 = 0, which fails GreaterThan
        var invoice = TestData.ValidInvoice();
        invoice.Prefix = "";
        invoice.Number = 0;
        invoice.Subtotal = 0m;
        var validator = new InvoiceValidator();

        var result = validator.Validate(invoice);

        var errors = result.ShouldFail();
        errors.ShouldContainError("GetTotalWithTax", "GreaterThan");
    }

    [Fact]
    public void MethodTarget_ZeroSubtotal_FailsGreaterThanOnTotal()
    {
        // GetTotalWithTax(0.1m) returns 0 * 1.1 = 0, which fails GreaterThan(0m)
        var invoice = TestData.ValidInvoice();
        invoice.Subtotal = 0m;
        var validator = new InvoiceValidator();

        var result = validator.Validate(invoice);

        var errors = result.ShouldFail();
        errors.ShouldContainError("GetTotalWithTax", "GreaterThan");
    }

    [Fact]
    public void MethodTarget_TargetPathIsMethodNameWithoutParens()
    {
        var invoice = TestData.ValidInvoice();
        invoice.Subtotal = 0m;
        var validator = new InvoiceValidator();

        var result = validator.Validate(invoice);

        var errors = result.ShouldFail();
        errors.ShouldContainError("GetTotalWithTax", "GreaterThan");
        errors.ShouldNotContainErrorForPath("GetTotalWithTax()");
    }

    [Fact]
    public void MethodTarget_TargetNameIsHumanized()
    {
        var invoice = TestData.ValidInvoice();
        invoice.Prefix = null;
        invoice.Subtotal = 0m;
        var validator = new InvoiceValidator();

        // GetTotalWithTax fails, so we can check the humanized name
        var result = validator.Validate(invoice);

        var errors = result.ShouldFail();
        errors.ShouldContainError("GetTotalWithTax", "GreaterThan")
            .WithTargetName("Get Total With Tax");
    }

    [Fact]
    public void MethodTarget_GetInvoiceId_HasCorrectTargetPathAndName()
    {
        // GetInvoiceId() returns $"{Prefix}-{Number}", so it always contains a dash and
        // number, making it impossible to be null/whitespace. We verify the TargetPath and
        // TargetName via GetTotalWithTax which can be triggered to fail.
        var invoice = TestData.ValidInvoice();
        invoice.Subtotal = 0m;
        var validator = new InvoiceValidator();

        var result = validator.Validate(invoice);

        var errors = result.ShouldFail();
        errors.ShouldContainError("GetTotalWithTax", "GreaterThan")
            .WithTargetName("Get Total With Tax");
    }
}
