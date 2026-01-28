using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC204Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  using System.Collections.Generic;
                                                                  
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class Order
                                                                  {
                                                                      public List<LineItem> LineItems { get; set; } = [];    
                                                                  }
                                                                  
                                                                  public class LineItem
                                                                  {
                                                                      public string? SKU { get; set; }
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
                                                                           builder.EnsureEach(x => x.LineItems, l => DefineFavouriteNumbersRules(l));
                                                                       }
                                                                       
                                                                       private void DefineFavouriteNumbersRules(IValidationRuleBuilder<LineItem> builder)
                                                                       {
                                                                           builder.Ensure(x => x.SKU).IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC204()
    {
        AssertGenerator(inputs: [InputRequests, InputValidator], 
            outputs: [],
            diagnostics: [$"EnsureEach expects a lambda as the last parameter."],
            assertTrackingSteps: false);
    }
}