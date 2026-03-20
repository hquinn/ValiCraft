using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class EnsureEachTests
{
    // =========================================================================
    // OrderEnsureEachDirectValidator - Direct rules on collection items
    // =========================================================================

    [Fact]
    public void DirectValidator_ValidTags_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderEnsureEachDirectValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void DirectValidator_EmptyCollection_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        order.Tags = [];
        var validator = new OrderEnsureEachDirectValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void DirectValidator_OneInvalidTag_FailsWithNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.Tags = [""];
        var validator = new OrderEnsureEachDirectValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(1);
        errors.ShouldContainErrorForPath("")
            .WithTargetName("Tags");
    }

    [Fact]
    public void DirectValidator_MultipleInvalidTags_ReturnsErrorForEach()
    {
        var order = TestData.ValidOrder();
        order.Tags = ["", "   ", null!];
        var validator = new OrderEnsureEachDirectValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(3);
    }

    [Fact]
    public void DirectValidator_MixOfValidAndInvalidTags_OnlyInvalidHaveErrors()
    {
        var order = TestData.ValidOrder();
        order.Tags = ["valid", "", "also-valid", "   "];
        var validator = new OrderEnsureEachDirectValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
    }

    // =========================================================================
    // OrderEnsureEachValidateWithValidator - ValidateWith on collection items
    // =========================================================================

    [Fact]
    public void ValidateWithValidator_ValidLineItems_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderEnsureEachValidateWithValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void ValidateWithValidator_OneInvalidLineItem_FailsWithCorrectPaths()
    {
        var order = TestData.ValidOrder();
        order.LineItems = [new LineItem { Code = null, Price = 0m, Quantity = 1 }];
        var validator = new OrderEnsureEachValidateWithValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[0].Price", "GreaterThan");
    }

    [Fact]
    public void ValidateWithValidator_SecondItemInvalid_FailsAtCorrectIndex()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            TestData.ValidLineItem(),
            new LineItem { Code = null, Price = 0m, Quantity = 1 }
        ];
        var validator = new OrderEnsureEachValidateWithValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[1].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[1].Price", "GreaterThan");
        errors.ShouldNotContainErrorForPath("LineItems[0].Code");
        errors.ShouldNotContainErrorForPath("LineItems[0].Price");
    }

    [Fact]
    public void ValidateWithValidator_VerifyFullTargetPaths()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            new LineItem { Code = null, Price = 0m, Quantity = 1 },
            new LineItem { Code = null, Price = 0m, Quantity = 1 }
        ];
        var validator = new OrderEnsureEachValidateWithValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(4);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[0].Price", "GreaterThan");
        errors.ShouldContainError("LineItems[1].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[1].Price", "GreaterThan");
    }

    // =========================================================================
    // OrderEnsureEachLambdaValidator - Inline lambda rules on collection items
    // =========================================================================

    [Fact]
    public void LambdaValidator_ValidLineItems_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderEnsureEachLambdaValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void LambdaValidator_OneInvalidLineItem_FailsWithCorrectPaths()
    {
        var order = TestData.ValidOrder();
        order.LineItems = [new LineItem { Code = null, Price = 0m, Quantity = 1 }];
        var validator = new OrderEnsureEachLambdaValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[0].Price", "GreaterThan");
    }

    [Fact]
    public void LambdaValidator_SecondItemInvalid_FailsAtCorrectIndex()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            TestData.ValidLineItem(),
            new LineItem { Code = null, Price = 0m, Quantity = 1 }
        ];
        var validator = new OrderEnsureEachLambdaValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[1].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[1].Price", "GreaterThan");
        errors.ShouldNotContainErrorForPath("LineItems[0].Code");
        errors.ShouldNotContainErrorForPath("LineItems[0].Price");
    }

    [Fact]
    public void LambdaValidator_MultipleInvalidItems_FailsWithCorrectPaths()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            new LineItem { Code = null, Price = 0m, Quantity = 1 },
            new LineItem { Code = null, Price = 0m, Quantity = 1 }
        ];
        var validator = new OrderEnsureEachLambdaValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(4);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[0].Price", "GreaterThan");
        errors.ShouldContainError("LineItems[1].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[1].Price", "GreaterThan");
    }

    // =========================================================================
    // OrderEnsureEachWithIfValidator - If condition inside EnsureEach
    // =========================================================================

    [Fact]
    public void WithIfValidator_ItemWithPositiveQuantityAndNullCode_Fails()
    {
        var order = TestData.ValidOrder();
        order.LineItems = [new LineItem { Code = null, Price = 10m, Quantity = 5 }];
        var validator = new OrderEnsureEachWithIfValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(1);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
    }

    [Fact]
    public void WithIfValidator_ItemWithZeroQuantityAndNullCode_Passes()
    {
        var order = TestData.ValidOrder();
        order.LineItems = [new LineItem { Code = null, Price = 10m, Quantity = 0 }];
        var validator = new OrderEnsureEachWithIfValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void WithIfValidator_ItemWithNegativeQuantityAndNullCode_Passes()
    {
        var order = TestData.ValidOrder();
        order.LineItems = [new LineItem { Code = null, Price = 10m, Quantity = -1 }];
        var validator = new OrderEnsureEachWithIfValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void WithIfValidator_MixOfItems_OnlyConditionMetItemsValidated()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            new LineItem { Code = null, Price = 10m, Quantity = 5 },  // Quantity > 0, null Code -> error
            new LineItem { Code = null, Price = 10m, Quantity = 0 },  // Quantity <= 0, null Code -> skipped
            new LineItem { Code = null, Price = 10m, Quantity = 3 }   // Quantity > 0, null Code -> error
        ];
        var validator = new OrderEnsureEachWithIfValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFailWith(2);
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[2].Code", "NotNullOrWhiteSpace");
        errors.ShouldNotContainErrorForPath("LineItems[1].Code");
    }

    [Fact]
    public void WithIfValidator_AllItemsMeetConditionWithValidCode_Passes()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            TestData.ValidLineItem(),
            TestData.ValidLineItem("SKU-002", 29.99m)
        ];
        var validator = new OrderEnsureEachWithIfValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    // =========================================================================
    // OrderEnsureEachRulesThenValidateWithValidator - Rules then ValidateWith
    // =========================================================================

    [Fact]
    public void RulesThenValidateWithValidator_ValidLineItems_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderEnsureEachRulesThenValidateWithValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void RulesThenValidateWithValidator_InvalidLineItem_FailsNestedValidation()
    {
        var order = TestData.ValidOrder();
        order.LineItems =
        [
            new LineItem { Code = null, Price = 0m, Quantity = 1 }
        ];
        var validator = new OrderEnsureEachRulesThenValidateWithValidator();

        var result = validator.Validate(order);

        var errors = result.ShouldFail();
        errors.ShouldContainError("LineItems[0].Code", "NotNullOrWhiteSpace");
        errors.ShouldContainError("LineItems[0].Price", "GreaterThan");
    }

    // =========================================================================
    // OrderEnsureEachHaltValidator - Halt mode inside EnsureEach
    // =========================================================================

    [Fact]
    public void HaltValidator_ValidTags_ReturnsNoErrors()
    {
        var order = TestData.ValidOrder();
        var validator = new OrderEnsureEachHaltValidator();

        var result = validator.Validate(order);

        result.ShouldPass();
    }

    [Fact]
    public void HaltValidator_EmptyTag_HaltsAfterNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.Tags = [""];
        var validator = new OrderEnsureEachHaltValidator();

        var result = validator.Validate(order);

        // Halt mode: empty string fails NotNullOrWhiteSpace, halts before MinLength
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainErrorForPath("")
            .WithTargetName("Tags");
    }

    [Fact]
    public void HaltValidator_NullTag_HaltsAfterNotNullOrWhiteSpace()
    {
        var order = TestData.ValidOrder();
        order.Tags = [null!];
        var validator = new OrderEnsureEachHaltValidator();

        var result = validator.Validate(order);

        // Halt mode: null fails NotNullOrWhiteSpace, halts before MinLength
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainErrorForPath("")
            .WithTargetName("Tags");
    }

    [Fact]
    public void HaltValidator_SingleCharTag_PassesNotNullOrWhiteSpaceFailsMinLength()
    {
        var order = TestData.ValidOrder();
        order.Tags = ["a"];
        var validator = new OrderEnsureEachHaltValidator();

        var result = validator.Validate(order);

        // "a" passes NotNullOrWhiteSpace but fails MinLength(2)
        var errors = result.ShouldFailWith(1);
        errors.ShouldContainErrorForPath("")
            .WithTargetName("Tags");
    }

    [Fact]
    public void HaltValidator_MultipleTagsFirstInvalid_HaltsEntireLoop()
    {
        var order = TestData.ValidOrder();
        order.Tags = ["", "valid-tag"];
        var validator = new OrderEnsureEachHaltValidator();

        var result = validator.Validate(order);

        // Halt exits the entire loop after the first tag fails, so second tag is never checked
        var errors = result.ShouldFailWith(1);
    }
}
