# Dependency Injection

ValiCraft provides AOT-friendly dependency injection support built directly into the source generator. When your project references `Microsoft.Extensions.DependencyInjection`, the generator automatically discovers all your validators at compile time and generates registration code — no additional packages or reflection required.

## Registering All Validators

Call `AddValiCraft()` to register all non-static validators discovered at compile time:

```csharp
using ValiCraft.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValiCraft();
```

This registers:

- Every `Validator<T>` as `IValidator<T>`
- Every `AsyncValidator<T>` as `IAsyncValidator<T>`

Static validators (`StaticValidator<T>` and `StaticAsyncValidator<T>`) are excluded because they use static methods and do not require DI.

Validators are then injected wherever they are needed:

```csharp
public class OrderController(IValidator<Order> orderValidator)
{
    public IActionResult CreateOrder(Order order)
    {
        var result = orderValidator.Validate(order);

        if (result is null)
        {
            return Ok(order);
        }

        return BadRequest(result);
    }
}
```

## Service Lifetime

By default, validators are registered as **Transient**. You can override this:

```csharp
builder.Services.AddValiCraft(ServiceLifetime.Scoped);
```

Transient is the safest default because validators may accept other validators through constructor injection. Using Scoped or Singleton is fine when you know your validators are stateless.

## Multi-Project Solutions

In solutions with multiple projects, the source generator runs per-project. Each project that contains validators gets its own generated registrar. Module projects only need a reference to ValiCraft — they do not need to reference `Microsoft.Extensions.DependencyInjection`.

**Host-level registration** — A single `AddValiCraft()` call in your host project registers all validators from the current project **and** all referenced projects:

```csharp
// In MyApp.WebApi (references MyApp.Orders and MyApp.Customers)
builder.Services.AddValiCraft(); // Registers validators from all three projects
```

**Module-level registration** — Each project also gets its own extension method if you want to register modules independently:

```csharp
// Register only validators from the Orders module
builder.Services.AddMyAppOrdersValiCraft();
```

The method name is derived from the assembly name: `Add{AssemblyName}ValiCraft()`.

Cross-project discovery works entirely at compile time. The generator emits an assembly-level attribute for each project with validators, and the host project's generator discovers these attributes when it compiles. No runtime reflection is involved.
