using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC207Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class Order
                                                                  {
                                                                      public required string OrderNumber { get; set; }
                                                                  }
                                                                  """;
    
    // Extension method without MapToValidationRule attribute
    [StringSyntax("CSharp")] private const string InputExtensions = """
                                                                    using ValiCraft.BuilderTypes;
                                                                    
                                                                    namespace Test.Extensions;
                                                                    
                                                                    public static class CustomRuleExtensions
                                                                    {
                                                                        // Missing [MapToValidationRule] attribute
                                                                        public static IValidationRuleBuilderType<TRequest, string> IsCustomFormat<TRequest>(
                                                                            this IBuilderType<TRequest, string> builder)
                                                                            where TRequest : class
                                                                        {
                                                                            return builder.Is(value => !string.IsNullOrEmpty(value) && value.StartsWith("ORD-"));
                                                                        }
                                                                    }
                                                                    """;
    
    [StringSyntax("CSharp")] private const string InputValidator = """
                                                                   using Test.Requests;
                                                                   using Test.Extensions;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class OrderValidator : Validator<Order>
                                                                   {
                                                                       protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                       {
                                                                           builder.Ensure(x => x.OrderNumber)
                                                                               .IsCustomFormat();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC207()
    {
        AssertGenerator(inputs: [InputRequests, InputExtensions, InputValidator], 
            outputs: [],
            diagnostics: ["Missing MapToValidationRule attribute on extenstion method."],
            assertTrackingSteps: false);
    }
}
