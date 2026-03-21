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
    
    [StringSyntax("CSharp")] private const string ExpectedValidators = """
                                                                   // <auto-generated />
                                                                   #nullable enable

                                                                   using Test.Requests;
                                                                   using ValiCraft;
                                                                   using ValiCraft.Attributes;
                                                                   using ValiCraft.BuilderTypes;

                                                                   namespace Test.Validators
                                                                   {
                                                                       /// <summary>
                                                                       /// Generated static validator for <see cref="global::Test.Requests.User"/>.
                                                                       /// </summary>
                                                                       public partial class UserValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.User>
                                                                       {
                                                                           /// <inheritdoc />
                                                                           public static global::ValiCraft.ValidationErrors? Validate(global::Test.Requests.User request)
                                                                           {
                                                                               var errors = RunValidationLogic(request, null);

                                                                               if (errors is null) return null;

                                                                               return new global::ValiCraft.ValidationErrors
                                                                               {
                                                                                   Code = "UserErrors",
                                                                                   Message = "One or more validation errors occurred.",
                                                                                   Severity = global::ValiCraft.ErrorSeverity.Error,
                                                                                   Errors = errors
                                                                               };
                                                                           }

                                                                           /// <inheritdoc />
                                                                           [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                           public static global::System.Collections.Generic.List<global::ValiCraft.ValidationError>? RunValidation(global::Test.Requests.User request, string? inheritedTargetPath)
                                                                           {
                                                                               return RunValidationLogic(request, inheritedTargetPath);
                                                                           }

                                                                           private static global::System.Collections.Generic.List<global::ValiCraft.ValidationError>? RunValidationLogic(global::Test.Requests.User request, string? inheritedTargetPath)
                                                                           {
                                                                               global::System.Collections.Generic.List<global::ValiCraft.ValidationError>? errors = null;

                                                                               if (!global::ValiCraft.Rules.NotNullOrEmpty(request.Name))
                                                                               {
                                                                                   errors ??= new(1);
                                                                                   errors.Add(new global::ValiCraft.ValidationError
                                                                                   {
                                                                                       Code = nameof(global::ValiCraft.Rules.NotNullOrEmpty),
                                                                                       Message = $"Name must not be null or empty.",
                                                                                       Severity = global::ValiCraft.ErrorSeverity.Error,
                                                                                       TargetName = "Name",
                                                                                       TargetPath = $"{inheritedTargetPath}Name",
                                                                                       AttemptedValue = request.Name,
                                                                                   });
                                                                               }

                                                                               return errors;
                                                                           }
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
            outputs: [ExpectedValidators],
            diagnostics: [],
            assertTrackingSteps: false);
    }
}
