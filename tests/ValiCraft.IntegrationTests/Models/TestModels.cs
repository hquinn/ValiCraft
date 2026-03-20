namespace ValiCraft.IntegrationTests.Models;

// --- Core domain models ---

public class Order
{
    public string? OrderNumber { get; set; }
    public string? CustomerName { get; set; }
    public decimal OrderTotal { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public Address ShippingAddress { get; set; } = new();
    public List<LineItem> LineItems { get; set; } = [];
    public Payment? Payment { get; set; }
    public bool IsExpress { get; set; }
    public bool IsInternational { get; set; }
    public string? Notes { get; set; }
    public List<string> Tags { get; set; } = [];
}

public class LineItem
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
}

// --- Polymorphic hierarchy ---

public abstract class Payment
{
    public decimal Amount { get; set; }
}

public class CreditCardPayment : Payment
{
    public string? CardNumber { get; set; }
}

public class CryptoPayment : Payment
{
    public string? WalletAddress { get; set; }
}

public class BankTransferPayment : Payment
{
    public string? AccountNumber { get; set; }
}

// --- Struct types ---

public struct Coordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

// --- Record struct ---

public record struct GeoRect(double North, double South, double East, double West);

// --- Record class ---

public record ShippingInfo(string? Carrier, string? TrackingNumber, int WeightKg);

// --- Method target model ---

public class Invoice
{
    public string? Prefix { get; set; }
    public int Number { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }

    public string GetInvoiceId() => $"{Prefix}-{Number}";
    public decimal GetTotalWithTax(decimal taxRate) => Subtotal * (1 + taxRate);
}

// --- Nested class host ---

public partial class FeatureModule
{
    public class NestedOrder
    {
        public string? Name { get; set; }
        public decimal Total { get; set; }
    }

    [global::ValiCraft.Attributes.GenerateValidator]
    public partial class NestedOrderValidator : global::ValiCraft.Validator<NestedOrder>
    {
        protected override void DefineRules(global::ValiCraft.IValidationRuleBuilder<NestedOrder> builder)
        {
            builder.Ensure(x => x.Name)
                .IsNotNullOrWhiteSpace();

            builder.Ensure(x => x.Total)
                .IsGreaterThan(0m);
        }
    }
}
