using ValiCraft.Attributes;
using ValiCraft.IntegrationTests.Models;

namespace ValiCraft.IntegrationTests.Validators;

// =============================================================================
// Sync Validators (Validator<T>)
// =============================================================================

// --- Core rule chain validators ---

[GenerateValidator]
public partial class LineItemValidator : Validator<LineItem>
{
    protected override void DefineRules(IValidationRuleBuilder<LineItem> builder)
    {
        builder.Ensure(x => x.Code)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Price)
            .IsGreaterThan(0m);
    }
}

[GenerateValidator]
public partial class AddressValidator : Validator<Address>
{
    protected override void DefineRules(IValidationRuleBuilder<Address> builder)
    {
        builder.Ensure(x => x.Street)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.City)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.ZipCode)
            .IsNotNullOrWhiteSpace();
    }
}

/// <summary>
/// Tests basic Ensure chains with extension method rules and multiple rules on same property.
/// </summary>
[GenerateValidator]
public partial class OrderBasicValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3)
            .HasMaxLength(20);

        builder.Ensure(x => x.OrderTotal)
            .IsGreaterThan(0m);

        builder.Ensure(x => x.Quantity)
            .IsGreaterThan(0)
            .IsLessThan(1000);
    }
}

/// <summary>
/// Tests Is() rules with expression lambda, block lambda, and method group syntax.
/// </summary>
[GenerateValidator]
public partial class OrderIsRuleValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        // Expression lambda
        builder.Ensure(x => x.OrderTotal)
            .Is(x => x > 0m);

        // Block lambda
        builder.Ensure(x => x.Quantity)
            .Is(x =>
            {
                return x > 0 && x < 1000;
            });

        // Method group
        builder.Ensure(x => x.OrderNumber)
            .Is(IsValidOrderNumber);
    }

    private static bool IsValidOrderNumber(string? value)
        => !string.IsNullOrWhiteSpace(value) && value.Length >= 3;
}

/// <summary>
/// Tests message overrides: WithMessage, WithErrorCode, WithTargetName, WithSeverity.
/// </summary>
[GenerateValidator]
public partial class OrderMessageOverrideValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace()
            .WithMessage("Order number is mandatory")
            .WithErrorCode("MISSING_ORDER_NUMBER")
            .WithTargetName("PO Number")
            .WithSeverity(ErrorSeverity.Critical);

        builder.Ensure(x => x.OrderTotal)
            .IsGreaterThan(0m)
            .WithMessage("Total must be positive")
            .WithSeverity(ErrorSeverity.Warning);
    }
}

/// <summary>
/// Tests OnFailureMode.Halt: stops after first failing rule on a chain.
/// </summary>
[GenerateValidator]
public partial class OrderHaltValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        // Halt mode: if NotNullOrWhiteSpace fails, MinLength should NOT be evaluated
        builder.Ensure(x => x.OrderNumber, OnFailureMode.Halt)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3)
            .HasMaxLength(20);

        // Continue mode (default): all rules evaluated
        builder.Ensure(x => x.Quantity)
            .IsGreaterThan(0)
            .IsLessThan(1000);
    }
}

/// <summary>
/// Tests WithOnFailure grouping with Halt and Continue modes.
/// </summary>
[GenerateValidator]
public partial class OrderWithOnFailureValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.WithOnFailure(OnFailureMode.Halt, b =>
        {
            b.Ensure(x => x.OrderNumber)
                .IsNotNullOrWhiteSpace();

            b.Ensure(x => x.OrderTotal)
                .IsGreaterThan(0m);
        });
    }
}

/// <summary>
/// Tests If() conditional validation.
/// </summary>
[GenerateValidator]
public partial class OrderConditionalValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        // Only validate Notes when IsExpress is true
        builder.If(x => x.IsExpress == true, b =>
        {
            b.Ensure(x => x.Notes)
                .IsNotNullOrWhiteSpace();
        });

        // Only validate ShippingAddress when IsInternational is true
        builder.If(x => x.IsInternational == true, b =>
        {
            b.Ensure(x => x.ShippingAddress)
                .ValidateWith(new AddressValidator());
        });
    }
}

/// <summary>
/// Tests ValidateWith for nested object validation.
/// </summary>
[GenerateValidator]
public partial class OrderNestedValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.ShippingAddress)
            .ValidateWith(new AddressValidator());
    }
}

/// <summary>
/// Tests rules then ValidateWith on the same chain (IsNotNull then ValidateWith).
/// Uses nullable Payment property to test null-guard + nested validation.
/// </summary>
[GenerateValidator]
public partial class OrderRulesThenValidateWithValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.ShippingAddress)
            .ValidateWith(new AddressValidator());
    }
}

