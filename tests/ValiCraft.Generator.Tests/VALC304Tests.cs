using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC304Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class User
                                                                  {
                                                                      public string? Name { get; set; }    
                                                                  }
                                                                  """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithInstanceMethod = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : StaticValidator<User>
                                                                   {
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   
                                                                       private bool ValidateName(string? name)
                                                                       {
                                                                           return !string.IsNullOrEmpty(name);
                                                                       }
                                                                   }
                                                                   """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithStaticMethod = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : StaticValidator<User>
                                                                   {
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   
                                                                       private static bool ValidateName(string? name)
                                                                       {
                                                                           return !string.IsNullOrEmpty(name);
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC304_WhenStaticValidatorHasInstanceMethod()
    {
        AssertGenerator(inputs: [InputRequests, InputValidatorWithInstanceMethod], 
            outputs: [],
            diagnostics: ["Static validators cannot have instance methods. Method 'ValidateName' should be removed or made static."],
            assertTrackingSteps: false);
    }
    
    [Fact]
    public void ShouldNotReportVALC304_WhenStaticValidatorHasStaticMethod()
    {
        // Static methods are allowed - validation should pass
        AssertGenerator(inputs: [InputRequests, InputValidatorWithStaticMethod], 
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: false);
    }
}
