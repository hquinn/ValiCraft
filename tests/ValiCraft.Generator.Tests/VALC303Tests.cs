using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC303Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class User
                                                                  {
                                                                      public string? Name { get; set; }    
                                                                  }
                                                                  """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithInstanceProperty = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : StaticValidator<User>
                                                                   {
                                                                       public string Prefix { get; set; } = "test";
                                                                   
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithStaticProperty = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : StaticValidator<User>
                                                                   {
                                                                       public static string Prefix { get; set; } = "test";
                                                                   
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC303_WhenStaticValidatorHasInstanceProperty()
    {
        AssertGenerator(inputs: [InputRequests, InputValidatorWithInstanceProperty], 
            outputs: [],
            diagnostics: ["Static validators cannot have instance properties. Property 'Prefix' should be removed or made static."],
            assertTrackingSteps: false);
    }
    
    [Fact]
    public void ShouldNotReportVALC303_WhenStaticValidatorHasStaticProperty()
    {
        // Static properties are allowed - validation should pass
        AssertGenerator(inputs: [InputRequests, InputValidatorWithStaticProperty], 
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: false);
    }
}
