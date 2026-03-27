using ValiCraft.IntegrationTests.Helpers;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests.Tests;

public class ConcurrencyAndStressTests
{
    // =====================================================================
    // Thread Safety — shared validator instance across parallel threads
    // =====================================================================

    [Fact]
    public void SharedValidatorInstance_ConcurrentValidCalls_AllPass()
    {
        var validator = new OrderBasicValidator();

        Parallel.For(0, 100, _ =>
        {
            var order = TestData.ValidOrder();
            var result = validator.Validate(order);
            result.ShouldPass();
        });
    }

    [Fact]
    public void SharedValidatorInstance_ConcurrentInvalidCalls_AllFail()
    {
        var validator = new OrderBasicValidator();

        Parallel.For(0, 100, _ =>
        {
            var order = TestData.ValidOrder();
            order.OrderNumber = null;
            order.OrderTotal = -1m;

            var errors = validator.Validate(order).ShouldFail();
            errors.ShouldContainErrorForPath("OrderNumber");
            errors.ShouldContainErrorForPath("OrderTotal");
        });
    }

    [Fact]
    public void SharedValidatorInstance_MixedValidAndInvalidCalls_CorrectResults()
    {
        var validator = new OrderBasicValidator();

        Parallel.For(0, 200, i =>
        {
            var order = TestData.ValidOrder();

            if (i % 2 == 0)
            {
                // Valid
                var result = validator.Validate(order);
                result.ShouldPass();
            }
            else
            {
                // Invalid
                order.OrderNumber = null;
                var errors = validator.Validate(order).ShouldFail();
                errors.ShouldContainErrorForPath("OrderNumber");
            }
        });
    }

    [Fact]
    public async Task SharedValidatorInstance_ConcurrentCallsViaTaskWhenAll_AllPass()
    {
        var validator = new OrderBasicValidator();

        var tasks = Enumerable.Range(0, 100).Select(_ => Task.Run(() =>
        {
            var order = TestData.ValidOrder();
            var result = validator.Validate(order);
            result.ShouldPass();
        }));

        await Task.WhenAll(tasks);
    }

    // =====================================================================
    // Large Collection Stress Tests
    // =====================================================================

    [Fact]
    public void LargeCollection_AllValidItems_Passes()
    {
        var validator = new OrderEnsureEachDirectValidator();
        var order = TestData.ValidOrder();
        order.Tags = Enumerable.Range(0, 10_000).Select(i => $"tag-{i}").ToList();

        var result = validator.Validate(order);
        result.ShouldPass();
    }

    [Fact]
    public void LargeCollection_SingleInvalidItem_ReportsCorrectError()
    {
        var validator = new OrderEnsureEachDirectValidator();
        var order = TestData.ValidOrder();
        order.Tags = Enumerable.Range(0, 10_000).Select(i => $"tag-{i}").ToList();
        order.Tags[5_000] = "   "; // Invalid: whitespace-only

        var errors = validator.Validate(order).ShouldFailWith(1);
    }

    [Fact]
    public void LargeCollection_AllInvalidItems_ReportsAllErrors()
    {
        var validator = new OrderEnsureEachDirectValidator();
        var order = TestData.ValidOrder();
        order.Tags = Enumerable.Repeat("", 1_000).ToList();

        var errors = validator.Validate(order).ShouldFailWith(1_000);
    }

    [Fact]
    public void LargeCollection_WithNestedValidation_AllValidItems_Passes()
    {
        var validator = new OrderEnsureEachValidateWithValidator();
        var order = TestData.ValidOrder();
        order.LineItems = Enumerable.Range(0, 10_000)
            .Select(i => TestData.ValidLineItem($"SKU-{i:D5}", 9.99m))
            .ToList();

        var result = validator.Validate(order);
        result.ShouldPass();
    }
}
