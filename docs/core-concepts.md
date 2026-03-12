# Core Concepts

## Defining Validators

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
- **`EnsureEach(x => x.Collection)`** — Validates each item in a collection (chain rules directly for simple types, or use a builder callback for complex types)

## Running Validation

ValiCraft validators implement `IValidator<T>`:

```csharp
public interface IValidator<TRequest>
{
    ValidationErrors? Validate(TRequest request);
    [EditorBrowsable(Never)] ValidationErrors? Validate(TRequest request, string? inheritedTargetPath);
}
```

## Working with Results

The `Validate` method returns `ValidationErrors?` — `null` means the model is valid, non-null means there are errors:

```csharp
var result = validator.Validate(user);

if (result is null)
{
    // Model is valid
    ProcessUser(user);
}
else
{
    // result.Errors contains the list of validation errors
    HandleErrors(result);
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
