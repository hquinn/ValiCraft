# ValiCraft

[![NuGet](https://img.shields.io/nuget/v/ValiCraft.svg)](https://www.nuget.org/packages/ValiCraft/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ValiCraft.svg)](https://www.nuget.org/packages/ValiCraft/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A high-performance validation library for .NET that uses source generators to craft fast, allocation-free validation logic at compile time.

## Table of Contents

- [Why ValiCraft?](#why-valicraft)
- [Performance](#performance)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Core Concepts](#core-concepts)
  - [Defining Validators](#defining-validators)
  - [Running Validation](#running-validation)
  - [Working with Results](#working-with-results)
- [Built-in Rules](#built-in-rules)
- [Advanced Features](#advanced-features)
  - [Customizing Errors](#customizing-errors)
  - [Failure Modes](#failure-modes)
  - [Grouped Failure Modes](#grouped-failure-modes)
  - [Conditional Validation](#conditional-validation)
  - [Collection Validation](#collection-validation)
  - [Nested Object Validation](#nested-object-validation)
  - [Polymorphic Validation](#polymorphic-validation)
  - [Async Validation](#async-validation)
  - [Static Validators](#static-validators)
  - [Custom Validation Rules](#custom-validation-rules)
- [Diagnostics](#diagnostics)
- [Requirements](#requirements)
- [License](#license)
- [Contributing](#contributing)

## Why ValiCraft?

ValiCraft takes a fundamentally different approach to validation. Instead of interpreting validation rules at runtime, ValiCraft uses C# source generators to analyze your validator definitions at compile time and generate highly optimized, native validation code.

**Key Benefits:**

- **🚀 Extreme Performance** — Validation executes in nanoseconds with zero allocations for valid models
- **📦 Zero Runtime Overhead** — No reflection, no expression tree compilation, no runtime code generation
- **🔧 Compile-Time Safety** — Validation logic is generated and type-checked at build time
- **✨ Fluent API** — Clean, expressive syntax for defining validation rules
- **🎯 Full IntelliSense** — Generated code integrates seamlessly with your IDE
- **🐛 Debuggable** — Step through the generated validation logic just like your own code

## Performance

ValiCraft delivers exceptional performance compared to traditional validation libraries. The following benchmarks were run on .NET 10 (Apple M1 Pro):

### Simple Model Validation

| Method | Mean | Ratio | Allocated |
|--------|-----:|------:|----------:|
| ValiCraft (Valid) | **5.5 ns** | 1.00x | **0 B** |
| FluentValidation (Valid) | 350.6 ns | 63.6x | 664 B |
| ValiCraft (Invalid) | **115.9 ns** | 21.0x | 728 B |
| FluentValidation (Invalid) | 2,416.1 ns | 438.5x | 6,688 B |

### Collection Validation

| Method | Mean | Ratio | Allocated |
|--------|-----:|------:|----------:|
| ValiCraft (Small) | **112.9 ns** | 1.00x | 632 B |
| FluentValidation (Small) | 360.1 ns | 3.19x | 632 B |
| ValiCraft (Large) | **123.3 ns** | 1.09x | 632 B |
| FluentValidation (Large) | 365.7 ns | 3.24x | 632 B |

### Validator Instantiation

| Method | Mean | Allocated |
|--------|-----:|----------:|
| ValiCraft (Simple) | **~0 ns** | **0 B** |
| FluentValidation (Simple) | 1,690 ns | 6,640 B |
| ValiCraft (Complex) | **~0 ns** | **0 B** |
| FluentValidation (Complex) | 7,006 ns | 25,016 B |

> **Note:** ValiCraft validators have zero instantiation cost because the source generator produces pure static validation code with no runtime initialization.

## Installation

```bash
dotnet add package ValiCraft
```

This also installs the required dependencies:
- **MonadCraft** — Provides the `Result<TError, TValue>` type for functional error handling
- **ErrorCraft** — Provides `IValidationError` and `ValidationError` types

## Quick Start

### 1. Define Your Model

```csharp
public class User
{
    public required string Username { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
}
```

### 2. Create a Validator

```csharp
using ValiCraft;
using ValiCraft.Attributes;

[GenerateValidator]
public partial class UserValidator : Validator<User>
{
    protected override void DefineRules(IValidationRuleBuilder<User> builder)
    {
        builder.Ensure(x => x.Username)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3)
            .HasMaxLength(50);

        builder.Ensure(x => x.Email)
            .IsNotNullOrWhiteSpace()
            .IsEmailAddress();

        builder.Ensure(x => x.Age)
            .IsGreaterOrEqualThan(0)
            .IsLessThan(150);
    }
}
```

The `[GenerateValidator]` attribute tells the source generator to create the validation implementation. The `partial` keyword is required to allow the generated code to extend your class.

### 3. Run Validation

```csharp
var validator = new UserValidator();
var user = new User { Username = "john", Email = "john@example.com", Age = 30 };

// Option 1: Get a Result<IValidationErrors, User>
var result = validator.Validate(user);

string message = result.Match(
    success: u => $"Welcome, {u.Username}!",
    failure: errors => $"Validation failed: {errors.Message}");

// Option 2: Get errors as a list
IReadOnlyList<IValidationError> errors = validator.ValidateToList(user);
if (errors.Count == 0)
{
    Console.WriteLine("User is valid!");
}
```

## Core Concepts

### Defining Validators

Every validator inherits from `Validator<T>` (or `AsyncValidator<T>` for async rules) and overrides the `DefineRules` method:

```csharp
[GenerateValidator]
public partial class OrderValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        // Define your validation rules here
    }
}
```

The two key rule chain builders are:

- **`Ensure(x => x.Property)`** — Validates a single property
- **`EnsureEach(x => x.Collection, ...)`** — Validates each item in a collection

### Running Validation

ValiCraft validators implement `IValidator<T>`, which provides two methods:

```csharp
public interface IValidator<TRequest>
{
    // Returns a Result type for functional error handling
    Result<IValidationErrors, TRequest> Validate(TRequest request);
    
    // Returns errors as a list (empty if valid)
    IReadOnlyList<IValidationError> ValidateToList(TRequest request);
}
```

### Working with Results

The `Validate` method returns a `Result<IValidationErrors, TRequest>` which can be pattern matched:

```csharp
var result = validator.Validate(user);

// Using Match for exhaustive handling
var output = result.Match(
    success: user => ProcessUser(user),
    failure: errors => HandleErrors(errors));

// Check success/failure
if (result.IsSuccess)
{
    var validUser = result.Value;
}
```

Each `IValidationError` contains rich information:

```csharp
public interface IValidationError
{
    string Code { get; }           // Error code (e.g., "IsNotNullOrWhiteSpace")
    string Message { get; }        // Human-readable message
    string TargetName { get; }     // Property name (humanized)
    string TargetPath { get; }     // Full path (e.g., "Orders[0].Total")
    ErrorSeverity Severity { get; }
    IReadOnlyDictionary<string, object>? Metadata { get; }
}

// ValidationError<T> also includes the attempted value
public interface IValidationError<out T> : IValidationError
{
    T AttemptedValue { get; }
}
```

## Built-in Rules

ValiCraft includes 50+ built-in validation rules:

### String Rules

| Rule | Description |
|------|-------------|
| `IsNotNull()` | Value must not be null |
| `IsNotNullOrEmpty()` | String must not be null or empty |
| `IsNotNullOrWhiteSpace()` | String must not be null, empty, or whitespace |
| `HasMinLength(n)` | Minimum string length |
| `HasMaxLength(n)` | Maximum string length |
| `HasLength(n)` | Exact string length |
| `HasLengthBetween(min, max)` | String length within range |
| `IsEmailAddress()` | Valid email format |
| `IsUrl()` | Valid HTTP/HTTPS URL |
| `IsAlphaNumeric()` | Only letters and digits |
| `StartsWith(prefix)` | Starts with substring |
| `EndsWith(suffix)` | Ends with substring |
| `Contains(substring)` | Contains substring |
| `Matches(pattern)` | Matches regex pattern (string) |
| `MatchesRegex(regex)` | Matches compiled regex |

### Numeric/Comparison Rules

| Rule | Description |
|------|-------------|
| `IsGreaterThan(value)` | Greater than |
| `IsGreaterOrEqualThan(value)` | Greater than or equal |
| `IsLessThan(value)` | Less than |
| `IsLessOrEqualThan(value)` | Less than or equal |
| `IsBetween(min, max)` | Within range (inclusive) |
| `IsBetweenExclusive(min, max)` | Within range (exclusive) |
| `IsPositive()` | Greater than zero |
| `IsPositiveOrZero()` | Greater than or equal to zero |
| `IsNegative()` | Less than zero |
| `IsNegativeOrZero()` | Less than or equal to zero |
| `IsEqual(value)` | Equals value |
| `IsNotEqual(value)` | Not equals value |
| `IsNotDefault()` | Not default value for type |

### Collection Rules

| Rule | Description |
|------|-------------|
| `HasMinCount(n)` | Minimum item count |
| `HasMaxCount(n)` | Maximum item count |
| `HasCount(n)` | Exact item count |
| `HasCountBetween(min, max)` | Item count within range |
| `IsEmpty()` | Collection must be empty |
| `HasItems()` | Collection must have items |
| `IsUnique()` | All items unique |
| `IsUniqueWithComparer(comparer)` | Unique with custom comparer |
| `CollectionContains(item)` | Contains specific item |
| `CollectionNotContains(item)` | Does not contain item |

### Date/Time Rules

| Rule | Description |
|------|-------------|
| `IsInFuture()` | After current UTC time |
| `IsInFutureOrPresent()` | At or after current UTC time |
| `IsInPast()` | Before current UTC time |
| `IsInPastOrPresent()` | At or before current UTC time |
| `IsAfter(date)` | After specified date |
| `IsBefore(date)` | Before specified date |
| `IsAtOrAfter(date)` | At or after specified date |
| `IsAtOrBefore(date)` | At or before specified date |
| `IsDateBetween(start, end)` | Within date range |
| `HasMinAge(years)` | Minimum age from birth date |
| `HasMaxAge(years)` | Maximum age from birth date |

### Inclusion/Exclusion Rules

| Rule | Description |
|------|-------------|
| `IsIn(collection)` | Value in allowed set |
| `IsNotIn(collection)` | Value not in forbidden set |
| `IsInValues(v1, v2, ...)` | Value in allowed values |
| `IsNotInValues(v1, v2, ...)` | Value not in forbidden values |

### Custom Predicate

| Rule | Description |
|------|-------------|
| `Is(predicate)` | Custom validation predicate |
| `Is(async predicate)` | Async custom predicate |

## Advanced Features

### Customizing Errors

ValiCraft provides comprehensive error customization through fluent methods that can be chained after any rule:

#### Error Message

Override the default message using `WithMessage()` with support for placeholders:

```csharp
builder.Ensure(x => x.Age)
    .IsGreaterOrEqualThan(18)
    .WithMessage("You must be at least {ValueToCompare} years old to register");
```

**Available Placeholders:**

| Placeholder | Description                                                                                           |
|-------------|-------------------------------------------------------------------------------------------------------|
| `{TargetName}` | Property name (humanized: "firstName" → "First Name")                                                 |
| `{TargetValue}` | The actual value being validated                                                                      |
| `{ValueToCompare}` | The comparison value (for comparison rules)                                                           |
| Rule-specific | Each rule may define additional placeholders. Check XML comments of rules for available placeholders. |

**Format Specifiers:**

Placeholders support standard .NET format specifiers:

```csharp
builder.Ensure(x => x.Price)
    .IsGreaterThan(0)
    .WithMessage("Price must be positive. Got: {TargetValue:C}");  // Currency format

builder.Ensure(x => x.Percentage)
    .IsBetween(0, 100)
    .WithMessage("Value {TargetValue:F2} is outside the valid range");  // 2 decimal places
```

#### Error Code

Set a custom error code for programmatic error handling:

```csharp
builder.Ensure(x => x.Email)
    .IsEmailAddress()
    .WithErrorCode("INVALID_EMAIL_FORMAT");
```

#### Target Name

Override the humanized property name in error messages:

```csharp
builder.Ensure(x => x.DOB)
    .IsInPast()
    .WithTargetName("Date of Birth");  // Instead of "DOB"
```

#### Severity

Set the error severity level:

```csharp
builder.Ensure(x => x.Email)
    .IsEmailAddress()
    .WithSeverity(ErrorSeverity.Error);      // Default - blocking error

builder.Ensure(x => x.Nickname)
    .HasMaxLength(20)
    .WithSeverity(ErrorSeverity.Warning);    // Non-blocking warning

builder.Ensure(x => x.ProfilePicture)
    .IsNotNull()
    .WithSeverity(ErrorSeverity.Info);       // Informational message
```

#### Metadata

Attach custom metadata to errors for additional context:

```csharp
builder.Ensure(x => x.OrderTotal)
    .IsGreaterThan(0)
    .WithMetadata("Category", "Financial")
    .WithMetadata("RequiresReview", true);
```

#### Chaining All Overrides

All error customizations can be chained together:

```csharp
builder.Ensure(x => x.OrderTotal)
    .IsGreaterThan(0)
        .WithMessage("Order total must be positive")
        .WithErrorCode("INVALID_ORDER_TOTAL")
        .WithTargetName("Order Total")
        .WithSeverity(ErrorSeverity.Error)
        .WithMetadata("Category", "Financial");
```

### Failure Modes

Control validation flow when a rule fails using `OnFailureMode`:

```csharp
// Halt: Stop validating this property on first failure
builder.Ensure(x => x.Age, OnFailureMode.Halt)
    .IsGreaterOrEqualThan(0)   // If this fails...
    .IsLessThan(150);           // ...this won't run

// Continue (default): Collect all errors
builder.Ensure(x => x.Name, OnFailureMode.Continue)
    .IsNotNullOrWhiteSpace()
    .HasMinLength(2)
    .HasMaxLength(100);
```

### Grouped Failure Modes

Use `WithOnFailure` to apply a failure mode to multiple rule chains at once:

```csharp
builder.WithOnFailure(OnFailureMode.Halt, b =>
{
    // All rules in this block share the Halt failure mode
    b.Ensure(x => x.OrderNumber)
        .IsNotNull();

    b.EnsureEach(x => x.LineItems, itemBuilder =>
    {
        itemBuilder.Ensure(item => item.Code)
            .IsNotNull();
    });
});
```

**Nested Failure Modes:**

You can nest failure modes with different behaviors at each level:

```csharp
builder.WithOnFailure(OnFailureMode.Halt, b =>
{
    // Parent: Halt on any failure
    b.Ensure(x => x.OrderNumber)
        .IsNotNull();

    // Child: Continue collecting all item errors even if parent would halt
    b.EnsureEach(x => x.LineItems, OnFailureMode.Continue, itemBuilder =>
    {
        itemBuilder.Ensure(item => item.Code)
            .IsNotNull();

        itemBuilder.Ensure(item => item.Amount)
            .IsGreaterThan(0);
    });
});
```

### Conditional Validation

Apply rules only when a condition is met:

```csharp
// Property-level condition
builder.Ensure(x => x.PhoneNumber)
    .IsNotNullOrWhiteSpace()
    .If(x => x.ContactPreference == "phone");

// Rule-level condition
builder.Ensure(x => x.ShippingAddress)
    .IsNotNullOrWhiteSpace()
    .If(x => x.RequiresShipping);
```

### Collection Validation

Validate each item in a collection:

```csharp
builder.EnsureEach(x => x.OrderItems, itemBuilder =>
{
    itemBuilder.Ensure(item => item.ProductId)
        .IsNotNullOrWhiteSpace();

    itemBuilder.Ensure(item => item.Quantity)
        .IsGreaterThan(0);
        
    itemBuilder.Ensure(item => item.Price)
        .IsPositive();
});
```

Error paths include the index: `"OrderItems[0].Quantity"`, `"OrderItems[1].Price"`, etc.

### Nested Object Validation

Delegate validation to another validator using `ValidateWith`:

```csharp
// Single nested object
builder.Ensure(x => x.BillingAddress)
    .ValidateWith(new AddressValidator());

// Collection items
builder.EnsureEach(x => x.LineItems)
    .ValidateWith(new LineItemValidator());
```

You can inject validators via constructor for better testability:

```csharp
[GenerateValidator]
public partial class OrderValidator(IValidator<Address> addressValidator) : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.ShippingAddress)
            .ValidateWith(addressValidator);
    }
}
```

### Polymorphic Validation

Validate properties with abstract or interface types differently based on their runtime type using `Polymorphic`:

```csharp
public abstract class Payment { public decimal Amount { get; set; } }
public class CreditCardPayment : Payment { public string CardNumber { get; set; } }
public class CryptoPayment : Payment { public string WalletAddress { get; set; } }
public class CashPayment : Payment { }

public class Order
{
    public Payment? Payment { get; set; }
}

[GenerateValidator]
public partial class OrderValidator(
    IValidator<CreditCardPayment> creditCardValidator,
    IValidator<CryptoPayment> cryptoValidator) : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Polymorphic(x => x.Payment)
            .WhenType<CreditCardPayment>().ValidateWith(creditCardValidator)
            .WhenType<CryptoPayment>().ValidateWith(cryptoValidator)
            .WhenType<CashPayment>().Fail("Cash payments are not accepted")
            .Otherwise().Allow();  // Or .Fail() to reject unknown types
    }
}
```

**Type-Specific Actions:**

| Action | Description |
|--------|-------------|
| `.ValidateWith(validator)` | Delegate to a validator for that type |
| `.Allow()` | Allow the type without additional validation |
| `.Fail()` | Reject with a default error message |
| `.Fail("message")` | Reject with a custom message (supports `{TargetName}` placeholder) |

**Null Handling:**

By default, null values are skipped. To fail on null:

```csharp
builder.Polymorphic(x => x.Payment, PolymorphicNullBehavior.Fail)
    .WhenType<CreditCardPayment>().ValidateWith(creditCardValidator)
    .Otherwise().Allow();
```

### Async Validation

For rules that need async operations (database checks, API calls):

```csharp
[GenerateValidator]
public partial class UserValidator : AsyncValidator<User>
{
    private readonly IUserRepository _repository;

    public UserValidator(IUserRepository repository)
    {
        _repository = repository;
    }

    protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
    {
        builder.Ensure(x => x.Username)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3)
            .Is(async (username, ct) => 
                await _repository.IsUsernameAvailableAsync(username, ct))
            .WithMessage("Username '{TargetValue}' is already taken");
    }
}

// Usage
var validator = new UserValidator(repository);
var result = await validator.ValidateAsync(user, cancellationToken);
var errors = await validator.ValidateToListAsync(user, cancellationToken);
```

Async validators implement `IAsyncValidator<T>`:

```csharp
public interface IAsyncValidator<TRequest>
{
    Task<Result<IValidationErrors, TRequest>> ValidateAsync(
        TRequest request, 
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<IValidationError>> ValidateToListAsync(
        TRequest request, 
        CancellationToken cancellationToken = default);
}
```

### Static Validators

For scenarios where you don't need dependency injection and want the simplest possible API, ValiCraft provides static validators. These generate pure static methods with zero instantiation cost:

```csharp
[GenerateValidator]
public partial class AddressValidator : StaticValidator<Address>
{
    protected override void DefineRules(IValidationRuleBuilder<Address> builder)
    {
        builder.Ensure(x => x.Street)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.City)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.PostalCode)
            .Matches(@"^\d{5}$");
    }
}

// Usage - no instantiation needed!
var result = AddressValidator.Validate(address);
var errors = AddressValidator.ValidateToList(address);
```

**When to use static validators:**

- ✅ Self-contained validation logic with no external dependencies
- ✅ Maximum performance with zero allocation for validator instance
- ✅ Simple APIs where static method calls are preferred
- ✅ Composing validators using `Validate<TValidator>()` instead of dependency injection

**When to use regular validators:**

- ✅ Need to inject services (repositories, configuration, etc.)
- ✅ Runtime-configurable validation rules
- ✅ Using `ValidateWith(validator)` with injected validator instances

#### Static Async Validators

For async validation without dependency injection:

```csharp
[GenerateValidator]
public partial class OrderValidator : StaticAsyncValidator<Order>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();
    }
}

// Usage
var result = await OrderValidator.ValidateAsync(order, cancellationToken);
```

#### Composing Static Validators

Use `Validate<TValidator>()` to compose static validators without instantiation:

```csharp
[GenerateValidator]
public partial class CustomerValidator : StaticValidator<Customer>
{
    protected override void DefineRules(IValidationRuleBuilder<Customer> builder)
    {
        // Delegate to another static validator - no instance needed!
        builder.Ensure(x => x.BillingAddress)
            .Validate<AddressValidator>();

        builder.EnsureEach(x => x.ShippingAddresses)
            .Validate<AddressValidator>();
    }
}
```

This also works with polymorphic validation:

```csharp
builder.Polymorphic(x => x.Payment)
    .WhenType<CreditCardPayment>().Validate<CreditCardPaymentValidator>()
    .WhenType<CryptoPayment>().Validate<CryptoPaymentValidator>()
    .Otherwise().Allow();
```

**Constraints:**

Static validators are stateless by design. The source generator will report errors if you try to add:

- Parameterized constructors (`VALC301`)
- Instance fields (`VALC302`)
- Instance properties (`VALC303`)
- Instance methods (`VALC304`)

### Custom Validation Rules

ValiCraft provides multiple ways to define custom validation rules, from simple inline predicates to reusable extension methods.

#### The `Is()` Method

The `Is()` method is the foundation of custom validation in ValiCraft. It accepts a predicate function that returns `true` if the value is valid, `false` otherwise.

**Basic Usage — Inline Lambda:**

```csharp
builder.Ensure(x => x.PostalCode)
    .Is(code => code != null && code.Length == 5)
    .WithMessage("Postal code must be exactly 5 characters");
```

**With Regex:**

```csharp
builder.Ensure(x => x.Email)
    .Is(email => Regex.IsMatch(email ?? "", @"^[\w.-]+@[\w.-]+\.\w+$"))
    .WithMessage("Invalid email format");
```

**Block Lambda for Complex Logic:**

```csharp
builder.Ensure(x => x.Password)
    .Is(password =>
    {
        if (string.IsNullOrEmpty(password)) return false;
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsDigit)) return false;
        return true;
    })
    .WithMessage("Password must be at least 8 characters with uppercase and digit");
```

**Method Reference (Identifier Name):**

Reference a static method directly:

```csharp
public static class ValidationHelpers
{
    public static bool IsValidLuhn(string? value)
    {
        // Luhn algorithm implementation
        if (string.IsNullOrEmpty(value)) return false;
        // ... validation logic
        return true;
    }
}

// In validator
builder.Ensure(x => x.CreditCardNumber)
    .Is(ValidationHelpers.IsValidLuhn)
    .WithMessage("Invalid credit card number");
```

**Member Access:**

Reference a method on a class:

```csharp
builder.Ensure(x => x.OrderDate)
    .Is(BusinessRules.Orders.IsValidOrderDate);
```

#### `Is()` with Parameters

The `Is()` method has overloads that accept additional parameters (up to 6). This allows you to pass comparison values that can be used in error messages via `[RulePlaceholder]`:

**Single Parameter:**

```csharp
// Using built-in rule with parameter
builder.Ensure(x => x.Quantity)
    .Is(Rules.GreaterThan, 0);  // Same as .IsGreaterThan(0)

// Custom rule with parameter
public static bool IsDivisibleBy(int value, int divisor) => divisor != 0 && value % divisor == 0;

builder.Ensure(x => x.Quantity)
    .Is(IsDivisibleBy, 10)
    .WithMessage("Quantity must be divisible by 10");
```

**Multiple Parameters:**

```csharp
// Two parameters
public static bool IsBetween(int value, int min, int max) => value >= min && value <= max;

builder.Ensure(x => x.Age)
    .Is(IsBetween, 18, 120)
    .WithMessage("Age must be between 18 and 120");

// Three parameters
public static bool IsValidDate(DateTime value, int minYear, int maxYear, bool allowWeekends)
{
    if (value.Year < minYear || value.Year > maxYear) return false;
    if (!allowWeekends && (value.DayOfWeek == DayOfWeek.Saturday || value.DayOfWeek == DayOfWeek.Sunday)) return false;
    return true;
}

builder.Ensure(x => x.AppointmentDate)
    .Is(IsValidDate, 2020, 2030, false)
    .WithMessage("Date must be a weekday between 2020 and 2030");
```

**Parameter Overloads:**

| Overload | Signature |
|----------|-----------|
| No params | `Is(Func<TTarget, bool> rule)` |
| 1 param | `Is<TParam>(Func<TTarget, TParam, bool> rule, TParam param)` |
| 2 params | `Is<TParam1, TParam2>(Func<TTarget, TParam1, TParam2, bool> rule, TParam1 p1, TParam2 p2)` |
| 3 params | `Is<T1, T2, T3>(...)` |
| 4 params | `Is<T1, T2, T3, T4>(...)` |
| 5 params | `Is<T1, T2, T3, T4, T5>(...)` |
| 6 params | `Is<T1, T2, T3, T4, T5, T6>(...)` |

#### Async `Is()` Rules

In async validators, `Is()` also accepts async predicates with `CancellationToken`:

```csharp
[GenerateValidator]
public partial class UserValidator : AsyncValidator<User>
{
    private readonly IUserRepository _repository;

    public UserValidator(IUserRepository repository)
    {
        _repository = repository;
    }

    protected override void DefineRules(IAsyncValidationRuleBuilder<User> builder)
    {
        // Async lambda
        builder.Ensure(x => x.Username)
            .Is(async (username, ct) => await _repository.IsUsernameAvailableAsync(username, ct))
            .WithMessage("Username is already taken");

        // Async with parameter
        builder.Ensure(x => x.Email)
            .Is(async (email, domain, ct) => await _repository.IsEmailAllowedAsync(email, domain, ct), "example.com")
            .WithMessage("Email must be from the example.com domain");
    }
}
```

**Async Parameter Overloads:**

| Overload | Signature |
|----------|-----------|
| No params | `Is(Func<TTarget, CancellationToken, Task<bool>> rule)` |
| 1 param | `Is<TParam>(Func<TTarget, TParam, CancellationToken, Task<bool>> rule, TParam param)` |
| 2 params | `Is<TParam1, TParam2>(...)` |
| ... | Up to 6 parameters |

#### Using Static Methods

Reference any static method that returns `bool`. The first parameter is the value being validated:

```csharp
public static class CustomRules
{
    public static bool IsValidPostalCode(string? value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return Regex.IsMatch(value, @"^\d{5}(-\d{4})?$");
    }
}

// In your validator
builder.Ensure(x => x.PostalCode)
    .Is(CustomRules.IsValidPostalCode);
```

#### Creating Reusable Extension Methods

For rules you want to reuse across validators with full IntelliSense support and default messages, create an extension method with the required attributes:

```csharp
using ValiCraft;
using ValiCraft.Attributes;
using ValiCraft.BuilderTypes;

public static class MyRuleExtensions
{
    /// <summary>
    /// Validates that a string is a valid US postal code.
    /// </summary>
    [DefaultMessage("{TargetName} must be a valid US postal code")]
    [MapToValidationRule(typeof(MyRules), nameof(MyRules.UsPostalCode))]
    public static IValidationRuleBuilderType<TRequest, string?> IsUsPostalCode<TRequest>(
        this IBuilderType<TRequest, string?> builder)
        where TRequest : class
    {
        return builder.Is(MyRules.UsPostalCode);
    }

    /// <summary>
    /// Validates that a number is divisible by a specified value.
    /// </summary>
    [DefaultMessage("{TargetName} must be divisible by {Divisor}")]
    [RulePlaceholder("{Divisor}", "divisor")]
    [MapToValidationRule(typeof(MyRules), nameof(MyRules.DivisibleBy))]
    public static IValidationRuleBuilderType<TRequest, int> IsDivisibleBy<TRequest>(
        this IBuilderType<TRequest, int> builder,
        int divisor)
        where TRequest : class
    {
        return builder.Is(MyRules.DivisibleBy, divisor);
    }
}

public static class MyRules
{
    public static bool UsPostalCode(string? value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return Regex.IsMatch(value, @"^\d{5}(-\d{4})?$");
    }

    public static bool DivisibleBy(int value, int divisor)
    {
        return divisor != 0 && value % divisor == 0;
    }
}
```

**Required Attributes:**

| Attribute | Purpose |
|-----------|---------|
| `[MapToValidationRule(Type, string)]` | **Required.** Maps the extension method to its static rule method. The source generator uses this to find the rule implementation. |
| `[DefaultMessage(string)]` | Sets the default error message. Supports `{TargetName}` and `{TargetValue}` placeholders plus custom placeholders. |
| `[RulePlaceholder(string, string)]` | Defines custom placeholders for error messages. First parameter is the placeholder name (with braces), second is the parameter name. |

**Usage:**

```csharp
[GenerateValidator]
public partial class AddressValidator : Validator<Address>
{
    protected override void DefineRules(IValidationRuleBuilder<Address> builder)
    {
        builder.Ensure(x => x.PostalCode)
            .IsUsPostalCode();  // Uses default message

        builder.Ensure(x => x.UnitNumber)
            .IsDivisibleBy(100)
            .WithMessage("Unit must be in increments of {Divisor}");  // Custom message
    }
}
```

#### Extension Method Pattern

The extension method pattern follows this structure:

```csharp
[DefaultMessage("...")]           // Optional: default error message
[RulePlaceholder("...", "...")]   // Optional: custom placeholders (can have multiple)
[MapToValidationRule(typeof(RulesClass), nameof(RulesClass.MethodName))]  // Required
public static IValidationRuleBuilderType<TRequest, TTarget> RuleName<TRequest>(
    this IBuilderType<TRequest, TTarget> builder,
    /* optional parameters */)
    where TRequest : class
{
    return builder.Is(RulesClass.MethodName /* , parameters */);
}
```

**Key points:**

1. The extension method extends `IBuilderType<TRequest, TTarget>` and returns `IValidationRuleBuilderType<TRequest, TTarget>`
2. The `[MapToValidationRule]` attribute is required — the source generator will emit `VALC207` if missing
3. The rule method must be a static method that returns `bool`, with the value to validate as the first parameter
4. Use `#nullable disable` / `#nullable restore` around the method if the target type is a nullable reference type that shouldn't accept null

#### Async Custom Rules

For async rules, use the async overload of `Is()`:

```csharp
builder.Ensure(x => x.Username)
    .Is(async (username, ct) => await IsUsernameAvailableAsync(username, ct))
    .WithMessage("Username is already taken");
```

## Diagnostics

ValiCraft emits helpful compiler diagnostics when there are issues with your validators:

| Code | Description |
|------|-------------|
| `VALC201` | Missing `partial` keyword on validator class |
| `VALC202` | Missing `Validator<T>` or `AsyncValidator<T>` base class |
| `VALC203` | Invalid rule invocation — use `.Is()` or a valid extension method |
| `VALC204` | Invalid lambda in `EnsureEach` — expects a lambda as the last parameter |
| `VALC205` | Cannot retrieve parameter name from lambda definition |
| `VALC206` | Invalid builder argument used in scope |
| `VALC207` | Missing `[MapToValidationRule]` attribute on extension method |
| `VALC301` | Static validator has parameterized constructor |
| `VALC302` | Static validator has instance field |
| `VALC303` | Static validator has instance property |
| `VALC304` | Static validator has instance method |

These diagnostics appear directly in your IDE with exact source locations.

## Requirements

- **.NET 8.0** or higher
- **C# 12** or higher

## License

MIT — see [LICENSE](LICENSE) for details.

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.
