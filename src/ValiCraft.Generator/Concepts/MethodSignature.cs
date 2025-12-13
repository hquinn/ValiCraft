using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public enum SignatureMatching
{
    None,
    Full,
    Partial
}

public record MethodSignature(EquatableArray<ParameterInfo> Parameters)
{
    public SignatureMatching MatchesTypes(EquatableArray<TypeInfo> arguments)
    {
        return MatchesTypes(arguments, []);
    }
    
    /// <summary>
    /// Matches argument types against parameter types, considering type parameters.
    /// </summary>
    /// <param name="arguments">The actual argument types to check.</param>
    /// <param name="typeParameterNames">Names of type parameters from the class (e.g., "T", "TValue").
    /// Used to detect if a parameter type contains a type parameter.</param>
    /// <returns>The matching result.</returns>
    public SignatureMatching MatchesTypes(EquatableArray<TypeInfo> arguments, IReadOnlyList<string> typeParameterNames)
    {
        if (Parameters.Count != arguments.Count)
        {
            return SignatureMatching.None;
        }

        var signatureMatching = SignatureMatching.Full;

        for (var i = 0; i < Parameters.Count; i++)
        {
            var parameter = Parameters[i];
            var argument = arguments[i];

            if (argument.Matches(parameter.Type))
            {
                continue;
            }

            // Parameter is directly a type parameter (like T) - matches any type
            if (parameter.Type.IsGeneric)
            {
                signatureMatching = SignatureMatching.Partial;
                continue;
            }

            // Check if the parameter type contains any type parameter (like IEnumerable<T>)
            // This allows partial matching for types that will be substituted
            if (ContainsAnyTypeParameter(parameter.Type.FormattedTypeName, typeParameterNames))
            {
                signatureMatching = SignatureMatching.Partial;
                continue;
            }

            // Concrete type doesn't match
            return SignatureMatching.None;
        }

        return signatureMatching;
    }
    
    /// <summary>
    /// Checks if a type string contains any of the given type parameter names.
    /// Uses word boundary detection to avoid false positives (e.g., "String" containing "T").
    /// </summary>
    private static bool ContainsAnyTypeParameter(string typeString, IReadOnlyList<string> typeParameterNames)
    {
        return typeParameterNames.Any(name => ContainsTypeParameter(typeString, name));
    }
    
    /// <summary>
    /// Checks if a type string contains a specific type parameter name as a whole word.
    /// </summary>
    private static bool ContainsTypeParameter(string typeString, string typeParameterName)
    {
        var index = 0;
        while ((index = typeString.IndexOf(typeParameterName, index, System.StringComparison.Ordinal)) >= 0)
        {
            var beforeOk = index == 0 || !char.IsLetterOrDigit(typeString[index - 1]);
            var afterIndex = index + typeParameterName.Length;
            var afterOk = afterIndex >= typeString.Length || !char.IsLetterOrDigit(typeString[afterIndex]);

            if (beforeOk && afterOk)
            {
                return true;
            }

            index++;
        }

        return false;
    }
}