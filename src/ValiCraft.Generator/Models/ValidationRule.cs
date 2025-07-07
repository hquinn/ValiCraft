using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record ValidationRule(
    ClassInfo Class,
    string NameForExtensionMethod,
    MessageInfo? DefaultMessage,
    MethodSignature IsValidSignature,
    EquatableArray<RulePlaceholder> RulePlaceholders)
{
    public MapToValidationRuleData GetMapToValidationRuleData()
    {
        return new MapToValidationRuleData(Class.FullyQualifiedWithoutGenerics,
            Class.FullyQualifiedUnboundedName,
            GetValidationRuleGenericFormat());
    }

    public string GetParametersForExtensionMethod(string builderParameter)
    {
        var parameterCount = IsValidSignature.Parameters.Count - 1;

        if (parameterCount == 0)
        {
            return builderParameter;
        }

        var parameters = IsValidSignature.Parameters.Skip(1).Select(parameter =>
        {
            if (parameter.Type.IsGeneric)
            {
                var genericParameter = Class.GenericParameters
                    .First(genericParameter => genericParameter.Type.Matches(parameter.Type));

                if (genericParameter.InheritedPositions.Contains(0))
                {
                    parameter = parameter with { Type = new("TPropertyType", true, false) };
                }
            }

            return parameter.ToString();
        });

        return $"{builderParameter}, {string.Join(", ", parameters)}";
    }

    public string GetGenericArgumentsForExtensionMethod()
    {
        var genericParametersOutput = string.Join(", ",
            // Check if any has an inherited position of 0, which maps to TPropertyType on IValidationRule
            Class.GenericParameters
                .Select(x => (x.InheritedPositions.Contains(0) ? x.Type with {TypeName = "TPropertyType" } : x.Type).TypeName));

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

    private string GetValidationRuleGenericFormat()
    {
        if (Class.GenericParameters.Count == 0)
        {
            return string.Empty;
        }

        var mappingIndexes = new int[Class.GenericParameters.Count];

        for (var genericIndex = 0; genericIndex < Class.GenericParameters.Count; genericIndex++)
        {
            var genericParameter = Class.GenericParameters[genericIndex];

            var found = false;
            for (var methodIndex = 0; methodIndex < IsValidSignature.Parameters.Count; methodIndex++)
            {
                var methodParameter = IsValidSignature.Parameters[methodIndex];

                if (methodParameter.Type.Matches(genericParameter.Type))
                {
                    mappingIndexes[genericIndex] = methodIndex;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                mappingIndexes[genericIndex] = -1;
            }
        }

        return $"<{string.Join(", ", mappingIndexes.Select(x => $"{{{x}}}"))}>";
    }
}