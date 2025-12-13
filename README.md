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

This will also add a dependency to `MonadCraft`, a library containing the Result<TError, TValue>` type.

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

| Method                           | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ValiCraft_SmallCollection        |  25.07 ns |  0.852 ns |  0.564 ns |  1.00 |    0.03 | 0.0204 |     128 B |        1.00 |
| FluentValidation_SmallCollection | 428.81 ns | 42.833 ns | 25.489 ns | 17.11 |    1.03 | 0.1006 |     632 B |        4.94 |
| ValiCraft_LargeCollection        |  25.46 ns |  1.263 ns |  0.835 ns |  1.02 |    0.04 | 0.0204 |     128 B |        1.00 |
| FluentValidation_LargeCollection | 422.70 ns |  5.528 ns |  3.289 ns | 16.87 |    0.37 | 0.1006 |     632 B |        4.94 |

| Method                        | Mean         | Error     | StdDev    | Ratio  | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |-------------:|----------:|----------:|-------:|--------:|-------:|-------:|----------:|------------:|
| ValiCraft_ValidModel          |     16.36 ns |  0.105 ns |  0.055 ns |   1.00 |    0.00 |      - |      - |         - |          NA |
| FluentValidation_ValidModel   |  1,189.68 ns |  7.684 ns |  4.573 ns |  72.73 |    0.35 | 0.1354 |      - |     856 B |          NA |
| ValiCraft_InvalidModel        |    282.32 ns |  4.598 ns |  2.736 ns |  17.26 |    0.17 | 0.1988 |      - |    1248 B |          NA |
| FluentValidation_InvalidModel | 10,219.38 ns | 45.458 ns | 23.775 ns | 624.73 |    2.41 | 4.2267 | 0.1068 |   26552 B |          NA |

| Method                        | Mean         | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |-------------:|-----------:|----------:|-------:|--------:|-------:|-------:|----------:|------------:|
| ValiCraft_ValidModel          |     5.975 ns |  0.3761 ns | 0.2488 ns |   1.00 |    0.06 |      - |      - |         - |          NA |
| FluentValidation_ValidModel   |   382.958 ns |  1.9151 ns | 1.0017 ns |  64.19 |    2.47 | 0.1054 |      - |     664 B |          NA |
| ValiCraft_InvalidModel        |    74.609 ns |  0.5920 ns | 0.3916 ns |  12.50 |    0.48 | 0.0650 |      - |     408 B |          NA |
| FluentValidation_InvalidModel | 2,527.574 ns | 11.1362 ns | 5.8244 ns | 423.63 |   16.29 | 1.0948 | 0.0076 |    6880 B |          NA |

| Method                                          | Mean          | Error       | StdDev      | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------------------------ |--------------:|------------:|------------:|------:|--------:|-------:|-------:|----------:|------------:|
| ValiCraft_SimpleValidator_Instantiation         |     0.0000 ns |   0.0000 ns |   0.0000 ns |     ? |       ? |      - |      - |         - |           ? |
| FluentValidation_SimpleValidator_Instantiation  | 1,823.0625 ns |  44.1707 ns |  23.1021 ns |     ? |       ? | 1.1063 | 0.0076 |    6952 B |           ? |
| ValiCraft_ComplexValidator_Instantiation        |     0.0000 ns |   0.0000 ns |   0.0000 ns |     ? |       ? |      - |      - |         - |           ? |
| FluentValidation_ComplexValidator_Instantiation | 8,001.6715 ns | 786.1477 ns | 411.1705 ns |     ? |       ? | 4.0894 | 0.1221 |   26024 B |           ? |

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

## Async Validators

ValiCraft supports asynchronous validation for scenarios that require I/O operations like database lookups or API calls.

### Creating an Async Validator

```csharp
using ValiCraft;
using ValiCraft.Attributes;

[GenerateAsyncValidator]
public partial class UserValidator : AsyncValidator<User>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
    {
        // Sync rules work in async validators
        builder.Ensure(x => x.Email)
            .IsNotNullOrEmpty()
            .IsEmailAddress();

        // Async validation with MustAsync
        builder.Ensure(x => x.Email)
            .MustAsync(async (email, ct) => await IsEmailUniqueAsync(email, ct))
            .WithMessage("Email is already in use");

        builder.Ensure(x => x.Username)
            .MustAsync(async (username, ct) => await IsUsernameAvailableAsync(username, ct))
            .WithMessage("Username is not available")
            .WithErrorCode("USERNAME_TAKEN");
    }

    private async Task<bool> IsEmailUniqueAsync(string? email, CancellationToken ct)
    {
        // Check against database, external API, etc.
        return await _dbContext.Users.AllAsync(u => u.Email != email, ct);
    }

    private async Task<bool> IsUsernameAvailableAsync(string? username, CancellationToken ct)
    {
        return await _userService.IsUsernameAvailableAsync(username, ct);
    }
}
```

### Using Async Validators

```csharp
var validator = new UserValidator();

// Async validation
Result<IReadOnlyList<IValidationError>, User> result = 
    await validator.ValidateAsync(user, cancellationToken);

// Or get errors directly
IReadOnlyList<IValidationError> errors = 
    await validator.ValidateToListAsync(user, cancellationToken);
```

### MustAsync Features

`MustAsync` supports all the same features as sync `Must`:

```csharp
// With custom messages and error codes
builder.Ensure(x => x.Email)
    .MustAsync(async (email, ct) => await CheckEmailAsync(email, ct))
    .WithMessage("The email '{TargetValue}' is already registered")
    .WithErrorCode("DUPLICATE_EMAIL");

// Chaining multiple async rules
builder.Ensure(x => x.Email)
    .MustAsync(async (email, ct) => await IsValidDomainAsync(email, ct))
    .WithMessage("Email domain is not allowed")
    .MustAsync(async (email, ct) => await IsNotBlacklistedAsync(email, ct))
    .WithMessage("Email is blacklisted");

// Mix sync and async rules
builder.Ensure(x => x.Email)
    .IsNotNullOrEmpty()                    // Sync rule
    .IsEmailAddress()                       // Sync rule  
    .MustAsync(async (e, ct) => await IsUniqueAsync(e, ct))  // Async rule
    .WithMessage("Email must be unique");
```

### Creating Reusable Async Validation Rules

For frequently used async validations, create reusable rules:

```csharp
using ValiCraft;
using ValiCraft.Attributes;

[GenerateAsyncRuleExtension("IsUniqueEmail")]
[DefaultMessage("'{TargetName}' is already in use")]
[DefaultErrorCode("DUPLICATE_EMAIL")]
public class UniqueEmailRule : IAsyncValidationRule<string?>
{
    public static async Task<bool> IsValidAsync(string? email, CancellationToken ct)
    {
        // Your async validation logic
        return await EmailService.IsUniqueAsync(email, ct);
    }
}
```

Use it in your async validators:

```csharp
builder.Ensure(x => x.Email)
    .IsUniqueEmail();  // Generated extension method
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