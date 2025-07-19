using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Concepts;
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
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location) : Rule(
    Arguments,
    DefaultMessage,
    DefaultErrorCode,
    RuleOverrides,
    Placeholders,
    Location)
{
    public override string GenerateCodeForRule(
        string requestName,
        string indent,
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
                     {indent}{GetIfElseIfKeyword(context)} (!{validationRuleInvocation}.IsValid({isValidCallArgsString}))
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

        var args = Arguments
            .Select(argument => argument.Type.FormattedTypeName)
            .Prepend(target.Type.FormattedTypeName)
            .ToArray<object>();

        return string.Format(ValidationRuleData!.ValidationRuleGenericFormat, args);
    }
}