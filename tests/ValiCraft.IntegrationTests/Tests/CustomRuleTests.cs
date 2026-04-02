using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class CustomRuleTests
{
    // =====================================================================
    // Custom rule without parameters
    // =====================================================================

    [Fact]
    public void CustomRule_ValidZipCode_Passes()
    {
        var validator = new AddressCustomRuleValidator();
        var address = TestData.ValidAddress();
        address.ZipCode = "62704";

        var result = validator.Validate(address);
        result.ShouldPass();
    }

    [Fact]
    public void CustomRule_ValidZipCodeWithExtension_Passes()
    {
        var validator = new AddressCustomRuleValidator();
        var address = TestData.ValidAddress();
        address.ZipCode = "62704-1234";

        var result = validator.Validate(address);
        result.ShouldPass();
    }

    [Fact]
    public void CustomRule_InvalidZipCode_Fails()
    {
        var validator = new AddressCustomRuleValidator();
        var address = TestData.ValidAddress();
        address.ZipCode = "ABC";

        var errors = validator.Validate(address).ShouldFail();
        errors.ShouldContainError("ZipCode", "UsPostalCode");
    }

    [Fact]
    public void CustomRule_NullZipCode_Fails()
    {
        var validator = new AddressCustomRuleValidator();
        var address = TestData.ValidAddress();
        address.ZipCode = null;

        var errors = validator.Validate(address).ShouldFail();
        errors.ShouldContainError("ZipCode", "UsPostalCode");
    }

    // =====================================================================
    // Custom rule with parameter
    // =====================================================================

    [Fact]
    public void CustomRuleWithParameter_ValidDivisibility_Passes()
    {
        var validator = new OrderCustomRuleValidator();
        var order = TestData.ValidOrder();
        order.Quantity = 10;

        var result = validator.Validate(order);
        result.ShouldPass();
    }

    [Fact]
    public void CustomRuleWithParameter_InvalidDivisibility_Fails()
    {
        var validator = new OrderCustomRuleValidator();
        var order = TestData.ValidOrder();
        order.Quantity = 7;

        var errors = validator.Validate(order).ShouldFail();
        errors.ShouldContainError("Quantity", "DivisibleBy");
    }

    [Fact]
    public void CustomRuleWithParameter_Zero_Passes()
    {
        var validator = new OrderCustomRuleValidator();
        var order = TestData.ValidOrder();
        order.Quantity = 0; // 0 is divisible by 5

        var result = validator.Validate(order);
        result.ShouldPass();
    }
}
