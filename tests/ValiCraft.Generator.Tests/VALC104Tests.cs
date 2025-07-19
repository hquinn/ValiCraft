using System.Diagnostics.CodeAnalysis;
using MonadCraft;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC104Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string Input = """
                                                          using System;
                                                          using ValiCraft;
                                                          using ValiCraft.Attributes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          [GenerateRuleExtension("IsEqual")]
                                                          [DefaultMessage("'{TargetName}' must be equal.")]
                                                          [RulePlaceholder("{ValueToCompare}", "test")]
                                                          public class EqualRule<TTargetType> : IValidationRule<TTargetType, TTargetType>
                                                              where TTargetType : IEquatable<TTargetType>
                                                          {
                                                              public static bool IsValid(TTargetType property, TTargetType parameter)
                                                              {
                                                                  return property.Equals(parameter);
                                                              }
                                                          }
                                                          """;
    
    [Fact]
    public void ShouldReportVALC104()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName], 
            inputs: [Input], 
            outputs: [],
            diagnostics: ["Parameter name 'test' is invalid. It must match a parameter name from the IsValid method."],
            assertTrackingSteps: false);
    }
}