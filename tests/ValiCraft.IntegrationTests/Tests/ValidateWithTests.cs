using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class ValidateWithTests
{
    // =========================================================================
    // OrderNestedValidator - ValidateWith nested object validation
    // =========================================================================

    [Fact]
    public void OrderNestedValidator_ValidOrderWithValidAddress_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderNestedValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderNestedValidator_InvalidAddress_PropagatesChildErrors()
    {
        var order = TestData.ValidOrder();
        order.ShippingAddress = new Address
        {
            Street = "",
            City = "",
            ZipCode = ""
        };
        var validator = new OrderNestedValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(3);
        errors.ShouldContainErrorForPath("ShippingAddress.Street");
        errors.ShouldContainErrorForPath("ShippingAddress.City");
        errors.ShouldContainErrorForPath("ShippingAddress.ZipCode");
    }

    [Fact]
    public void OrderNestedValidator_InvalidAddress_TargetPathPrefixIsCorrect()
    {
        var order = TestData.ValidOrder();
        order.ShippingAddress = new Address
        {
            Street = null,
            City = null,
            ZipCode = null
        };
        var validator = new OrderNestedValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(3);
        errors.ShouldContainError("ShippingAddress.Street", "NotNullOrWhiteSpace");
        errors.ShouldContainError("ShippingAddress.City", "NotNullOrWhiteSpace");
        errors.ShouldContainError("ShippingAddress.ZipCode", "NotNullOrWhiteSpace");
    }

    [Fact]
    public void OrderNestedValidator_ValidAddressButInvalidOrderNumber_OnlyOrderNumberError()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        var validator = new OrderNestedValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
        errors.ShouldNotContainErrorForPath("ShippingAddress.Street");
        errors.ShouldNotContainErrorForPath("ShippingAddress.City");
        errors.ShouldNotContainErrorForPath("ShippingAddress.ZipCode");
    }

    [Fact]
    public void OrderNestedValidator_BothInvalid_ErrorsFromParentAndChild()
    {
        var order = TestData.ValidOrder();
        order.OrderNumber = null;
        order.ShippingAddress = new Address
        {
            Street = null,
            City = null,
            ZipCode = null
        };
        var validator = new OrderNestedValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(4);
        errors.ShouldContainError("OrderNumber", "NotNullOrWhiteSpace");
        errors.ShouldContainError("ShippingAddress.Street", "NotNullOrWhiteSpace");
        errors.ShouldContainError("ShippingAddress.City", "NotNullOrWhiteSpace");
        errors.ShouldContainError("ShippingAddress.ZipCode", "NotNullOrWhiteSpace");
    }

    // =========================================================================
    // OrderRulesThenValidateWithValidator - ValidateWith without parent rules
    // =========================================================================

    [Fact]
    public void OrderRulesThenValidateWithValidator_ValidAddress_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderRulesThenValidateWithValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void OrderRulesThenValidateWithValidator_InvalidAddress_ProducesNestedErrors()
    {
        var order = TestData.ValidOrder();
        order.ShippingAddress = new Address
        {
            Street = null,
            City = null,
            ZipCode = null
        };
        var validator = new OrderRulesThenValidateWithValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(3);
        errors.ShouldContainError("ShippingAddress.Street", "NotNullOrWhiteSpace");
        errors.ShouldContainError("ShippingAddress.City", "NotNullOrWhiteSpace");
        errors.ShouldContainError("ShippingAddress.ZipCode", "NotNullOrWhiteSpace");
    }
}
