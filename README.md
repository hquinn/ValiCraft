# ValiCraft

A powerful, compile-time validation framework for .NET that crafts fast and boilerplate-free validation logic using source generators.

## Why ValiCraft?

ValiCraft uses C# source generators to transform your validation rules into highly optimized, allocation-free code at compile time. This approach delivers:

- **🚀 Extreme Performance**: ~5-6ns per validation with minimal allocations
- **✨ Clean, Fluent API**: Expressive and intuitive syntax for defining validation rules
- **🔧 Compile-Time Safety**: Validation logic is generated and checked at build time
- **📦 Zero Runtime Overhead**: No reflection, no runtime code generation
- **🎯 Type-Safe**: Full IntelliSense support and compile-time type checking

## Installation

```bash
dotnet add package ValiCraft
```

This will also add a dependency to `MonadCraft`, a library containing the `IError` and `Result<TError, TValue>` types.

## Quick Start

### 1. Define Your Model

```csharp
public class User
{
    public required string Username { get; set; }
    public string? EmailAddress { get; set; }
    public int Age { get; set; }
    public required List<Post> Posts { get; set; }
}

public class Post
{
    public required string Title { get; set; }
    public required string Message { get; set; }
}
```

### 2. Create a Validator

```csharp
using ValiCraft;
using ValiCraft.Attributes;
using ValiCraft.Rules;

[GenerateValidator] // Source generator will create the validation logic
public partial class UserValidator : Validator<User>
{
    protected override void DefineRules(IValidationRuleBuilder<User> builder)
    {
        builder.Ensure(x => x.Username)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3)
            .HasMaxLength(50);

        builder.Ensure(x => x.EmailAddress)
            .IsNotNullOrWhiteSpace()
            .IsEmailAddress()
            .WithErrorCode("INVALID_EMAIL")
            .WithMessage("'{TargetName}' must be a valid email address");

        builder.Ensure(x => x.Age, OnFailureMode.Halt)
            .IsGreaterOrEqualThan(18)
            .IsLessThan(120);

        builder.EnsureEach(x => x.Posts, postBuilder =>
        {
            postBuilder.Ensure(post => post.Title)
                .IsNotNullOrWhiteSpace()
                .HasMinLength(5);

            postBuilder.Ensure(post => post.Message)
                .IsNotNullOrEmpty()
                .If(post => !string.IsNullOrEmpty(post.Title));
        });
    }
}
```

### 3. Validate Your Objects

```csharp
var validator = new UserValidator();

// Using Result type (recommended)
Result<IReadOnlyList<IValidationError>, User> result = validator.Validate(user);

string outcome = result
    .Match(
        success: user => $"${user.Username} is valid!",
        failure: errors => GetErrorString(errors));

// Or get errors directly
IReadOnlyList<IValidationError> errors = validator.ValidateToList(user);
```

## Performance

ValiCraft's source generator approach delivers exceptional performance compared to traditional validation libraries:

| Method                              | Mean     | Allocated |
|-------------------------------------|----------|-----------|
| ValiCraft (Valid Model)             | ~6 ns    | 0 B       |
| FluentValidation (Valid Model)      | ~200 ns  | 120 B     |
| ValiCraft (Invalid Model)           | ~15 ns   | 32 B      |
| FluentValidation (Invalid Model)    | ~450 ns  | 600 B     |
| ValiCraft (Validator Instantiation) | ~1 ns    | 0 B       |
| FluentValidation (Instantiation)    | ~800 ns  | 400 B     |

**Note**: Benchmarks run on Apple M1 Pro, .NET 9.0. Your results may vary.

## Features

### Built-in Validation Rules

ValiCraft includes 50+ built-in validation rules:

**String Validation**
- `IsNotNull()`, `IsNotNullOrEmpty()`, `IsNotNullOrWhiteSpace()`
- `HasMinLength()`, `HasMaxLength()`, `HasLength()`, `HasLengthBetween()`
- `IsEmailAddress()`, `IsUrl()`, `IsAlphaNumeric()`
- `StartsWith()`, `EndsWith()`, `Contains()`, `Matches()` (regex)

**Numeric Validation**
- `IsGreaterThan()`, `IsGreaterOrEqualThan()`, `IsLessThan()`, `IsLessOrEqualThan()`
- `IsBetween()`, `IsBetweenExclusive()`
- `IsPositive()`, `IsPositiveOrZero()`, `IsNegative()`, `IsNegativeOrZero()`

**Collection Validation**
- `HasMinCount()`, `HasMaxCount()`, `HasCount()`, `HasCountBetween()`
- `IsEmpty()`, `HasItems()`
- `IsUnique()`, `IsUniqueWithComparer()`
- `CollectionContains()`, `CollectionNotContains()`

**Date/Time Validation**
- `IsInFuture()`, `IsInFutureOrPresent()`, `IsInPast()`, `IsInPastOrPresent()`
- `IsDateBetween()`, `HasMinAge()`, `HasMaxAge()`

