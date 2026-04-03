using System.Text;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

internal static class ErrorCreationHelper
{
    private static readonly MessageInfo FallbackMessage = new("'{TargetName}' doesn't satisfy the condition", true);

    internal static string GetErrorCreation(
        string requestName,
        string validationRuleInvocation,
        IndentModel indent,
        ValidationTarget target,
        RuleChainContext context,
        RuleOverrideData ruleOverrides,
        MessageInfo? defaultMessage,
        MessageInfo? defaultErrorCode,
        EquatableArray<RulePlaceholder> placeholders,
        EquatableArray<ArgumentInfo> arguments)
    {
        const string errorTypeName = $"global::{KnownNames.Types.ValidationError}";
        const string errorSeverity = $"global::{KnownNames.Enums.ErrorSeverity}";
        var targetAccess = string.Format(target.AccessorExpressionFormat, requestName);

        var targetNameInfo = ruleOverrides.OverrideTargetName ?? target.DefaultTargetName;

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

        var metadataValue = GetMetadata(ruleOverrides, indent);

        return $$"""
                 {{indent}}{
                 {{indent}}    errors ??= new({{context.Counter}});
                 {{indent}}    errors.Add(new {{errorTypeName}}
                 {{indent}}    {
                 {{indent}}        Code = {{GetErrorCode(validationRuleInvocation, ruleOverrides, defaultErrorCode)}},
                 {{indent}}        Message = {{GetErrorMessage(requestName, target, targetNameInfo, ruleOverrides, defaultMessage, placeholders, arguments)}},
                 {{indent}}        Severity = {{GetSeverity(errorSeverity, ruleOverrides)}},
                 {{indent}}        TargetName = "{{targetNameInfo.Value}}",
                 {{indent}}        TargetPath = {{targetPath}},
                 {{indent}}        AttemptedValue = {{targetAccess}},{{metadataValue}}
                 {{indent}}    });
                 {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}}
                 """;
    }

    private static string GetMetadata(RuleOverrideData ruleOverrides, IndentModel indent)
    {
        if (ruleOverrides.OverrideMetadata is not { Count: > 0 })
        {
            return string.Empty;
        }

        var entries = new StringBuilder();
        foreach (var entry in ruleOverrides.OverrideMetadata)
        {
            var valueExpression = entry.IsLiteral && entry.ValueType == "string"
                ? "\"" + entry.Value + "\""
                : entry.Value;

            entries.AppendLine();
            entries.Append(indent + "            { \"" + entry.Key + "\", " + valueExpression + " },");
        }

        var result = new StringBuilder();
        result.AppendLine();
        result.Append(indent + "        Metadata = new global::System.Collections.Generic.Dictionary<string, object>");
        result.AppendLine();
        result.Append(indent + "        {");
        result.Append(entries);
        result.AppendLine();
        result.Append(indent + "        },");

        return result.ToString();
    }

    private static string GetSeverity(string errorSeverityType, RuleOverrideData ruleOverrides)
    {
        if (ruleOverrides.OverrideSeverity is not null)
        {
            var severityInfo = ruleOverrides.OverrideSeverity;

            // If it's a literal value (e.g., directly written enum), extract the enum member name
            if (severityInfo.IsLiteral)
            {
                // For literals like "Warning", prepend the type
                return $"{errorSeverityType}.{severityInfo.Value}";
            }

            // It's an expression (e.g., ErrorSeverity.Warning, config.Severity, SomeClass.DefaultSeverity)
            var severity = severityInfo.Value;

            // If it contains "ErrorSeverity.", replace with the global type for consistency
            if (severity.Contains("ErrorSeverity."))
            {
                var value = severity.Substring(severity.LastIndexOf('.') + 1);
                return $"{errorSeverityType}.{value}";
            }

            // Otherwise, use the expression as-is (for config properties, constants, etc.)
            return severity;
        }

        return $"{errorSeverityType}.Error";
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

    internal static string GetErrorCode(
        string validationRuleInvocation,
        RuleOverrideData ruleOverrides,
        MessageInfo? defaultErrorCode)
    {
        if (ruleOverrides.OverrideErrorCode is not null)
        {
            if (ruleOverrides.OverrideErrorCode.IsLiteral)
            {
                return $"\"{ruleOverrides.OverrideErrorCode.Value}\"";
            }

            return ruleOverrides.OverrideErrorCode.Value;
        }

        if (defaultErrorCode is not null)
        {
            return $"\"{defaultErrorCode.Value}\"";
        }

        return $"nameof({validationRuleInvocation})";
    }

    private static string GetErrorMessage(
        string requestName,
        ValidationTarget target,
        MessageInfo targetNameInfo,
        RuleOverrideData ruleOverrides,
        MessageInfo? defaultMessage,
        EquatableArray<RulePlaceholder> placeholders,
        EquatableArray<ArgumentInfo> arguments)
    {
        var messageInfo = ruleOverrides.OverrideMessage ?? defaultMessage ?? FallbackMessage;

        // Build a complete map of all available placeholders for this rule invocation.
        var placeholderMap = MessageBuilder.BuildPlaceholderMap(requestName, target, targetNameInfo, placeholders, arguments);

        // Pass the template and the map to the builder.
        return MessageBuilder.BuildMessage(messageInfo, placeholderMap);
    }
}
