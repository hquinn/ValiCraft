using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                 {{indent}}    errors.Add(new {{errorTypeName}}<{{target.Type.FormattedTypeName}}>
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
                    new TypeInfo("string", false),
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

    // Regex to match placeholders with optional format specifiers: {Name} or {Name:format}
    // Captures: Group 1 = placeholder name, Group 2 = format specifier (optional)
    private static readonly Regex PlaceholderRegex = new(@"\{(\w+)(?::([^}]+))?\}", RegexOptions.Compiled);

    private static string BuildMessageFromLiteral(
        MessageInfo messageTemplate,
        Dictionary<string, ArgumentInfo> placeholderMap)
    {
        var result = PlaceholderRegex.Replace(messageTemplate.Value, match =>
        {
            var placeholderName = match.Groups[1].Value;
            var formatSpecifier = match.Groups[2].Success ? match.Groups[2].Value : null;
            var placeholderKey = $"{{{placeholderName}}}";

            if (!placeholderMap.TryGetValue(placeholderKey, out var replacementInfo))
            {
                // Unknown placeholder, leave as-is
                return match.Value;
            }

            // If the replacement value is a literal, bake it in.
            if (replacementInfo.IsLiteral)
            {
                var literalValue = replacementInfo.ConstantValue?.ToString() ?? replacementInfo.Value;
                
                // Apply format specifier if present and value is formattable
                if (formatSpecifier is not null && replacementInfo.ConstantValue is IFormattable formattable)
                {
                    return formattable.ToString(formatSpecifier, null);
                }
                
                return literalValue;
            }

            // Create a C# interpolation hole for the expression, with optional format specifier.
            return formatSpecifier is not null
                ? $"{{{replacementInfo.Value}:{formatSpecifier}}}"
                : $"{{{replacementInfo.Value}}}";
        });

        // Escape any double quotes for the C# string literal and return as interpolated string.
        return $"$\"{result.Replace("\"", "\"\"")}\"";
    }

    private static string BuildMessageFromExpression(
        MessageInfo messageTemplate,
        Dictionary<string, ArgumentInfo> placeholderMap)
    {
        var expressionBuilder = new StringBuilder(messageTemplate.Value);

        // Find all placeholders in the template to preserve their format specifiers.
        var matches = PlaceholderRegex.Matches(messageTemplate.Value);
        
        // Process in reverse order to maintain correct string positions when replacing.
        foreach (Match match in matches.Cast<Match>().Reverse())
        {
            var placeholderName = match.Groups[1].Value;
            var formatSpecifier = match.Groups[2].Success ? match.Groups[2].Value : null;
            var placeholderKey = $"{{{placeholderName}}}";

            if (!placeholderMap.TryGetValue(placeholderKey, out var replacementInfo))
            {
                // Unknown placeholder, skip
                continue;
            }

            var valueExpression = replacementInfo.IsLiteral 
                ? $"\"{replacementInfo.Value}\"" 
                : replacementInfo.Value;

            string finalReplacementExpression;
            
            if (formatSpecifier is not null)
            {
                // Use string.Format for formatted values at runtime
                finalReplacementExpression = replacementInfo.Type.PureTypeName.EndsWith("string")
                    ? valueExpression
                    : $"string.Format(\"{{0:{formatSpecifier}}}\", {valueExpression})";
            }
            else
            {
                finalReplacementExpression = replacementInfo.Type.PureTypeName.EndsWith("string")
                    ? valueExpression
                    : $"{valueExpression}?.ToString() ?? \"\"";
            }

            expressionBuilder.Append($".Replace(\"{match.Value}\", {finalReplacementExpression})");
        }

        return expressionBuilder.ToString();
    }
}