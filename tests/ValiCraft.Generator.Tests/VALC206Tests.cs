using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC206Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
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
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : Validator<User>
                                                                   {
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.WithOnFailure(OnFailureMode.Continue, userBuilder =>
                                                                           {
                                                                               builder.Ensure(x => x.Name)
                                                                                   .IsNotNullOrEmpty();
                                                                           });
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC206()
    {
        AssertGenerator(inputs: [InputRequests, InputValidator], 
            outputs: [],
            diagnostics: ["'builder' cannot be used in this scope. Try using 'userBuilder'."],
            assertTrackingSteps: false);
    }
}