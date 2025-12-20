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
    
    [StringSyntax("CSharp")] private const string InputValidationRules = """
                                                          using System;
                                                          using ValiCraft;
                                                          using ValiCraft.Attributes;
                                                          using ValiCraft.BuilderTypes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          [DefaultMessage("'{TargetName}' must not be null.")]
                                                          public class NotNullRule<T> : IValidationRule<T?>
                                                          {
                                                              public static bool IsValid(T? value) => value is not null;
                                                          }
                                                          
                                                          [DefaultMessage("'{TargetName}' must not be empty.")]
                                                          public class NotEmptyRule: IValidationRule<string?>
                                                          {
                                                              public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
                                                          }
                                                          
                                                          [DefaultMessage("'{TargetName}' must not be null.")]
                                                          public static class NotNullRuleExtensions
                                                          {
                                                              [MapToValidationRule(typeof(NotNullRule<>), "<{0}>")]
                                                              public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotNull<TRequest, TTargetType>(
                                                                  this IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                                                                  => throw new NotImplementedException("Never gets called");
                                                          }
                                                          
                                                          [DefaultMessage("'{TargetName}' must not be empty.")]
                                                          public static class NotEmptyRuleExtensions
                                                          {
                                                              [MapToValidationRule(typeof(NotEmptyRule), "")]
                                                              public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotEmpty<TRequest, TTargetType>(
                                                                  this IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                                                                  => throw new NotImplementedException("Never gets called");
                                                          }
                                                          """;
    
    [StringSyntax("CSharp")] private const string InputValidator = """
                                                                   using Test.Rules;
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
                                                                                   .IsNotNull()
                                                                                   .IsNotEmpty();
                                                                           });
                                                                       }
                                                                   }
                                                                   """;
    
    [Fact]
    public void ShouldReportVALC206()
    {
        AssertGenerator(inputs: [InputRequests, InputValidationRules, InputValidator], 
            outputs: [],
            diagnostics: ["'builder' cannot be used in this scope. Try using 'userBuilder'."],
            assertTrackingSteps: false);
    }
}