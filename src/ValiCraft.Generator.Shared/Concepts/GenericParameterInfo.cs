using System.Collections.Generic;
using ValiCraft.Generator.Shared.Extensions;
using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Generator.Shared.Concepts;

public record GenericParameterInfo
{
    public GenericParameterInfo(string name, List<int> inheritedPositions)
        : this(name, inheritedPositions.ToEquatableImmutableArray())
    {
    }
    
    public GenericParameterInfo(string name, EquatableArray<int> inheritedPositions)
    {
        Name = name;
        InheritedPositions = inheritedPositions;
    }
    
    public string Name { get; init; }
    public EquatableArray<int> InheritedPositions { get; init; }
}