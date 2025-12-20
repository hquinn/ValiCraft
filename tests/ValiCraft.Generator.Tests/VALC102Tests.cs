using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC102Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string Input = """
                                                          using System;
                                                          using ValiCraft;
                                                          using ValiCraft.Attributes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          [GenerateRuleExtension("IsNotEmpty")]
                                                          [DefaultMessage("'{TargetName}' must not be empty.")]
                                                          public class NotEmptyRule // Removed IValidationRule to trigger VALC102
                                                          {
                                                              public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
                                                          }
                                                          """;
    
    [Fact]
    public void ShouldReportVALC102()
    {
        AssertGenerator(inputs: [Input], 
            outputs: [],
            diagnostics: ["Missing ValiCraft.IValidationRule interface on Validation Rule marked with [GenerateRuleExtension]"],
            assertTrackingSteps: false);
    }
}