/// <summary>
/// Tests EnsureEach for collection validation with direct rules.
/// </summary>
[GenerateValidator]
public partial class OrderEnsureEachDirectValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.EnsureEach(x => x.Tags)
            .IsNotNullOrWhiteSpace();
    }
}

/// <summary>
/// Tests EnsureEach with ValidateWith for nested collection validation.
/// </summary>
[GenerateValidator]
public partial class OrderEnsureEachValidateWithValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.EnsureEach(x => x.LineItems)
            .ValidateWith(new LineItemValidator());
    }
}

/// <summary>
/// Tests EnsureEach with lambda rules on each item.
/// </summary>
[GenerateValidator]
public partial class OrderEnsureEachLambdaValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.EnsureEach(x => x.LineItems, lineItemBuilder =>
        {
            lineItemBuilder.Ensure(li => li.Code)
                .IsNotNullOrWhiteSpace();

            lineItemBuilder.Ensure(li => li.Price)
                .IsGreaterThan(0m);
        });
    }
}

/// <summary>
/// Tests EnsureEach with If condition inside.
/// </summary>
[GenerateValidator]
public partial class OrderEnsureEachWithIfValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.EnsureEach(x => x.LineItems, lineItemBuilder =>
        {
            lineItemBuilder.If(li => li.Quantity > 0, b =>
            {
                b.Ensure(li => li.Code)
                    .IsNotNullOrWhiteSpace();
            });
        });
    }
}

/// <summary>
/// Tests EnsureEach with rules then ValidateWith on the same chain.
/// </summary>
[GenerateValidator]
public partial class OrderEnsureEachRulesThenValidateWithValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.EnsureEach(x => x.LineItems)
            .IsNotNull()
            .ValidateWith(new LineItemValidator());
    }
}

/// <summary>
/// Tests EnsureEach with OnFailureMode.Halt.
/// </summary>
[GenerateValidator]
public partial class OrderEnsureEachHaltValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.EnsureEach(x => x.Tags, OnFailureMode.Halt)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2);
    }
}

/// <summary>
/// Tests polymorphic validation with WhenType and Allow.
/// </summary>
[GenerateValidator]
public partial class OrderPolymorphicValidator(
    CreditCardPaymentValidator creditCardValidator) : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        builder.Polymorphic(x => x.Payment)
            .WhenType<CreditCardPayment>().ValidateWith(creditCardValidator)
            .WhenType<CryptoPayment>().Allow();
    }
}

[GenerateValidator]
public partial class CreditCardPaymentValidator : Validator<CreditCardPayment>
{
    protected override void DefineRules(IValidationRuleBuilder<CreditCardPayment> builder)
    {
        builder.Ensure(x => x.CardNumber)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Amount)
            .IsGreaterThan(0m);
    }
}

/// <summary>
/// Tests polymorphic validation with Fail on null.
/// </summary>
[GenerateValidator]
public partial class OrderPolymorphicFailOnNullValidator : Validator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Polymorphic(x => x.Payment, PolymorphicNullBehavior.Fail)
            .WhenType<CreditCardPayment>().Allow();
    }
}

/// <summary>
/// Tests struct target type.
/// </summary>
[GenerateValidator]
public partial class CoordinateValidator : Validator<Coordinate>
{
    protected override void DefineRules(IValidationRuleBuilder<Coordinate> builder)
    {
        builder.Ensure(x => x.Latitude)
            .IsGreaterThan(-90.0)
            .IsLessThan(90.0);

        builder.Ensure(x => x.Longitude)
            .IsGreaterThan(-180.0)
            .IsLessThan(180.0);
    }
}

/// <summary>
/// Tests record struct target type.
/// </summary>
[GenerateValidator]
public partial class GeoRectValidator : Validator<GeoRect>
{
    protected override void DefineRules(IValidationRuleBuilder<GeoRect> builder)
    {
        builder.Ensure(x => x.North)
            .IsGreaterThan(-90.0)
            .IsLessThan(90.0);

        builder.Ensure(x => x.South)
            .IsGreaterThan(-90.0)
            .IsLessThan(90.0);
    }
}

/// <summary>
/// Tests record class target type.
/// </summary>
[GenerateValidator]
public partial class ShippingInfoValidator : Validator<ShippingInfo>
{
    protected override void DefineRules(IValidationRuleBuilder<ShippingInfo> builder)
    {
        builder.Ensure(x => x.Carrier)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.TrackingNumber)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.WeightKg)
            .IsGreaterThan(0);
    }
}

