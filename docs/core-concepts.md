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

## Working with Results

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
