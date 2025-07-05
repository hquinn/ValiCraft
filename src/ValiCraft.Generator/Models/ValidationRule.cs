using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Models;

public record ValidationRule
{
    public ValidationRule(
        ClassInfo classInfo,
        string nameForExtensionMethod,
        MessageInfo? defaultMessage,
        MethodSignature isValidSignature,
        EquatableArray<RulePlaceholder> rulePlaceholders)
    {
        Class = classInfo;
        NameForExtensionMethod = nameForExtensionMethod;
        DefaultMessage = defaultMessage;
        IsValidSignature = isValidSignature;
        RulePlaceholders = rulePlaceholders;
    }

    public ClassInfo Class { get; init; }
    public string NameForExtensionMethod { get; init; }
    public MessageInfo? DefaultMessage { get; init; }
    public MethodSignature IsValidSignature { get; init; }
    public EquatableArray<RulePlaceholder> RulePlaceholders { get; init; }

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
            if (parameter.TypeIsGeneric)
            {
                var genericParameter = Class.GenericParameters
                    .First(genericParameter => genericParameter.Name == parameter.TypeName);

                if (genericParameter.InheritedPositions.Contains(0))
                {
                    parameter = parameter with { TypeName = "TPropertyType" };
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

        for (var genericParameterIndex = 0;
             genericParameterIndex < Class.GenericParameters.Count;
             genericParameterIndex++)
        {
            var genericParameter = Class.GenericParameters[genericParameterIndex];

            var found = false;
            for (var methodParameterIndex = 0;
                 methodParameterIndex < IsValidSignature.Parameters.Count;
                 methodParameterIndex++)
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