/// <summary>
/// Tests method targets (parameterless and with parameters).
/// </summary>
[GenerateValidator]
public partial class InvoiceValidator : Validator<Invoice>
{
    protected override void DefineRules(IValidationRuleBuilder<Invoice> builder)
    {
        builder.Ensure(x => x.GetInvoiceId())
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.GetTotalWithTax(0.1m))
            .IsGreaterThan(0m);
    }
}

// NestedOrderValidator is defined in Models/TestModels.cs inside FeatureModule

// =============================================================================
// Async Validators (AsyncValidator<T>)
// =============================================================================

[GenerateValidator]
public partial class AsyncLineItemValidator : AsyncValidator<LineItem>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<LineItem> builder)
    {
        builder.Ensure(x => x.Code)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Price)
            .IsGreaterThan(0m);
    }
}

[GenerateValidator]
public partial class AsyncOrderBasicValidator : AsyncValidator<Order>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3);

        builder.Ensure(x => x.OrderTotal)
            .IsGreaterThan(0m);
    }
}

[GenerateValidator]
public partial class AsyncOrderConditionalValidator : AsyncValidator<Order>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        builder.If(x => x.IsExpress == true, b =>
        {
            b.Ensure(x => x.Notes)
                .IsNotNullOrWhiteSpace();
        });
    }
}

[GenerateValidator]
public partial class AsyncOrderCollectionValidator : AsyncValidator<Order>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
    {
        // Use sync validator from async context (supported by framework)
        builder.EnsureEach(x => x.LineItems)
            .ValidateWith(new LineItemValidator());
    }
}

[GenerateValidator]
public partial class AsyncOrderPolymorphicValidator(
    CreditCardPaymentValidator creditCardValidator) : AsyncValidator<Order>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
    {
        // Use sync validator from async context for polymorphic
        builder.Polymorphic(x => x.Payment)
            .WhenType<CreditCardPayment>().ValidateWith(creditCardValidator)
            .WhenType<CryptoPayment>().Allow();
    }
}

// =============================================================================
// Static Validators (StaticValidator<T>)
// =============================================================================

[GenerateValidator]
public partial class StaticLineItemValidator : StaticValidator<LineItem>
{
    protected override void DefineRules(IValidationRuleBuilder<LineItem> builder)
    {
        builder.Ensure(x => x.Code)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Price)
            .IsGreaterThan(0m);
    }
}

[GenerateValidator]
public partial class StaticOrderBasicValidator : StaticValidator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3);

        builder.Ensure(x => x.OrderTotal)
            .IsGreaterThan(0m);
    }
}

[GenerateValidator]
public partial class StaticCoordinateValidator : StaticValidator<Coordinate>
{
    protected override void DefineRules(IValidationRuleBuilder<Coordinate> builder)
    {
        builder.Ensure(x => x.Latitude)
            .IsGreaterThan(-90.0)
            .IsLessThan(90.0);

        builder.Ensure(x => x.Longitude)
            .IsGreaterThan(-180.0)
            .IsLessThan(180.0);
    }
}

[GenerateValidator]
public partial class StaticOrderConditionalValidator : StaticValidator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        builder.If(x => x.IsExpress == true, b =>
        {
            b.Ensure(x => x.Notes)
                .IsNotNullOrWhiteSpace();
        });
    }
}

[GenerateValidator]
public partial class StaticOrderCollectionValidator : StaticValidator<Order>
{
    protected override void DefineRules(IValidationRuleBuilder<Order> builder)
    {
        // Use sync validator for nested validation from static context
        builder.EnsureEach(x => x.LineItems)
            .ValidateWith(new LineItemValidator());
    }
}

// =============================================================================
// Static Async Validators (StaticAsyncValidator<T>)
// =============================================================================

[GenerateValidator]
public partial class StaticAsyncLineItemValidator : StaticAsyncValidator<LineItem>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<LineItem> builder)
    {
        builder.Ensure(x => x.Code)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Price)
            .IsGreaterThan(0m);
    }
}

[GenerateValidator]
public partial class StaticAsyncOrderBasicValidator : StaticAsyncValidator<Order>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3);

        builder.Ensure(x => x.OrderTotal)
            .IsGreaterThan(0m);
    }
}

[GenerateValidator]
public partial class StaticAsyncOrderConditionalValidator : StaticAsyncValidator<Order>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        builder.If(x => x.IsExpress == true, b =>
        {
            b.Ensure(x => x.Notes)
                .IsNotNullOrWhiteSpace();
        });
    }
}
