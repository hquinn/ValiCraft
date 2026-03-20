using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class ConditionalValidationTests
{
    [Fact]
    public void ValidOrder_NotExpressNotInternational_Passes()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void ExpressOrder_WithValidNotes_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.Notes = "Rush delivery requested";
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void ExpressOrder_WithNullNotes_FailsWithNotesError()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.Notes = null;
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Notes");
    }

    [Fact]
    public void NonExpressOrder_WithNullNotes_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = false;
        order.Notes = null;
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void InternationalOrder_WithValidAddress_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsInternational = true;
        order.ShippingAddress = TestData.ValidAddress();
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void InternationalOrder_WithInvalidAddress_FailsWithAddressErrors()
    {
        var order = TestData.ValidOrder();
        order.IsInternational = true;
        order.ShippingAddress = new Address
        {
            Street = "",
            City = "",
            ZipCode = ""
        };
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("ShippingAddress.Street");
        errors.ShouldContainErrorForPath("ShippingAddress.City");
        errors.ShouldContainErrorForPath("ShippingAddress.ZipCode");
    }

    [Fact]
    public void NonInternationalOrder_WithInvalidAddress_Passes()
    {
        var order = TestData.ValidOrder();
        order.IsInternational = false;
        order.ShippingAddress = new Address
        {
            Street = "",
            City = "",
            ZipCode = ""
        };
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void ExpressAndInternational_BothInvalid_FailsWithNotesAndAddressErrors()
    {
        var order = TestData.ValidOrder();
        order.IsExpress = true;
        order.IsInternational = true;
        order.Notes = null;
        order.ShippingAddress = new Address
        {
            Street = "",
            City = "",
            ZipCode = ""
        };
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Notes");
        errors.ShouldContainErrorForPath("ShippingAddress.Street");
        errors.ShouldContainErrorForPath("ShippingAddress.City");
        errors.ShouldContainErrorForPath("ShippingAddress.ZipCode");
    }

    [Fact]
    public void InvalidOrderNumber_RegardlessOfConditions_AlwaysFails()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderConditionalValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
    }
}
