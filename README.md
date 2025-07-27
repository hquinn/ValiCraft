# ValiCraft

A powerful, compile-time validation framework for .NET that crafts fast and boilerplate-free validation logic.

## Installation

```
dotnet add package ValiCraft
```

This will also add a dependency to `MonadCraft`, a library containing the `IError` and `Result<TError, TValue` types.

## Simple Example

Define your class you want to validate:

```csharp
public class User
{
    public required string Username { get; set; }
    public string? EmailAddress { get; set; }
    public int NumberOfFollowers { get; set; }
    public required List<Post> Posts { get; set; }
}

public class Post
{
    public required string Title { get; set; }
    public required string Message { get; set; }
}
```

Next, define your validator:

```csharp
using ValiCraft;
using ValiCraft.Attributes;
using ValiCraft.BuilderTypes;
using ValiCraft.Rules;

namespace Test;

[GenerateValidator] // Required for the source-generator to work
public partial class UserValidator : Validator<User>
{
    protected override void DefineRules(IValidationRuleBuilder<User> userBuilder)
    {
        userBuilder.Ensure(user => user.Username)
            .IsNotNullOrWhiteSpace();

        userBuilder.Ensure(user => user.EmailAddress)
            .IsNotNullOrWhiteSpace().WithErrorCode("EmailError").WithMessage("'{TargetName}' field must be a valid email.");

        userBuilder.Ensure(user => user.NumberOfFollowers, OnFailureMode.Halt)
            .IsGreaterOrEqualThan(0).WithMessage("'{TargetName}' must be positive. Attempted value {TargetValue}")
            .IsLessThan(1000000).WithMessage("Too many followers. Max {TargetName} is {ValueToCompare}");

        userBuilder.EnsureEach(user => user.Posts, postBuilder =>
        {
            postBuilder.Ensure(post => post.Title)
                .Must(title => title.Length > 5).WithMessage("{TargetName} is too short.");

            postBuilder.Ensure(post => post.Message)
               .IsNotNullOrEmpty().If(post => !string.IsNullOrEmpty(post.Title));
        });
    }
}
```

This will produce efficient validation logic which can be used to validate your class.

You can validate using two different methods, either using the Result type or a list of Validation errors.

```csharp
// Using Result type
IValidator<User> userValidator = new UserValidator();
Result<IReadonlyList<IValidationError>, User> validationResult = userValidator.Validate(user);
```

```csharp
// Using list of Validation errors
IValidator<User> userValidator = new UserValidator();
IReadOnlyList<IValidationError> validationErrorList = userValidator.ValidateToList(user);
```