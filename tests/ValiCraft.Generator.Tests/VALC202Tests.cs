using System.Diagnostics.CodeAnalysis;
using LitePrimitives;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC202Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class User
                                                                  {
                                                                      public string? Name { get; set; }    
                                                                  }
                                                                  """;
    [StringSyntax("CSharp")] private const string InputValidationRules = """
                                                          using System;
                                                          using ValiCraft;
                                                          using ValiCraft.Attributes;
                                                          using ValiCraft.BuilderTypes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          [DefaultMessage("'{PropertyName}' must not be empty.")]
                                                          public class NotEmptyRule: IValidationRule<string?>
                                                          {
                                                              public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
                                                          }
                                                          
                                                          [DefaultMessage("'{PropertyName}' must not be empty.")]
                                                          public static class NotEmptyRuleExtensions
                                                          {
                                                              [MapToValidationRule(typeof(NotEmptyRule), "")]
                                                              public static IValidationRuleBuilderType<TRequest, TPropertyType> IsNotEmpty<TRequest, TPropertyType>(
                                                                  this IBuilderType<TRequest, TPropertyType> builder) where TRequest : class
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
                                                                   
                                                                   // Removed Validator<User> to trigger VALC202
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator
                                                                   {
                                                                       protected void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC202()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Validation<>)],
            trackingSteps: [TrackingSteps.ValidationRuleInfoResultTrackingName, TrackingSteps.ValidatorInfoResultTrackingName], 
            inputs: [InputRequests, InputValidationRules, InputValidator], 
            outputs: [],
            diagnostics: ["Missing Validator base class on Validator marked with [GenerateValidator]"],
            assertTrackingSteps: false);
    }
}