**Comparison Validation**
- `IsEqual()`, `IsNotEqual()`, `IsEqualWithComparer()`, `IsNotEqualWithComparer()`
- `IsIn()`, `IsNotIn()`
- `IsNotDefault()`

**Custom Validation**
- `Must()` - Define custom predicates with lambda expressions

### Advanced Features

#### Conditional Validation
```csharp
builder.Ensure(x => x.PhoneNumber)
    .IsNotNullOrWhiteSpace()
    .If(x => x.ContactPreference == "phone");
```

#### Custom Error Messages with Placeholders
```csharp
builder.Ensure(x => x.Age)
    .IsGreaterOrEqualThan(18)
    .WithMessage("'{TargetName}' must be at least {ValueToCompare}, but was {TargetValue}");
```

Available placeholders:
- `{TargetName}` - Property name (humanized: "firstName" → "First Name")
- `{TargetValue}` - Actual value being validated
- `{ValueToCompare}` - Expected value (for comparison rules)
- Custom placeholders defined by specific rules

#### Error Codes
```csharp
builder.Ensure(x => x.Email)
    .IsEmailAddress()
    .WithErrorCode("EMAIL_INVALID");
```

#### Failure Modes
```csharp
// Stop validating this property on first failure
builder.Ensure(x => x.Age, OnFailureMode.Halt)
    .IsGreaterThan(0)
    .IsLessThan(150);

// Continue validating all rules (default)
builder.Ensure(x => x.Name, OnFailureMode.Continue)
    .IsNotNullOrWhiteSpace()
    .HasMinLength(2);
```

#### Collection Validation
```csharp
// Validate each item in a collection
builder.EnsureEach(x => x.Tags, tagBuilder =>
{
    tagBuilder.Ensure(tag => tag.Name)
        .IsNotNullOrWhiteSpace()
        .HasMaxLength(50);
});
```

#### Nested Object Validation
```csharp
// Validate with another validator
builder.Ensure(x => x.Address)
    .ValidateWith(new AddressValidator());
```

#### WhenNotNull - Optional Property Validation
```csharp
// Only validate if the value is not null
builder.Ensure(x => x.OptionalEmail)
    .WhenNotNull()
    .IsEmailAddress()
    .HasMaxLength(255);
```

#### Either - OR-Based Validation
```csharp
// At least one of these validation groups must pass
builder.Either(
    b => b.Ensure(x => x.Email).IsNotNullOrEmpty(),
    b => b.Ensure(x => x.Phone).IsNotNullOrEmpty()
);
```

#### Date/Time Validation with Testability
```csharp
// Use parameterized date checks for testable validation
builder.Ensure(x => x.ExpiryDate)
    .IsAfter(DateTime.UtcNow);

// Or use reference dates for testing
var referenceDate = new DateTime(2024, 1, 1);
builder.Ensure(x => x.StartDate)
    .IsAfter(referenceDate);
```

#### Regex Validation
```csharp
// Using string pattern
builder.Ensure(x => x.PostalCode)
    .Matches(@"^\d{5}(-\d{4})?$");

// Using pre-compiled Regex for better performance
private static readonly Regex PostalCodeRegex = new(@"^\d{5}(-\d{4})?$", RegexOptions.Compiled);

builder.Ensure(x => x.PostalCode)
    .MatchesRegex(PostalCodeRegex);
```

#### Convenient IsIn/IsNotIn with Multiple Values
```csharp
// Check if value is one of allowed values
builder.Ensure(x => x.Status)
    .IsInValues("Active", "Pending", "Closed");

// Check if value is not one of forbidden values
builder.Ensure(x => x.Role)
    .IsNotInValues("Admin", "SuperUser");
```

## Creating Custom Validation Rules

You can create your own reusable validation rules:

```csharp
using ValiCraft;
using ValiCraft.Attributes;

[GenerateRuleExtension("IsValidPostalCode")]
[DefaultMessage("'{TargetName}' must be a valid postal code")]
public class PostalCodeRule : IValidationRule<string?>
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return Regex.IsMatch(value, @"^\d{5}(-\d{4})?$");
    }
}
```

Use it in your validators:

```csharp
builder.Ensure(x => x.PostalCode)
    .IsValidPostalCode();
```

## How It Works

ValiCraft uses C# Source Generators to analyze your validator definitions at compile time and generate optimized validation code. This means:

1. **Zero Runtime Overhead**: No reflection or runtime code generation
2. **Full IntelliSense**: All generated code is available in your IDE
3. **Debuggable**: Step through the generated validation logic
4. **Type-Safe**: Compilation errors if validation rules don't match your types

The generated code is pure C#, highly optimized, and produces minimal allocations.

## Requirements

- .NET 8.0 or higher
- C# 12 or higher

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.