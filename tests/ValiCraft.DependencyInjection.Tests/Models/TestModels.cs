namespace ValiCraft.DependencyInjection.Tests.Models;

public class TestOrder
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal OrderTotal { get; set; }
    public TestCustomer Customer { get; set; } = new();
}

public class TestCustomer
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class TestItem
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
