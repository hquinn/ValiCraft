namespace ValiCraft.IntegrationTests.Models;

/// <summary>
/// Struct type to test struct validation support.
/// </summary>
public struct Coordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

/// <summary>
/// Struct with a method to test struct + method target combination.
/// </summary>
public struct Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double GetArea() => Width * Height;
}

/// <summary>
/// Class with methods to test method validation targets.
/// </summary>
public class Customer
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Balance { get; set; }

    public string GetFullName() => $"{FirstName} {LastName}";
    public decimal GetDiscountedBalance(decimal discountRate) => Balance * (1 - discountRate);
}

/// <summary>
/// Model with non-nullable string properties to verify type safety.
/// </summary>
public class NonNullableModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
}

/// <summary>
/// Model with nested objects to test ValidateWith.
/// </summary>
public class ParentModel
{
    public string Name { get; set; } = string.Empty;
    public ChildModel Child { get; set; } = new();
}

public class ChildModel
{
    public string Description { get; set; } = string.Empty;
    public int Value { get; set; }
}

/// <summary>
/// Model with collection properties.
/// </summary>
public class CollectionModel
{
    public List<string> Tags { get; set; } = [];
    public List<ChildModel> Children { get; set; } = [];
}

/// <summary>
/// Model with nullable string properties to verify nullability support.
/// </summary>
public class NullableModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
}
