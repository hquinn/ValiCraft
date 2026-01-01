# ValiCraft

[![NuGet](https://img.shields.io/nuget/v/ValiCraft.svg)](https://www.nuget.org/packages/ValiCraft/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ValiCraft.svg)](https://www.nuget.org/packages/ValiCraft/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

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

This will also add a dependency to `MonadCraft`, a library containing the `Result<TError, TValue>` type, as well as `ErrorCraft`, a library containing the `IValidationError` and `ValidationError` types.

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
Result<IValidationErrors, User> result = validator.Validate(user);

string outcome = result
    .Match(
        success: user => $"${user.Username} is valid!",
        failure: errors => GetErrorString(errors));

// Or get errors directly
IReadOnlyList<IValidationError> errors = validator.ValidateToList(user);
```

## Performance

ValiCraft's source generator approach delivers exceptional performance compared to traditional validation libraries:

| Method                           | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| ValiCraft_SmallCollection        |  80.04 ns | 0.748 ns | 0.495 ns |  1.00 |    0.01 | 0.0688 |     432 B |        1.00 |
| FluentValidation_SmallCollection | 450.17 ns | 2.220 ns | 1.161 ns |  5.62 |    0.04 | 0.1006 |     632 B |        1.46 |
| ValiCraft_LargeCollection        |  82.86 ns | 2.506 ns | 1.491 ns |  1.04 |    0.02 | 0.0688 |     432 B |        1.00 |
| FluentValidation_LargeCollection | 417.04 ns | 5.280 ns | 3.493 ns |  5.21 |    0.05 | 0.1006 |     632 B |        1.46 |

| Method                        | Mean         | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |-------------:|-----------:|----------:|-------:|--------:|-------:|-------:|----------:|------------:|
| ValiCraft_ValidModel          |     17.99 ns |   0.091 ns |  0.060 ns |   1.00 |    0.00 |      - |      - |         - |          NA |
| FluentValidation_ValidModel   |  1,187.24 ns |   7.105 ns |  4.699 ns |  65.98 |    0.33 | 0.1354 |      - |     856 B |          NA |
| ValiCraft_InvalidModel        |    303.28 ns |   4.790 ns |  2.850 ns |  16.85 |    0.16 | 0.2599 | 0.0005 |    1632 B |          NA |
| FluentValidation_InvalidModel | 10,311.38 ns | 101.359 ns | 60.317 ns | 573.04 |    3.67 | 4.2267 | 0.1068 |   26552 B |          NA |

| Method                        | Mean         | Error       | StdDev      | Ratio  | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |-------------:|------------:|------------:|-------:|--------:|-------:|-------:|----------:|------------:|
| ValiCraft_ValidModel          |     7.244 ns |   0.0399 ns |   0.0238 ns |   1.00 |    0.00 |      - |      - |         - |          NA |
| FluentValidation_ValidModel   |   392.235 ns |   5.2912 ns |   3.1487 ns |  54.15 |    0.45 | 0.1054 |      - |     664 B |          NA |
| ValiCraft_InvalidModel        |   137.080 ns |   7.5040 ns |   3.9247 ns |  18.92 |    0.51 | 0.1159 |      - |     728 B |          NA |
| FluentValidation_InvalidModel | 2,666.771 ns | 215.4160 ns | 142.4844 ns | 368.16 |   18.80 | 1.0948 | 0.0076 |    6880 B |          NA |

| Method                                          | Mean          | Error      | StdDev     | Median        | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------------------------ |--------------:|-----------:|-----------:|--------------:|------:|--------:|-------:|-------:|----------:|------------:|
| ValiCraft_SimpleValidator_Instantiation         |     0.0000 ns |  0.0000 ns |  0.0000 ns |     0.0000 ns |     ? |       ? |      - |      - |         - |           ? |
| FluentValidation_SimpleValidator_Instantiation  | 1,909.0638 ns | 22.2632 ns | 13.2485 ns | 1,902.7726 ns |     ? |       ? | 1.1063 | 0.0076 |    6952 B |           ? |
| ValiCraft_ComplexValidator_Instantiation        |     0.0004 ns |  0.0010 ns |  0.0007 ns |     0.0000 ns |     ? |       ? |      - |      - |         - |           ? |
| FluentValidation_ComplexValidator_Instantiation | 7,605.7195 ns | 63.6780 ns | 42.1191 ns | 7,608.0119 ns |     ? |       ? | 4.0894 | 0.1221 |   25944 B |           ? |

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

#### Async Validation
ValiCraft supports fully asynchronous validators and rules.

```csharp
using ValiCraft;
using ValiCraft.Attributes;

[AsyncGenerateValidator]
public partial class AsyncUserValidator : AsyncValidator<User>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
    {
        builder.Ensure(x => x.Username)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3)
            // Custom async predicate
            .MustAsync(async (value, ct) => await IsUserNameAvailable(value, ct))
            .WithMessage("Username is already taken");

        builder.EnsureEach(x => x.Posts, post =>
            post.Ensure(p => p.Title).IsNotNullOrWhiteSpace());
    }
}

// Usage
var validator = new AsyncUserValidator();
IReadOnlyList<IValidationError> errors = await validator.ValidateToListAsync(user, cancellationToken);
```

Notes:
- Use `[AsyncGenerateValidator]` + `AsyncValidator<T>` + `IAsyncValidationRuleBuilder<T>` for async.
- Async counterparts are provided where applicable (e.g., `MustAsync(...)`, `ValidateToListAsync(...)`).
- You can mix sync and async rules inside async validators.

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

#### Error Severity
Set the severity level of validation errors:
```csharp
builder.Ensure(x => x.Email)
    .IsEmailAddress()
    .WithSeverity(ErrorSeverity.Error);      // Default severity

builder.Ensure(x => x.Nickname)
    .HasMaxLength(20)
    .WithSeverity(ErrorSeverity.Warning);    // Non-blocking warning

builder.Ensure(x => x.ProfilePicture)
    .IsNotNull()
    .WithSeverity(ErrorSeverity.Info);       // Informational message
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
Delegate to other validators for rich composition.

```csharp
// Single object
builder.Ensure(x => x.Address)
    .ValidateWith(new AddressValidator());

// For collections (per-item)
builder.EnsureEach(x => x.Orders, order =>
    order.ValidateWith(new OrderValidator()));
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

#### Custom Comparers
Pass custom comparers for equality/uniqueness checks.
```csharp
builder.Ensure(x => x.Items)
    .IsUniqueWithComparer(new MyItemComparer());

builder.Ensure(x => x.Code)
    .IsEqualWithComparer(otherCode, StringComparer.OrdinalIgnoreCase);
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

### Generator Diagnostics
ValiCraft emits clear diagnostics at compile time when rules are misused or ambiguous. Examples include:
- `VALC203`: issues around `Must/MustAsync` lambda forms
- `VALC204`: invalid rule chain usage for a target type

These diagnostics help you catch issues early with exact source locations and suggested fixes in messages.

## Requirements

- .NET 8.0 or higher
- C# 12 or higher

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.