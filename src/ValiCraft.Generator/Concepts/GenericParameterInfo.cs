using System.Collections.Generic;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public record GenericParameterInfo
{
    public GenericParameterInfo(string name, List<int> inheritedPositions)
        : this(name, inheritedPositions.ToEquatableImmutableArray(), string.Empty)
    {
    }

    public GenericParameterInfo(string name, EquatableArray<int> inheritedPositions, string constraints)
    {
        Name = name;
        InheritedPositions = inheritedPositions;
        Constraints = constraints;
    }

    public string Name { get; init; }
    public EquatableArray<int> InheritedPositions { get; init; }
    public string Constraints { get; init; }
}