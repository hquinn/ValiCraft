namespace ValiCraft.IntegrationTests.Models;

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
