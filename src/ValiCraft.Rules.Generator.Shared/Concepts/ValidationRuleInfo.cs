using System;
using System.Linq;
using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Utils;

namespace ValiCraft.Rules.Generator.Shared.Concepts;

public record ValidationRuleInfo
{
    public ValidationRuleInfo(ClassInfo classInfo, string nameForExtensionMethod, MethodSignature isValidSignature)
    {
        Class = classInfo;
        NameForExtensionMethod = nameForExtensionMethod;
        IsValidSignature = isValidSignature;
    }
    
    public ClassInfo Class { get; init; }
    public string NameForExtensionMethod { get; init; }
    public MethodSignature IsValidSignature { get; init; }

    public string GetParametersForExtensionMethod(string builderParameter)
    {
        var parameterCount = IsValidSignature.Parameters.Count - 1;

        if (parameterCount == 0)
        {
            return builderParameter;
        }

        var parameters = IsValidSignature.Parameters.Skip(1).Select(parameter => parameter.ToString());
        
        return $"{builderParameter}, {string.Join(", ", parameters)}";
    }

    public string GetGenericArgumentsForExtensionMethod()
    {
        var genericParametersOutput = string.Join(", ",
            // Check if any has an inherited position of 0, which maps to TPropertyType on IValidationRule
            Class.GenericParameters
                .Select(x => x.InheritedPositions.Contains(0) ? "TPropertyType" : x.Name));

        if (string.IsNullOrWhiteSpace(genericParametersOutput))
        {
            return "TRequest, TPropertyType";
        }
        
        if (genericParametersOutput.Contains("TPropertyType"))
        {
            return $"TRequest, {genericParametersOutput}";
        }
        
        return $"TRequest, TPropertyType, {genericParametersOutput}";
    }

    public string GetValidationRuleGenericFormat()
    {
        if (Class.GenericParameters.Count == 0)
        {
            return string.Empty;
        }

        var mappingIndexes = new int[Class.GenericParameters.Count];

        for (var genericParameterIndex = 0; genericParameterIndex < Class.GenericParameters.Count; genericParameterIndex++)
        {
            var genericParameter = Class.GenericParameters[genericParameterIndex];

            var found = false;
            for (var methodParameterIndex = 0; methodParameterIndex < IsValidSignature.Parameters.Count; methodParameterIndex++)
            {
                var methodParameter = IsValidSignature.Parameters[methodParameterIndex];

                if (methodParameter.TypeIsGeneric &&
                    TypeComparisonUtils.AreEquivalent(methodParameter.TypeName, genericParameter.Name))
                {
                    mappingIndexes[genericParameterIndex] = methodParameterIndex;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                mappingIndexes[genericParameterIndex] = -1;
            }
        }

        return $"<{string.Join(", ", mappingIndexes.Select(x => $"{{{x}}}"))}>";
    }
}