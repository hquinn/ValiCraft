using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC203Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class Order
                                                                  {
                                                                      public required string OrderNumber { get; set; }
                                                                  }
                                                                  """;
    
    [StringSyntax("CSharp")] private const string InputValidator = """
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
                                                                           // Call a method that doesn't exist - invalid rule invocation
                                                                           builder.Ensure(x => x.OrderNumber)
                                                                               .NonExistentRule();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC203()
    {
        AssertGenerator(inputs: [InputRequests, InputValidator], 
            outputs: [],
            diagnostics: [
                "Invalid rule invocation. Either use the .Is() method or define an extension method.",
                "'IEnsureBuilderType<Order, string>' does not contain a definition for 'NonExistentRule' and no accessible extension method 'NonExistentRule' accepting a first argument of type 'IEnsureBuilderType<Order, string>' could be found (are you missing a using directive or an assembly reference?)"
            ],
            assertTrackingSteps: false);
    }
}
