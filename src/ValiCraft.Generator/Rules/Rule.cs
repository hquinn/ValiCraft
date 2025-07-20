using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.Rules;

public abstract record Rule(
    EquatableArray<ArgumentInfo> Arguments,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    RuleOverrideData RuleOverrides,
    IfConditionModel IfCondition,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location)
{
    private const string FallbackMessage = "\"'An error has occurred\"";
    public abstract Rule? EnrichRule(
        ValidationTarget target,
        ValidationRule[] validRules,
        SourceProductionContext context);

    public abstract string GenerateCodeForRule(
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context);

    protected string GetErrorCreation(
        string requestName,
        string validationRuleInvocation,
        IndentModel indent,
        ValidationTarget target,
        RuleChainContext context)
    {
        const string errorTypeName = $"global::{KnownNames.Types.ValidationError}";
        const string errorSeverity = $"global::{KnownNames.Enums.ErrorSeverity}";
        var targetAccess = string.Format(target.AccessorExpressionFormat, requestName);
        
        var targetNameInfo = RuleOverrides.OverrideTargetName ?? target.DefaultTargetName;

        string targetPath;
        if (target.AccessorType == AccessorType.Object)
        {
            targetPath = $$"""
                           $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "{{context.TargetPath}}" : null)}"
                           """;
        }
        else
        {
            targetPath = $$"""
                           $"{inheritedTargetPath}{{context.TargetPath}}"
                           """;
        }

        return $$"""
                 {{indent}}{
                 {{indent}}    errors ??= new({{context.Counter}});
                 {{indent}}    errors.Add(new {{errorTypeName}}<{{target.Type.FormattedTypeName}}>
                 {{indent}}    {
                 {{indent}}        Code = {{GetErrorCode(validationRuleInvocation)}},
                 {{indent}}        Message = {{GetErrorMessage(requestName, target, targetNameInfo)}},
                 {{indent}}        Severity = {{errorSeverity}}.Error,
                 {{indent}}        TargetName = "{{targetNameInfo.Value}}",
                 {{indent}}        TargetPath = {{targetPath}},
                 {{indent}}        AttemptedValue = {{targetAccess}},
                 {{indent}}    });
                 {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}}
                 """;
    }

    private static string GetGotoLabelIfNeeded(IndentModel indent, RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                    {indent}    goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }

    protected virtual string GetErrorCode(string validationRuleInvocation)
    {
        if (RuleOverrides.OverrideErrorCode is not null)
        {
            if (RuleOverrides.OverrideErrorCode.IsLiteral)
            {
                return $"\"{RuleOverrides.OverrideErrorCode.Value}\"";
            }
            
            return RuleOverrides.OverrideErrorCode.Value;
        }

        if (DefaultErrorCode is not null)
        {
            return $"\"{DefaultErrorCode.Value}\"";
        }

        return $"nameof({validationRuleInvocation})";
    }

    private string GetErrorMessage(string requestName, ValidationTarget target, MessageInfo targetNameInfo)
    {
        var messageInfo = RuleOverrides.OverrideMessage ?? DefaultMessage;
        if (messageInfo is null)
        {
            return FallbackMessage;
        }

        // Build a complete map of all available placeholders for this rule invocation.
        var placeholderMap = BuildPlaceholderMap(requestName, target, targetNameInfo);

        // Pass the template and the map to the builder.
        return BuildMessage(messageInfo, placeholderMap);
    }

    private Dictionary<string, ArgumentInfo> BuildPlaceholderMap(
        string requestName,
        ValidationTarget target,
        MessageInfo targetNameInfo)
    {
        var map = new Dictionary<string, ArgumentInfo>
        {
            // Add the standard placeholders.
            // We treat them just like any other argument.
            {
                "{TargetName}", new ArgumentInfo(
                    "TargetName",
                    targetNameInfo.Value,
                    new TypeInfo("string", false, false),
                    targetNameInfo.IsLiteral,
                    null)
            },
            {
                "{TargetValue}", new ArgumentInfo(
                    "TargetValue",
                    string.Format(target.AccessorExpressionFormat, requestName),
                    target.Type,
                    false,
                    null)
            }
        };

        // Add custom placeholders by mapping them to the invocation arguments by name.
        foreach (var placeholder in Placeholders)
        {
            var argument = Arguments.FirstOrDefault(x => x.Name == placeholder.ParameterName);
            if (argument is not null)
            {
                map.Add(placeholder.PlaceholderName, argument);
            }
        }

        return map;
    }

    private static string BuildMessage(
        MessageInfo messageTemplate,
        Dictionary<string, ArgumentInfo> placeholderMap)
    {
        // We can do maximum compile-time optimization and generate an efficient interpolated string.
        if (messageTemplate.IsLiteral)
        {
            return BuildMessageFromLiteral(messageTemplate, placeholderMap);
        }

        // We must generate a chain of .Replace() calls to be executed at runtime.
        return BuildMessageFromExpression(messageTemplate, placeholderMap);
    }

    private static string BuildMessageFromLiteral(
        MessageInfo messageTemplate,
        Dictionary<string, ArgumentInfo> placeholderMap)
    {
        var templateBuilder = new StringBuilder(messageTemplate.Value);

        foreach (var entry in placeholderMap)
        {
            var placeholderText = entry.Key;
            var replacementInfo = entry.Value;

            // If the replacement value is a literal, bake it in.
            // Otherwise, create a C# interpolation hole for the expression.
            var replacementExpression = replacementInfo.IsLiteral
                ? replacementInfo.ConstantValue?.ToString() ?? replacementInfo.Value
                : $"{{{replacementInfo.Value}}}";

            templateBuilder.Replace(placeholderText, replacementExpression);
        }

        // Return a valid C# interpolated string expression.
        return $"$\"{templateBuilder.Replace("\"", "\"\"")}\"";
    }

    private static string BuildMessageFromExpression(
        MessageInfo messageTemplate,
        Dictionary<string, ArgumentInfo> placeholderMap)
    {
        var expressionBuilder = new StringBuilder(messageTemplate.Value);

        foreach (var entry in placeholderMap)
        {
            var placeholderText = entry.Key;
            var replacementInfo = entry.Value;

            var valueExpression = replacementInfo.IsLiteral ? $"\"{replacementInfo.Value}\"" : replacementInfo.Value;

            var finalReplacementExpression = replacementInfo.Type.PureTypeName.EndsWith("string")
                ? valueExpression
                : $"{valueExpression}?.ToString() ?? \"\"";

            expressionBuilder.Append($".Replace(\"{placeholderText}\", {finalReplacementExpression})");
        }

        return expressionBuilder.ToString();
    }
}