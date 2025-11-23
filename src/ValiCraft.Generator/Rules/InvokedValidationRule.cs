using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public abstract record InvokedValidationRule(
    string MethodName,
    MapToValidationRuleData? ValidationRuleData,
    EquatableArray<ArgumentInfo> Arguments,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    RuleOverrideData RuleOverrides,
    IfConditionModel IfCondition,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location) : Rule(
    Arguments,
    DefaultMessage,
    DefaultErrorCode,
    RuleOverrides,
    IfCondition,
    Placeholders,
    Location)
{
    public override string GenerateCodeForRule(
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context)
    {
        var targetAccess = string.Format(target.AccessorExpressionFormat, requestName);
        var validationRuleInvocation =
            $"global::{ValidationRuleData?.FullyQualifiedValidationRule}{ConstructValidationRuleGeneric(target)}";

        var isValidCallArgs = new List<string> { targetAccess };
        isValidCallArgs.AddRange(Arguments.GetArray()?.Select(x => x.Value) ?? []);
        var isValidCallArgsString = string.Join(", ", isValidCallArgs);

        var code = $"""
                     {IfCondition.GenerateIfBlock(@object, requestName, indent, context)}!{validationRuleInvocation}.IsValid({isValidCallArgsString}))
                     {GetErrorCreation(requestName, validationRuleInvocation, indent, target, context)}
                     """;
        
        context.UpdateIfElseMode();
        
        return code;
    }

    
    private string? ConstructValidationRuleGeneric(ValidationTarget target)
    {
        if (string.IsNullOrEmpty(ValidationRuleData?.ValidationRuleGenericFormat))
        {
            return null;
        }

        var args = new List<object>();

        // First, add the target type
        args.Add(target.Type.FormattedTypeName);

        // Then, add argument types
        foreach (var argument in Arguments.GetArray() ?? [])
        {
            args.Add(argument.Type.FormattedTypeName);
        }

        // Check if format string contains any negative placeholders (e.g., {-1000}, {-1001}, etc.)
        // These indicate generic parameters nested in the target type
        var format = ValidationRuleData!.ValidationRuleGenericFormat;
        if (format.Contains("{-"))
        {
            // Extract the generic arguments from the target type for each negative placeholder
            var targetTypeName = target.Type.FormattedTypeName;
            
            // Replace negative placeholders with extracted generic arguments
            format = System.Text.RegularExpressions.Regex.Replace(format, @"\{-1000\}", match =>
            {
                // -1000 indicates the first generic parameter that needs extraction
                var genericArg = ExtractFirstGenericArgument(targetTypeName);
                return genericArg ?? targetTypeName;
            });
            
            // For now, we only handle -1000 (first generic parameter)
            // If there are multiple generic parameters in rules, we'd need to extract them differently
        }

        return string.Format(format, args.ToArray());
    }

    private static string? ExtractFirstGenericArgument(string typeName)
    {
        // Extract the first generic argument from a type string like "List<string>" -> "string"
        // or "IEnumerable<int>?" -> "int"
        var startIndex = typeName.IndexOf('<');
        if (startIndex < 0) return null;

        var endIndex = typeName.LastIndexOf('>');
        if (endIndex < 0) return null;

        var genericPart = typeName.Substring(startIndex + 1, endIndex - startIndex - 1);

        // Handle nested generics by counting angle brackets
        var depth = 0;
        for (int i = 0; i < genericPart.Length; i++)
        {
            if (genericPart[i] == '<') depth++;
            else if (genericPart[i] == '>') depth--;
            else if (genericPart[i] == ',' && depth == 0)
            {
                // Multiple generic arguments, return the first one
                return genericPart.Substring(0, i).Trim();
            }
        }

        return genericPart.Trim();
    }
}