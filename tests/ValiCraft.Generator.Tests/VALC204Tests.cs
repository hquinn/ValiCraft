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
    
    [StringSyntax("CSharp")] private const string InputValidationRules = """
                                                          using System;
                                                          using ValiCraft;
                                                          using ValiCraft.Attributes;
                                                          using ValiCraft.BuilderTypes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          [DefaultMessage("'{TargetName}' must not be empty.")]
                                                          public class NotEmptyRule: IValidationRule<string?>
                                                          {
                                                              public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
                                                          }
                                                          
                                                          [DefaultMessage("'{TargetName}' must not be empty.")]
                                                          public static class NotEmptyRuleExtensions
                                                          {
                                                              [MapToValidationRule(typeof(NotEmptyRule), "")]
                                                              public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotEmpty<TRequest, TTargetType>(
                                                                  this IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                                                                  => throw new NotImplementedException("Never gets called");
                                                          }
                                                          """;
    
    [StringSyntax("CSharp")] private const string InputValidator = """
                                                                   using Test.Rules;
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
                                                                           builder.Ensure(x => x.SKU).IsNotEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC204()
    {
        AssertGenerator(inputs: [InputRequests, InputValidationRules, InputValidator], 
            outputs: [],
            diagnostics: [$"EnsureEach expects a lambda as the last parameter."],
            assertTrackingSteps: false);
    }
}