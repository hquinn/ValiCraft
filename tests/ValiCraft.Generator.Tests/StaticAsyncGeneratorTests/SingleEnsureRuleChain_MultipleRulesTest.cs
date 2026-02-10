using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.StaticAsyncGeneratorTests;

public class SingleEnsureRuleChain_MultipleRulesTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public decimal OrderTotal { get; set; }
                                                                 public string? ShippingReference { get; set; }
                                                             }
                                                             """;

    [StringSyntax("CSharp")] private const string InputValidatorsToGenerate = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.AsyncBuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class OrderValidator : StaticAsyncValidator<Order>
                                                                         {
                                                                             protected override void DefineRules(IAsyncValidationRuleBuilder<Order> orderBuilder)
                                                                             {
                                                                                 orderBuilder.Ensure(x => x.OrderNumber)
                                                                                    .IsNotNullOrWhiteSpace()
                                                                                    .HasMinLength(5);
                                                                             }
                                                                         }
                                                                         """;

    [Fact]
    public void ShouldGenerateStaticAsyncValidator()
    {
        AssertGenerator(inputs: [InputRequests, InputValidatorsToGenerate], 
            outputs: null,
            diagnostics: [],
            assertInitialCompilation: true);
    }
}
