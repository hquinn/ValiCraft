using System.Diagnostics.CodeAnalysis;
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
    
    [StringSyntax("CSharp")] private const string InputValidator = """
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
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC202()
    {
        AssertGenerator(inputs: [InputRequests, InputValidator], 
            outputs: [],
            diagnostics: ["Missing Validator base class on Validator marked with [GenerateValidator]"],
            assertTrackingSteps: false);
    }
}