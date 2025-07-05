using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public record GenericConstraintsInfo(string Type, EquatableArray<string> Constraints)
{
    public static GenericConstraintsInfo? CreateFromTypeParameterSymbol(
        ITypeParameterSymbol typeParameter)
    {
        var constraints = new List<string>();

        // Check for class, struct, notnull, unmanaged, and new() constraints
        if (typeParameter.HasReferenceTypeConstraint)
        {
            constraints.Add("class");
        }

        if (typeParameter.HasValueTypeConstraint)
        {
            constraints.Add("struct");
        }

        if (typeParameter.HasNotNullConstraint)
        {
            constraints.Add("notnull");
        }

        if (typeParameter.HasUnmanagedTypeConstraint)
        {
            constraints.Add("unmanaged");
        }

        // Add any type constraints (e.g., where T : IMyInterface)
        foreach (var constraintType in typeParameter.ConstraintTypes)
        {
            constraints.Add(constraintType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        if (typeParameter.HasConstructorConstraint)
        {
            constraints.Add("new()");
        }

        if (constraints.Count == 0)
        {
            return null;
        }

        return new GenericConstraintsInfo(typeParameter.Name, constraints.ToEquatableImmutableArray());
    }

    public override string ToString()
    {
        return $"where {Type} : {string.Join(", ", Constraints)}";
    }
}