# Contributing to ValiCraft

First off, thank you for considering contributing to ValiCraft! It's people like you that make ValiCraft such a great tool.

## Code of Conduct

This project and everyone participating in it is governed by our commitment to creating a welcoming and inclusive environment. Please be respectful and constructive in all interactions.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When creating a bug report, please include:

- **A clear and descriptive title**
- **Steps to reproduce the issue**
- **Expected behavior** vs **actual behavior**
- **Code samples** that demonstrate the issue
- **.NET version** and **ValiCraft version**
- **Any relevant error messages or stack traces**

### Suggesting Features

Feature suggestions are welcome! Please open an issue with:

- **A clear and descriptive title**
- **A detailed description** of the proposed feature
- **Use cases** explaining why this feature would be useful
- **Code examples** showing how the feature might be used

### Pull Requests

1. **Fork the repository** and create your branch from `main`
2. **Write tests** for any new functionality
3. **Ensure all tests pass** by running `dotnet test`
4. **Follow the existing code style**
5. **Update documentation** as needed
6. **Write a clear commit message** describing your changes

## Development Setup

### Prerequisites

- .NET 8.0 SDK or higher
- An IDE with C# support (VS Code, Visual Studio, Rider)

### Building the Project

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/ValiCraft.git
cd ValiCraft

# Build the solution
dotnet build

# Run tests
dotnet test

# Run benchmarks (optional)
cd benchmarks/ValiCraft.Benchmarks
dotnet run -c Release
```

### Project Structure

```
ValiCraft/
├── src/
│   ├── ValiCraft/              # Main library (runtime types)
│   └── ValiCraft.Generator/    # Source generator (compile-time)
├── tests/
│   ├── ValiCraft.Tests/        # Runtime tests
│   └── ValiCraft.Generator.Tests/  # Generator tests
└── benchmarks/
    └── ValiCraft.Benchmarks/   # Performance benchmarks
```

### Writing Tests

- **Generator tests** verify that the source generator produces correct code
- **Runtime tests** verify that the generated code behaves correctly
- Use the existing test patterns as a guide

### Code Style

- Use C# 12 features where appropriate
- Follow Microsoft's C# coding conventions
- Add XML documentation to all public APIs
- Keep methods focused and small
- Prefer immutability where possible

## Creating Custom Validation Rules

If you'd like to contribute new built-in validation rules:

1. Create the rule class in `src/ValiCraft/Rules/`
2. Add appropriate attributes (`GenerateRuleExtension`, `DefaultMessage`, etc.)
3. Include comprehensive unit tests
4. Update the README if adding a significant rule

### Example Rule Structure

```csharp
using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsValidPostalCode")]
[DefaultMessage("'{TargetName}' must be a valid postal code")]
[DefaultErrorCode("INVALID_POSTAL_CODE")]
public class PostalCodeRule : IValidationRule<string?>
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return Regex.IsMatch(value, @"^\d{5}(-\d{4})?$");
    }
}
```

## Questions?

Feel free to open an issue with the "question" label if you have any questions about contributing.

Thank you for contributing! 🎉
