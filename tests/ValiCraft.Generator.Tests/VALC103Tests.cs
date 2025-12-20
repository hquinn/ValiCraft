using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC103Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string FirstInput = """
                                                          using System;
                                                          using ValiCraft;
                                                          using ValiCraft.Attributes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          [GenerateRuleExtension("IsEqual")]
                                                          [DefaultMessage("'{TargetName}' must be equal.")]
                                                          [RulePlaceholder(null, "parameter")]
                                                          public class EqualRule<TTargetType> : IValidationRule<TTargetType, TTargetType>
                                                              where TTargetType : IEquatable<TTargetType>
                                                          {
                                                              public static bool IsValid(TTargetType property, TTargetType parameter)
                                                              {
                                                                  return property.Equals(parameter);
                                                              }
                                                          }
                                                          """;
    
    [StringSyntax("CSharp")] private const string SecondInput = """
                                                               using System;
                                                               using ValiCraft;
                                                               using ValiCraft.Attributes;

                                                               namespace Test.Rules;

                                                               [GenerateRuleExtension("IsEqual")]
                                                               [DefaultMessage("'{TargetName}' must be equal.")]
                                                               [RulePlaceholder("{ValueToCompare}", null)]
                                                               public class EqualRule<TTargetType> : IValidationRule<TTargetType, TTargetType>
                                                                   where TTargetType : IEquatable<TTargetType>
                                                               {
                                                                   public static bool IsValid(TTargetType property, TTargetType parameter)
                                                                   {
                                                                       return property.Equals(parameter);
                                                                   }
                                                               }
                                                               """;
    
    [Theory]
    [InlineData(FirstInput)]
    [InlineData(SecondInput)]
    public void ShouldReportVALC103(string input)
    {
        AssertGenerator(inputs: [input], 
            outputs: [],
            diagnostics: ["Placeholder constructor argument must be a string literal."],
            assertTrackingSteps: false);
    }
}