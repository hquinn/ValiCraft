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

ValiCraft validators implement:
 - `IValidator<T>` if inheriting from `Validator<T>`
 - `IAsyncValidator<T>` if inheriting from `AsyncValidator<T>`
 - `IStaticValidator<T>` if inheriting from `StaticValidator<T>`
 - `IStaticAsyncValidator<T>` if inheriting from `StaticAsyncValidator<T>`

Each interface defines a `ValidationError? Validate/ValidateAsync(T request)` method to use (with async providing a `CancellationToken` parameter).

Static validators makes use of static abstract members in the `Validate` methods.

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
