# ValiCraft

[![NuGet](https://img.shields.io/nuget/v/ValiCraft.svg)](https://www.nuget.org/packages/ValiCraft/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ValiCraft.svg)](https://www.nuget.org/packages/ValiCraft/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A high-performance validation library for .NET that uses source generators to craft fast, allocation-free validation logic at compile time.

## Why ValiCraft?

ValiCraft takes a fundamentally different approach to validation. Instead of interpreting validation rules at runtime, ValiCraft uses C# source generators to analyze your validator definitions at compile time and generate highly optimized, native validation code.

**Key Benefits:**

- **🚀 Extreme Performance** — Validation executes in nanoseconds with zero allocations for valid models
- **📦 Zero Runtime Overhead** — No reflection, no expression tree compilation, no runtime code generation
- **🔧 Compile-Time Safety** — Validation logic is generated and type-checked at build time
- **✨ Clean API** — Fluent builder pattern with IntelliSense support
- **🔌 DI Ready** — Source-generated dependency injection with no reflection


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

The `[GenerateValidator]` attribute tells the source generator to create the validation implementation. The `partial` keyword is required to allow the generated code to extend your type.

**Optional Configuration:**

The `[GenerateValidator]` attribute supports the following properties:

- `IncludeDefaultMetadata` (default: `false`) — When `true`, includes default metadata (`RequestType` and `ValidationCount`) in the generated `ValidationErrors` object.

```csharp
[GenerateValidator(IncludeDefaultMetadata = true)]
public partial class UserValidator : Validator<User>
{
    // ...
}
```

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

## Performance

ValiCraft delivers exceptional performance compared to traditional validation libraries. Here's a snapshot (full benchmarks in [docs/performance.md](docs/performance.md)):

### Simple Validation - Valid Model
| Method                | Mean          | Gen0   | Allocated |
|---------------------- |--------------:|-------:|----------:|
| ValiCraft             |     5.4063 ns |      - |         - |
| ValiCraftWithMetaData |     5.4166 ns |      - |         - |
| FluentValidation      |   168.1272 ns | 0.1097 |     688 B |

### Simple Validation - Invalid Model
| Method                | Mean          | Gen0   | Allocated |
|---------------------- |--------------:|-------:|----------:|
| ValiCraft             |    94.5754 ns | 0.0777 |     488 B |
| ValiCraftWithMetaData |   148.4748 ns | 0.1160 |     728 B |
| FluentValidation      | 2,561.6943 ns | 1.0681 |    6712 B |

> **Note:** ValiCraft validators have zero instantiation cost because the source generator produces pure static validation code with no runtime initialization. See [full benchmark results](docs/performance.md) for collection, complex, and instantiation benchmarks.

## Installation

```bash
dotnet add package ValiCraft
```

This also installs the required dependencies:
- **MonadCraft** — Provides the `Result<TError, TValue>` type for functional error handling
- **ErrorCraft** — Provides `IValidationError` and `ValidationError` types

## Documentation

| Topic | Description |
|-------|-------------|
| [Core Concepts](docs/core-concepts.md) | Defining validators, running validation, and working with results |
| [Built-in Rules](docs/built-in-rules.md) | 50+ built-in rules for strings, numbers, collections, dates, and more |
| [Advanced Features](docs/advanced-features.md) | Error customization, failure modes, conditional validation, collection/nested/polymorphic validation, async, static validators, and custom rules |
| [Dependency Injection](docs/dependency-injection.md) | AOT-friendly DI registration, service lifetimes, and multi-project solutions |
| [Performance](docs/performance.md) | Full benchmark results and comparisons |

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
| `VALC208` | Invalid statement in `DefineRules` — only builder invocations are allowed |
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
