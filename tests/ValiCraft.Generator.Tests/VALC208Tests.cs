using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC208Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;

                                                                  public class Order
                                                                  {
                                                                      public required string OrderNumber { get; set; }
                                                                      public required int Quantity { get; set; }
                                                                  }
                                                                  """;

    [StringSyntax("CSharp")] private const string InputLocalDeclaration = """
                                                                          using Test.Requests;
                                                                          using ValiCraft;
                                                                          using ValiCraft.Attributes;
                                                                          using ValiCraft.BuilderTypes;

                                                                          namespace Test.Validators;

                                                                          [GenerateValidator]
                                                                          public partial class OrderValidator : Validator<Order>
                                                                          {
                                                                              protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                              {
                                                                                  int argument = 2;
                                                                              }
                                                                          }
                                                                          """;

    [StringSyntax("CSharp")] private const string InputIfStatement = """
                                                                     using Test.Requests;
                                                                     using ValiCraft;
                                                                     using ValiCraft.Attributes;
                                                                     using ValiCraft.BuilderTypes;

                                                                     namespace Test.Validators;

                                                                     [GenerateValidator]
                                                                     public partial class OrderValidator : Validator<Order>
                                                                     {
                                                                         protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                         {
                                                                             if (true)
                                                                             {
                                                                                 builder.Ensure(x => x.OrderNumber).IsNotNullOrEmpty();
                                                                             }
                                                                         }
                                                                     }
                                                                     """;

    [StringSyntax("CSharp")] private const string InputForEachStatement = """
                                                                          using Test.Requests;
                                                                          using ValiCraft;
                                                                          using ValiCraft.Attributes;
                                                                          using ValiCraft.BuilderTypes;

                                                                          namespace Test.Validators;

                                                                          [GenerateValidator]
                                                                          public partial class OrderValidator : Validator<Order>
                                                                          {
                                                                              protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                              {
                                                                                  foreach (var item in new[] { 1, 2, 3 })
                                                                                  {
                                                                                  }
                                                                              }
                                                                          }
                                                                          """;

    [StringSyntax("CSharp")] private const string InputReturnStatement = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class OrderValidator : Validator<Order>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                             {
                                                                                 return;
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string InputThrowStatement = """
                                                                        using System;
                                                                        using Test.Requests;
                                                                        using ValiCraft;
                                                                        using ValiCraft.Attributes;
                                                                        using ValiCraft.BuilderTypes;

                                                                        namespace Test.Validators;

                                                                        [GenerateValidator]
                                                                        public partial class OrderValidator : Validator<Order>
                                                                        {
                                                                            protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                            {
                                                                                throw new InvalidOperationException();
                                                                            }
                                                                        }
                                                                        """;

    [StringSyntax("CSharp")] private const string InputTryCatchStatement = """
                                                                           using System;
                                                                           using Test.Requests;
                                                                           using ValiCraft;
                                                                           using ValiCraft.Attributes;
                                                                           using ValiCraft.BuilderTypes;

                                                                           namespace Test.Validators;

                                                                           [GenerateValidator]
                                                                           public partial class OrderValidator : Validator<Order>
                                                                           {
                                                                               protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                               {
                                                                                   try
                                                                                   {
                                                                                       builder.Ensure(x => x.OrderNumber).IsNotNullOrEmpty();
                                                                                   }
                                                                                   catch (Exception)
                                                                                   {
                                                                                   }
                                                                               }
                                                                           }
                                                                           """;

    [StringSyntax("CSharp")] private const string InputMultipleInvalidStatements = """
                                                                                   using Test.Requests;
                                                                                   using ValiCraft;
                                                                                   using ValiCraft.Attributes;
                                                                                   using ValiCraft.BuilderTypes;

                                                                                   namespace Test.Validators;

                                                                                   [GenerateValidator]
                                                                                   public partial class OrderValidator : Validator<Order>
                                                                                   {
                                                                                       protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                                       {
                                                                                           int argument = 2;
                                                                                           var name = "test";
                                                                                           builder.Ensure(x => x.OrderNumber).IsNotNullOrEmpty();
                                                                                       }
                                                                                   }
                                                                                   """;

    [Fact]
    public void ShouldReportVALC208_ForLocalVariableDeclaration()
    {
        AssertGenerator(inputs: [InputRequests, InputLocalDeclaration],
            outputs: [],
            diagnostics: [
                "Only builder invocations are allowed inside DefineRules. Local variable declarations are not supported."
            ],
            assertTrackingSteps: false);
    }

    [Fact]
    public void ShouldReportVALC208_ForIfStatement()
    {
        AssertGenerator(inputs: [InputRequests, InputIfStatement],
            outputs: [],
            diagnostics: [
                "Only builder invocations are allowed inside DefineRules. If statements are not supported."
            ],
            assertTrackingSteps: false);
    }

    [Fact]
    public void ShouldReportVALC208_ForForEachStatement()
    {
        AssertGenerator(inputs: [InputRequests, InputForEachStatement],
            outputs: [],
            diagnostics: [
                "Only builder invocations are allowed inside DefineRules. ForEach statements are not supported."
            ],
            assertTrackingSteps: false);
    }

    [Fact]
    public void ShouldReportVALC208_ForReturnStatement()
    {
        AssertGenerator(inputs: [InputRequests, InputReturnStatement],
            outputs: [],
            diagnostics: [
                "Only builder invocations are allowed inside DefineRules. Return statements are not supported."
            ],
            assertTrackingSteps: false);
    }

    [Fact]
    public void ShouldReportVALC208_ForThrowStatement()
    {
        AssertGenerator(inputs: [InputRequests, InputThrowStatement],
            outputs: [],
            diagnostics: [
                "Only builder invocations are allowed inside DefineRules. Throw statements are not supported."
            ],
            assertTrackingSteps: false);
    }

    [Fact]
    public void ShouldReportVALC208_ForTryCatchStatement()
    {
        AssertGenerator(inputs: [InputRequests, InputTryCatchStatement],
            outputs: [],
            diagnostics: [
                "Only builder invocations are allowed inside DefineRules. Try statements are not supported."
            ],
            assertTrackingSteps: false);
    }

    [Fact]
    public void ShouldReportVALC208_ForMultipleInvalidStatements()
    {
        AssertGenerator(inputs: [InputRequests, InputMultipleInvalidStatements],
            outputs: [],
            diagnostics: [
                "Only builder invocations are allowed inside DefineRules. Local variable declarations are not supported.",
                "Only builder invocations are allowed inside DefineRules. Local variable declarations are not supported."
            ],
            assertTrackingSteps: false);
    }
}
