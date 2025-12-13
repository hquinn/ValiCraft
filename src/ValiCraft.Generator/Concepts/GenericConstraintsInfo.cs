using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public record GenericConstraintsInfo(string Type, EquatableArray<string> Constraints)
{
    /// <summary>
    /// Creates a new GenericConstraintsInfo with all occurrences of the original type parameter name
    /// replaced with the substitute name in both Type and all constraint strings.
    /// </summary>
    public GenericConstraintsInfo WithSubstitutedTypeName(string originalName, string substituteName)
    {
        var newConstraints = new List<string>(Constraints.Count);
        foreach (var constraint in Constraints)
        {
            newConstraints.Add(SubstituteGenericParameter(constraint, originalName, substituteName));
        }
        
        return new GenericConstraintsInfo(
            Type == originalName ? substituteName : Type,
            newConstraints.ToEquatableImmutableArray());
    }
    
    /// <summary>
    /// Substitutes a generic parameter name within a type string.
    /// Handles cases like "IComparable&lt;T&gt;" -> "IComparable&lt;TTargetType&gt;"
    /// and "Func&lt;T, bool&gt;" -> "Func&lt;TTargetType, bool&gt;"
    /// </summary>
    public static string SubstituteGenericParameter(string typeString, string originalName, string substituteName)
    {
        if (string.IsNullOrEmpty(typeString) || string.IsNullOrEmpty(originalName))
        {
            return typeString;
        }
        
        // We need to be careful to only replace the type parameter, not parts of other names
        // e.g., "Test" should not become "TesTargetType" when replacing "T"
        // Pattern: match T when preceded by start, <, comma+space, or space and followed by end, >, comma, ?, or space
        var result = new System.Text.StringBuilder();
        var i = 0;
        
        while (i < typeString.Length)
        {
            // Check if we're at a potential match position
            if (i + originalName.Length <= typeString.Length)
            {
                var potentialMatch = typeString.Substring(i, originalName.Length);
                if (potentialMatch == originalName)
                {
                    // Check if this is a standalone type parameter (not part of a larger identifier)
                    var charBefore = i > 0 ? typeString[i - 1] : '\0';
                    var charAfter = i + originalName.Length < typeString.Length 
                        ? typeString[i + originalName.Length] 
                        : '\0';
                    
                    var isValidBefore = charBefore == '\0' || charBefore == '<' || charBefore == ',' || 
                                        charBefore == ' ' || charBefore == '(' || charBefore == ':';
                    var isValidAfter = charAfter == '\0' || charAfter == '>' || charAfter == ',' || 
                                       charAfter == ' ' || charAfter == ')' || charAfter == '?' ||
                                       charAfter == '[' || charAfter == ']';
                    
                    if (isValidBefore && isValidAfter)
                    {
                        result.Append(substituteName);
                        i += originalName.Length;
                        continue;
                    }
                }
            }
            
            result.Append(typeString[i]);
            i++;
        }
        
        return result.ToString();
    }
    
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