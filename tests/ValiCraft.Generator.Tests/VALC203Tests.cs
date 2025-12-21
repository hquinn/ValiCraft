using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC203Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
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
                                                          using ValiCraft.AsyncBuilderTypes;
                                                          
                                                          namespace Test.Rules;
                                                          
                                                          [GenerateRuleExtension("IsNotNull")]
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
                                                          
                                                          [DefaultMessage("'{TargetName}' must not be empty.")]
                                                          public static class NotEmptyRuleExtensions
                                                          {
                                                              [MapToValidationRule(typeof(NotEmptyRule), "")]
                                                              public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotEmpty<TRequest, TTargetType>(
                                                                  this IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                                                                  => throw new NotImplementedException("Never gets called");

                                                              [MapToValidationRule(typeof(NotEmptyRule), "")]
                                                              public static IAsyncValidationRuleBuilderType<TRequest, TTargetType> IsNotEmpty<TRequest, TTargetType>(
                                                                  this IAsyncBuilderType<TRequest, TTargetType> builder) where TRequest : class
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
                                                                           // Put IsNotNull (which is a weak semantic rule) before IsNotEmpty (which is a rich semantic rule)
                                                                           builder.Ensure(x => x.Name)
                                                                               .IsNotNull()
                                                                               .IsNotEmpty();
                                                                       }
                                                                   }
                                                                   """;
    
    [StringSyntax("CSharp")] private const string ExpectedNotNullExtensions = """
                                                                              // <auto-generated />
                                                                              #nullable enable
                                                                              
                                                                              namespace Test.Rules
                                                                              {
                                                                                  /// <summary>
                                                                                  /// Extension methods for the <see cref="Test.Rules.NotNullRule{T}"/> validation rule.
                                                                                  /// </summary>
                                                                                  [global::ValiCraft.Attributes.DefaultMessage("'{TargetName}' must not be null.")]
                                                                                  public static class NotNullRuleExtensions
                                                                                  {
                                                                                      /// <summary>
                                                                                      /// Adds the IsNotNull validation rule to the builder.
                                                                                      /// </summary>
                                                                                      /// <remarks>
                                                                                      /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
                                                                                      /// </remarks>
                                                                                      [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.NotNullRule<>), "<{0}>")]
                                                                                      public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TTargetType> IsNotNull<TRequest, TTargetType>(
                                                                                          this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                                                                                          => throw new global::System.NotImplementedException("Never gets called");

                                                                                      /// <summary>
                                                                                      /// Adds the IsNotNull validation rule to the async builder.
                                                                                      /// </summary>
                                                                                      /// <remarks>
                                                                                      /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
                                                                                      /// </remarks>
                                                                                      [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.NotNullRule<>), "<{0}>")]
                                                                                      public static global::ValiCraft.AsyncBuilderTypes.IAsyncValidationRuleBuilderType<TRequest, TTargetType> IsNotNull<TRequest, TTargetType>(
                                                                                          this global::ValiCraft.AsyncBuilderTypes.IAsyncBuilderType<TRequest, TTargetType> builder) where TRequest : class
                                                                                          => throw new global::System.NotImplementedException("Never gets called");
                                                                                  }
                                                                              }
                                                                              """;
    
    [Fact]
    public void ShouldReportVALC203()
    {
        AssertGenerator(inputs: [InputRequests, InputValidationRules, InputValidator], 
            outputs: [ExpectedNotNullExtensions],
            diagnostics: ["Rule cannot be mapped to a validation rule. Try moving the rule out of the invocation chain."],
            assertTrackingSteps: false);
    }
}