# Contributing to ValiCraft

Thank you for considering contributing to ValiCraft! It's people like you that make ValiCraft such a great tool.

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

- .NET 10.0 SDK (also builds for .NET 9.0 and .NET 8.0)

### Building the Project

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/ValiCraft.git
cd ValiCraft

# Build the solution
dotnet build

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/ValiCraft.Generator.Tests
dotnet test tests/ValiCraft.IntegrationTests
dotnet test tests/ValiCraft.Tests

# Run benchmarks (optional)
dotnet run -c Release --project benchmarks/ValiCraft.Benchmarks
```
### Writing Tests

- **Generator tests** — Snapshot tests verifying the source generator produces correct code. Located in `tests/ValiCraft.Generator.Tests/`
- **Integration tests** — End-to-end tests that compile and run generated validators. Located in `tests/ValiCraft.IntegrationTests/`
- **Unit tests** — Tests for runtime types and behaviors. Located in `tests/ValiCraft.Tests/`

Use the existing test patterns as a guide.


## Questions?

Feel free to open an issue with the "question" label if you have any questions about contributing.

Thank you for contributing! 🎉
