using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC302Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class User
                                                                  {
                                                                      public string? Name { get; set; }    
                                                                  }
                                                                  """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithInstanceField = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : StaticValidator<User>
                                                                   {
                                                                       private readonly string _prefix = "test";
                                                                   
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [StringSyntax("CSharp")] private const string InputValidatorWithStaticField = """
                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;
                                                                   
                                                                   namespace Test.Validators;
                                                                   
                                                                   [GenerateValidator]
                                                                   public partial class UserValidator : StaticValidator<User>
                                                                   {
                                                                       private static readonly string Prefix = "test";
                                                                   
                                                                       protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                                                       {
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNullOrEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC302_WhenStaticValidatorHasInstanceField()
    {
        AssertGenerator(inputs: [InputRequests, InputValidatorWithInstanceField], 
            outputs: [],
            diagnostics: ["Static validators cannot have instance fields. Field '_prefix' should be removed or made static."],
            assertTrackingSteps: false);
    }
    
    [Fact]
    public void ShouldNotReportVALC302_WhenStaticValidatorHasStaticField()
    {
        // Static fields are allowed - validation should pass
        AssertGenerator(inputs: [InputRequests, InputValidatorWithStaticField], 
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: false);
    }
}
