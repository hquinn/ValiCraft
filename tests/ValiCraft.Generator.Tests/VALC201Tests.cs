using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC201Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
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
                                                                   
                                                                   // Removed partial keyword to trigger VALC201
                                                                   [GenerateValidator]
                                                                   public class UserValidator : Validator<User>
                                                                   {
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC201()
    {
        AssertGenerator(inputs: [InputRequests, InputValidator], 
            outputs: [],
            diagnostics: ["Missing partial keyword on Validator marked with [GenerateValidator]"],
            assertTrackingSteps: false);
    }
}