using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC301Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class User
                                                                  {
                                                                      public string? Name { get; set; }    
                                                                  }
                                                                  """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithConstructor = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : StaticValidator<User>
                                                                   {
                                                                       private readonly string _prefix;
                                                                   
                                                                       public UserValidator(string prefix)
                                                                       {
                                                                           _prefix = prefix;
                                                                       }
                                                                   
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithPrimaryConstructor = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator(string prefix) : StaticValidator<User>
                                                                   {
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC301_WhenStaticValidatorHasParameterizedConstructor()
    {
        AssertGenerator(inputs: [InputRequests, InputValidatorWithConstructor], 
            outputs: [],
            diagnostics: [
                "Static validators cannot have parameterized constructors. Static validators are stateless and cannot use dependency injection.",
                "Static validators cannot have instance fields. Field '_prefix' should be removed or made static."
            ],
            assertTrackingSteps: false);
    }
    
    [Fact]
    public void ShouldReportVALC301_WhenStaticValidatorHasPrimaryConstructor()
    {
        AssertGenerator(inputs: [InputRequests, InputValidatorWithPrimaryConstructor], 
            outputs: [],
            diagnostics: ["Static validators cannot have parameterized constructors. Static validators are stateless and cannot use dependency injection."],
            assertTrackingSteps: false);
    }
}
