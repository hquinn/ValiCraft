using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;

namespace ValiCraft.IntegrationTests.Tests;

public class NestedClassTests
{
    // =========================================================================
    // FeatureModule.NestedOrderValidator - Validator defined inside a nested class
    // =========================================================================

    [Fact]
    public void NestedValidator_ValidOrder_Passes()
    {
        var order = new FeatureModule.NestedOrder { Name = "Test Order", Total = 50m };
        var validator = new FeatureModule.NestedOrderValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void NestedValidator_NullName_FailsWithNameError()
    {
        var order = new FeatureModule.NestedOrder { Name = null, Total = 50m };
        var validator = new FeatureModule.NestedOrderValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Name");
    }

    [Fact]
    public void NestedValidator_ZeroTotal_FailsWithTotalError()
    {
        var order = new FeatureModule.NestedOrder { Name = "Test Order", Total = 0m };
        var validator = new FeatureModule.NestedOrderValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainErrorForPath("Total");
    }

    [Fact]
    public void NestedValidator_BothInvalid_ReturnsTwoErrors()
    {
        var order = new FeatureModule.NestedOrder { Name = null, Total = 0m };
        var validator = new FeatureModule.NestedOrderValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainErrorForPath("Name");
        errors.ShouldContainErrorForPath("Total");
    }
}
