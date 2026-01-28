using System;
using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public record ExtensionMethodRule(
    EquatableArray<string> GenericArguments,
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
            $"global::{ValidationRuleData?.RuleType}.{ConstructValidationRuleGeneric()}";
        var errorCode = $"global::{ValidationRuleData?.RuleType}.{ValidationRuleData?.MethodName}";

        var isValidCallArgs = new List<string> { targetAccess };
        isValidCallArgs.AddRange(Arguments.GetArray()?.Select(x => x.Value) ?? []);
        var isValidCallArgsString = string.Join(", ", isValidCallArgs);

        var code = $"""
                     {IfCondition.GenerateIfBlock(@object, requestName, indent, context)}!{validationRuleInvocation}({isValidCallArgsString}))
                     {GetErrorCreation(requestName, errorCode, indent, target, context)}
                     """;
        
        context.UpdateIfElseMode();
        
        return code;
    }
    
    private string? ConstructValidationRuleGeneric()
    {
        if (string.IsNullOrEmpty(ValidationRuleData?.MethodName))
        {
            return null;
        }

        if (GenericArguments.Count == 0 ||
            (ValidationRuleData?.GenericArgumentIndices is null && GenericArguments.Count <= 1) ||
            ValidationRuleData?.GenericArgumentIndices == EquatableArray<int>.Empty)
        {
            return ValidationRuleData?.MethodName;
        }

        if (ValidationRuleData?.GenericArgumentIndices is null)
        {
            return $"{ValidationRuleData?.MethodName}<{string.Join(", ", GenericArguments.Skip(1))}>";
        }
        
        return $"{ValidationRuleData!.MethodName}<{string.Join(", ", GenericArguments.Where((_, i) => ValidationRuleData?.GenericArgumentIndices.Contains(i) == true))}>";
    }
}