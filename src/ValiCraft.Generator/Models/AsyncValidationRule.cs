using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

/// <summary>
/// Represents an async validation rule class being processed by the source generator.
/// </summary>
public record AsyncValidationRule(
    ClassInfo Class,
    string NameForExtensionMethod,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    MethodSignature IsValidAsyncSignature,
    EquatableArray<RulePlaceholder> RulePlaceholders)
{
    /// <summary>
    /// Gets the mapping index for each generic parameter.
    /// Positive values indicate the parameter index in IsValidAsync method.
    /// Negative values indicate the parameter is nested within another type (e.g., T in IEnumerable&lt;T&gt;).
    /// </summary>
    private int[] GetGenericParameterMappingIndexes()
    {
        if (Class.GenericParameters.Count == 0)
        {
            return [];
        }

        var mappingIndexes = new int[Class.GenericParameters.Count];

        for (var genericIndex = 0; genericIndex < Class.GenericParameters.Count; genericIndex++)
        {
            var genericParameter = Class.GenericParameters[genericIndex];

            var found = false;
            // Skip the last parameter (CancellationToken)
            for (var methodIndex = 0; methodIndex < IsValidAsyncSignature.Parameters.Count - 1; methodIndex++)
            {
                var methodParameter = IsValidAsyncSignature.Parameters[methodIndex];

                if (methodParameter.Type.Matches(genericParameter.Type))
                {
                    mappingIndexes[genericIndex] = methodIndex;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // Generic parameter is not directly in the method signature.
                // This likely means it's nested within a generic type (e.g., IEnumerable<T>).
                // We'll use a special marker that will be handled during code generation.
                mappingIndexes[genericIndex] = -1000 - genericIndex;
            }
        }

        return mappingIndexes;
    }
    
    public MapToValidationRuleData GetMapToValidationRuleData()
    {
        return new MapToValidationRuleData(Class.FullyQualifiedWithoutGenerics,
            Class.FullyQualifiedUnboundedName,
            GetValidationRuleGenericFormat());
    }

    public string GetParametersForExtensionMethod(string builderParameter)
    {
        // Exclude last parameter (CancellationToken) and first parameter (target)
        var parameterCount = IsValidAsyncSignature.Parameters.Count - 2;

        if (parameterCount <= 0)
        {
            return builderParameter;
        }

        // Build a mapping of original generic parameter names to their substitutes
        var substitutions = Class.GenericParameters
            .Where(gp => gp.InheritedPositions.Contains(0))
            .Select(gp => gp.Type.FormattedTypeName)
            .ToList();

        // Skip first parameter (target) and last parameter (CancellationToken)
        var parameters = IsValidAsyncSignature.Parameters.Skip(1).Take(parameterCount).Select(parameter =>
        {
            var typeString = parameter.Type.FormattedTypeName;
            
            // Apply all substitutions for generic parameters at position 0
            foreach (var originalName in substitutions)
            {
                typeString = GenericConstraintsInfo.SubstituteGenericParameter(typeString, originalName, "TTargetType");
            }
            
            // If the type changed, create a new TypeInfo
            if (typeString != parameter.Type.FormattedTypeName)
            {
                parameter = parameter with { Type = new TypeInfo(typeString, parameter.Type.IsGeneric, parameter.Type.IsNullable) };
            }

            return parameter.ToString();
        });

        return $"{builderParameter}, {string.Join(", ", parameters)}";
    }

    /// <summary>
    /// Gets the generic parameters that should be included in the extension method,
    /// excluding nested type parameters that can't be inferred.
    /// </summary>
    public IEnumerable<(string Name, GenericConstraintsInfo? Constraints, bool IsTargetType)> GetExtensionMethodGenericParameters()
    {
        var mappingIndexes = GetGenericParameterMappingIndexes();
        
        for (var i = 0; i < Class.GenericParameters.Count; i++)
        {
            var genericParameter = Class.GenericParameters[i];
            var mappingIndex = mappingIndexes[i];
            
            // Skip nested type parameters (negative mapping index)
            if (mappingIndex < 0)
            {
                continue;
            }
            
            var isTargetType = genericParameter.InheritedPositions.Contains(0);
            var name = isTargetType ? "TTargetType" : genericParameter.Type.FormattedTypeName;
            
            yield return (name, genericParameter.Constraints, isTargetType);
        }
    }

    public string GetGenericArgumentsForExtensionMethod()
    {
        var genericParametersList = GetExtensionMethodGenericParameters()
            .Select(p => p.Name)
            .ToList();
        
        var genericParametersOutput = string.Join(", ", genericParametersList);

        if (string.IsNullOrWhiteSpace(genericParametersOutput))
        {
            return "TRequest, TTargetType";
        }

        if (genericParametersOutput.Contains("TTargetType"))
        {
            return $"TRequest, {genericParametersOutput}";
        }

        return $"TRequest, TTargetType, {genericParametersOutput}";
    }

    private string GetValidationRuleGenericFormat()
    {
        if (Class.GenericParameters.Count == 0)
        {
            return string.Empty;
        }

        var mappingIndexes = GetGenericParameterMappingIndexes();
        return $"<{string.Join(", ", mappingIndexes.Select(x => $"{{{x}}}"))}>";
    }
}
