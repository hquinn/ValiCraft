using ValiCraft.IntegrationTests.Models;

namespace ValiCraft.IntegrationTests.Helpers;

/// <summary>
/// Shared factory methods for creating valid/invalid test models.
/// Centralizes test data so refactoring models requires changes in one place.
/// </summary>
public static class TestData
{
    public static Order ValidOrder() => new()
    {
        OrderNumber = "ORD-001",
        CustomerName = "John Doe",
        OrderTotal = 99.99m,
        Quantity = 5,
        OrderDate = DateTime.UtcNow,
        ShippingAddress = ValidAddress(),
        LineItems = [ValidLineItem(), ValidLineItem("SKU-002", 29.99m)],
        Payment = new CreditCardPayment { CardNumber = "4111111111111111", Amount = 99.99m },
        IsExpress = false,
        IsInternational = false,
        Notes = "Test order",
        Tags = ["priority", "wholesale"]
    };

    public static LineItem ValidLineItem(string code = "SKU-001", decimal price = 19.99m) => new()
    {
        Code = code,
        Description = "Test item",
        Price = price,
        Quantity = 1
    };

    public static Address ValidAddress() => new()
    {
        Street = "123 Main St",
        City = "Springfield",
        ZipCode = "62704"
    };

    public static Coordinate ValidCoordinate() => new()
    {
        Latitude = 45.0,
        Longitude = -93.0
    };

    public static GeoRect ValidGeoRect() => new(North: 45.0, South: 30.0, East: -80.0, West: -100.0);

    public static ShippingInfo ValidShippingInfo() => new("FedEx", "TRACK-12345", 10);

    public static Invoice ValidInvoice() => new()
    {
        Prefix = "INV",
        Number = 1001,
        Subtotal = 100m,
        TaxRate = 0.1m
    };
}
