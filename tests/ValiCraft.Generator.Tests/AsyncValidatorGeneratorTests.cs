using System.Diagnostics.CodeAnalysis;
using MonadCraft;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

/// <summary>
/// Tests for async validator generation using [GenerateAsyncValidator] attribute.
/// </summary>
public class AsyncValidatorGeneratorTests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  using System;
                                                                  using System.Collections.Generic;
                                                                  using System.Threading;
                                                                  using System.Threading.Tasks;
                                                                  
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class User
                                                                  {
                                                                      public string? Email { get; set; }
                                                                      public string? Name { get; set; }
                                                                      public int Age { get; set; }
                                                                  }
                                                                  """;

    #region Must (Sync predicate in async validator)

    /// <summary>
    /// Tests that a basic async validator with sync Must generates correctly.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithSyncMust()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("Email is required");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests Must with a method reference instead of a lambda.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMustMethodReference()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(IsValidEmail)
                                                  .WithMessage("Email is invalid");
                                          }
                                          
                                          private static bool IsValidEmail(string? email) 
                                              => !string.IsNullOrEmpty(email) && email.Contains("@");
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests Must with a block lambda body.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMustBlockLambda()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Age)
                                                  .Must(age => 
                                                  {
                                                      return age >= 18 && age <= 120;
                                                  })
                                                  .WithMessage("Age must be between 18 and 120");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests Must with null check expression (not pattern matching).
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithNullCheck()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => email != null)
                                                  .WithMessage("Email cannot be null");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region Multiple Rules

    /// <summary>
    /// Tests chaining multiple sync Must rules.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithChainedMustRules()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("Email is required")
                                                  .Must(email => email!.Contains("@"))
                                                  .WithMessage("Email must contain @");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests validation rules on multiple properties.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMultipleProperties()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("Email is required");
                                                  
                                              builder.Ensure(x => x.Name)
                                                  .Must(name => !string.IsNullOrEmpty(name))
                                                  .WithMessage("Name is required");
                                                  
                                              builder.Ensure(x => x.Age)
                                                  .Must(age => age >= 18)
                                                  .WithMessage("Must be 18 or older");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region WithMessage, WithErrorCode, WithTargetName

    /// <summary>
    /// Tests WithMessage configuration with placeholders.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithCustomMessage()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("'{TargetName}' cannot be empty, but got '{TargetValue}'");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests WithErrorCode configuration.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithCustomErrorCode()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithErrorCode("EMAIL_REQUIRED");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests WithTargetName configuration.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithCustomTargetName()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithTargetName("Email Address")
                                                  .WithMessage("'{TargetName}' is required");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests combining message, target name, and error code.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithAllConfigOptions()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("'{TargetName}' is required")
                                                  .WithTargetName("Email Address")
                                                  .WithErrorCode("REQUIRED");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region Conditional Validation (If on ValidationRuleBuilderType)

    /// <summary>
    /// Tests conditional validation with If on validation rule builder type.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithIfConditionOnValidationRuleBuilder()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => email!.Contains("@"))
                                                  .If(user => !string.IsNullOrEmpty(user.Email));
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region WhenNotNull

    /// <summary>
    /// Tests WhenNotNull for nullable properties.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithWhenNotNull()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .WhenNotNull()
                                                  .Must(email => email!.Contains("@"))
                                                  .WithMessage("Email must contain @");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region Constructor and Dependencies

    /// <summary>
    /// Tests async validator with constructor dependencies.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithConstructorDependencies()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      public interface IEmailService
                                      {
                                          Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct);
                                      }
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          private readonly IEmailService _emailService;
                                          
                                          public UserValidator(IEmailService emailService)
                                          {
                                              _emailService = emailService;
                                          }
                                          
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("Email is required");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region Empty Validator

    /// <summary>
    /// Tests an async validator with no rules defined.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithNoRules()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              // No rules
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region Complex Scenarios (Sync-only)

    /// <summary>
    /// Tests a complex async validator with multiple rules and configurations using only sync Must.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_ComplexScenarioSyncOnly()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              // Required email with format validation
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("Email is required")
                                                  .WithErrorCode("EMAIL_REQUIRED")
                                                  .Must(email => email!.Contains("@"))
                                                  .WithMessage("Email must be valid")
                                                  .WithErrorCode("EMAIL_INVALID");
                                                  
                                              // Optional name with length validation
                                              builder.Ensure(x => x.Name)
                                                  .WhenNotNull()
                                                  .Must(name => name!.Length >= 2)
                                                  .WithMessage("Name must be at least 2 characters")
                                                  .Must(name => name!.Length <= 100)
                                                  .WithMessage("Name must not exceed 100 characters");
                                                  
                                              // Age validation
                                              builder.Ensure(x => x.Age)
                                                  .Must(age => age >= 0)
                                                  .WithMessage("Age cannot be negative")
                                                  .Must(age => age <= 150)
                                                  .WithMessage("Age must be realistic");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region Different Expression Types in Must

    /// <summary>
    /// Tests Must with a simple binary expression.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithBinaryExpression()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Age)
                                                  .Must(age => age > 0 && age < 150)
                                                  .WithMessage("Age must be between 0 and 150");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests Must with string methods.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithStringMethods()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Name)
                                                  .Must(name => !string.IsNullOrWhiteSpace(name))
                                                  .WithMessage("Name cannot be empty or whitespace");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests Must with null coalescing and conditional access.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithNullConditional()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => email?.Length > 5)
                                                  .WithMessage("Email must be longer than 5 characters");
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion

    #region MustAsync (Async predicate validation)

    /// <summary>
    /// Tests MustAsync with an await expression in the lambda body.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMustAsyncAwait()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct))
                                                  .WithMessage("Email must be unique");
                                          }
                                          
                                          private static Task<bool> IsEmailUniqueAsync(string? email, CancellationToken ct)
                                              => Task.FromResult(!string.IsNullOrEmpty(email));
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests MustAsync returning Task directly (without await in lambda).
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMustAsyncDirectTask()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .MustAsync((email, ct) => IsEmailUniqueAsync(email, ct))
                                                  .WithMessage("Email must be unique");
                                          }
                                          
                                          private static Task<bool> IsEmailUniqueAsync(string? email, CancellationToken ct)
                                              => Task.FromResult(!string.IsNullOrEmpty(email));
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests combining MustAsync and sync Must in the same validator.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMixedSyncAndAsyncRules()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              // Sync rule first
                                              builder.Ensure(x => x.Email)
                                                  .Must(email => !string.IsNullOrEmpty(email))
                                                  .WithMessage("Email is required");
                                              
                                              // Then async rule
                                              builder.Ensure(x => x.Email)
                                                  .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct))
                                                  .WithMessage("Email must be unique");
                                          }
                                          
                                          private static Task<bool> IsEmailUniqueAsync(string? email, CancellationToken ct)
                                              => Task.FromResult(email != "taken@example.com");
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests MustAsync with custom WithMessage and WithErrorCode.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMustAsyncCustomMessages()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct))
                                                  .WithMessage("The email '{TargetValue}' is already in use")
                                                  .WithErrorCode("UNIQUE_EMAIL");
                                          }
                                          
                                          private static Task<bool> IsEmailUniqueAsync(string? email, CancellationToken ct)
                                              => Task.FromResult(email != "taken@example.com");
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests chaining multiple MustAsync rules.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithChainedMustAsyncRules()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .MustAsync(async (email, ct) => await IsEmailValidAsync(email, ct))
                                                  .WithMessage("Email format is invalid")
                                                  .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct))
                                                  .WithMessage("Email must be unique");
                                          }
                                          
                                          private static Task<bool> IsEmailValidAsync(string? email, CancellationToken ct)
                                              => Task.FromResult(!string.IsNullOrEmpty(email) && email.Contains("@"));
                                          
                                          private static Task<bool> IsEmailUniqueAsync(string? email, CancellationToken ct)
                                              => Task.FromResult(email != "taken@example.com");
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    /// <summary>
    /// Tests MustAsync on multiple properties.
    /// </summary>
    [Fact]
    public void ShouldGenerateAsyncValidator_WithMustAsyncOnMultipleProperties()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Linq;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : AsyncValidator<User>
                                      {
                                          protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
                                          {
                                              builder.Ensure(x => x.Email)
                                                  .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct))
                                                  .WithMessage("Email must be unique");
                                              
                                              builder.Ensure(x => x.Name)
                                                  .MustAsync(async (name, ct) => await IsNameValidAsync(name, ct))
                                                  .WithMessage("Name contains invalid characters");
                                          }
                                          
                                          private static Task<bool> IsEmailUniqueAsync(string? email, CancellationToken ct)
                                              => Task.FromResult(!string.IsNullOrEmpty(email));
                                              
                                          private static Task<bool> IsNameValidAsync(string? name, CancellationToken ct)
                                              => Task.FromResult(!string.IsNullOrEmpty(name) && name.All(char.IsLetter));
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: null,
            diagnostics: [],
            assertTrackingSteps: true);
    }

    #endregion
}
