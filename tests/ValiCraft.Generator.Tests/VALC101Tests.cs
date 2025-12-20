using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC101Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string Input = """
                                                          using System;
                                                          using ValiCraft;
                                                          using ValiCraft.Attributes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          // Set null here to trigger VALC101
                                                          [GenerateRuleExtension(null)]
                                                          [DefaultMessage("'{TargetName}' must not be empty.")]
                                                          public class NotEmptyRule : IValidationRule<string?>
                                                          {
                                                              public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
                                                          }
                                                          """;
    
    [Fact]
    public void ShouldReportVALC101()
    {
        AssertGenerator(inputs: [Input], 
            outputs: [],
            diagnostics: ["Missing Validation Rule Extension Name"],
            assertTrackingSteps: false);
    }
}