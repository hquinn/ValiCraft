using System;
using System.Linq;

namespace ValiCraft.Generator.Utils;

public static class TypeComparisonUtils
{
    /// <summary>
    /// Compares two type name strings for equivalence, ignoring nullable annotations ('?').
    /// For example, "T?" is considered equivalent to "T".
    /// </summary>
    /// <param name="typeName1">The first type name string.</param>
    /// <param name="typeName2">The second type name string.</param>
    /// <returns>True if the base type names are identical.</returns>
    public static bool AreEquivalent(string? typeName1, string? typeName2)
    {
        if (string.IsNullOrEmpty(typeName1) || string.IsNullOrEmpty(typeName2))
        {
            return false;
        }

        // Normalize both strings by trimming any trailing '?' character.
        if (typeName1!.EndsWith("?"))
        {
            typeName1 = typeName1.Substring(0, typeName1.Length - 1);
        }

        if (typeName2!.EndsWith("?"))
        {
            typeName2 = typeName2.Substring(0, typeName2.Length - 1);
        }

        // Compare the normalized spans for equality.
        return typeName1.SequenceEqual(typeName2);
    }
}