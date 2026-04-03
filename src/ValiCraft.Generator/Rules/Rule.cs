using System.Text;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

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
    private static MessageInfo FallbackMessage = new("'{TargetName}' doesn't satisfy the condition", true);

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

        var metadataValue = GetMetadata(indent);

        return $$"""
                 {{indent}}{
                 {{indent}}    errors ??= new({{context.Counter}});
                 {{indent}}    errors.Add(new {{errorTypeName}}
                 {{indent}}    {
                 {{indent}}        Code = {{GetErrorCode(validationRuleInvocation)}},
                 {{indent}}        Message = {{GetErrorMessage(requestName, target, targetNameInfo)}},
                 {{indent}}        Severity = {{GetSeverity(errorSeverity)}},
                 {{indent}}        TargetName = "{{targetNameInfo.Value}}",
                 {{indent}}        TargetPath = {{targetPath}},
                 {{indent}}        AttemptedValue = {{targetAccess}},{{metadataValue}}
                 {{indent}}    });
                 {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}}
                 """;
    }

    private string GetMetadata(IndentModel indent)
    {
        if (RuleOverrides.OverrideMetadata is not { Count: > 0 })
        {
            return string.Empty;
        }

        var entries = new StringBuilder();
        foreach (var entry in RuleOverrides.OverrideMetadata)
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

    private string GetSeverity(string errorSeverityType)
    {
        if (RuleOverrides.OverrideSeverity is not null)
        {
            var severityInfo = RuleOverrides.OverrideSeverity;
            
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
        var messageInfo = RuleOverrides.OverrideMessage ?? DefaultMessage ?? FallbackMessage;

        // Build a complete map of all available placeholders for this rule invocation.
        var placeholderMap = MessageBuilder.BuildPlaceholderMap(requestName, target, targetNameInfo, Placeholders, Arguments);

        // Pass the template and the map to the builder.
        return MessageBuilder.BuildMessage(messageInfo, placeholderMap);
    }
}