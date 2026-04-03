using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.Rules;

internal static class MessageBuilder
{
    // Regex to match placeholders with optional format specifiers: {Name} or {Name:format}
    // Captures: Group 1 = placeholder name, Group 2 = format specifier (optional)
    private static readonly Regex PlaceholderRegex = new(@"\{(\w+)(?::([^}]+))?\}", RegexOptions.Compiled);

    internal static Dictionary<string, ArgumentInfo> BuildPlaceholderMap(
        string requestName,
        ValidationTarget target,
        MessageInfo targetNameInfo,
        EquatableArray<RulePlaceholder> placeholders,
        EquatableArray<ArgumentInfo> arguments)
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
        foreach (var placeholder in placeholders)
        {
            var argument = arguments.FirstOrDefault(x => x.Name == placeholder.ParameterName);
            if (argument is not null)
            {
                map.Add(placeholder.PlaceholderName, argument);
            }
        }

        return map;
    }

    internal static string BuildMessage(
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
