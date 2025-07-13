using System.Collections.Generic;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public record GenericParameterInfo
{
    public GenericParameterInfo(TypeInfo type, List<int> inheritedPositions)
        : this(type, inheritedPositions.ToEquatableImmutableArray(), null)
    {
    }

    public GenericParameterInfo(
        TypeInfo type,
        EquatableArray<int> inheritedPositions,
        GenericConstraintsInfo? constraints)
    {
        Type = type;
        InheritedPositions = inheritedPositions;
        Constraints = constraints;
    }

    public TypeInfo Type { get; init; }
    public EquatableArray<int> InheritedPositions { get; init; }
    public GenericConstraintsInfo? Constraints { get; init; }
}