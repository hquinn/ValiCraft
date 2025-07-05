using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public enum SemanticMode
{
    /// <summary>
    ///     Rich Semantic Mode is when we're able to get semantic information about the validation rule extension method
    ///     from the invocation in the validator. This will typically happen when the validation rule extension method
    ///     exists in a separate project or the extension method has manually been created.
    /// </summary>
    RichSemanticMode,

    /// <summary>
    ///     Weak Semantic Mode is when we're not able to get semantic information about the validation rule extension method
    ///     from the invocation in the validator. This will happen when the validation rule that has
    ///     the [GenerateRuleExtension] is in the same project as the validator. This can also happen when a rule invocation
    ///     which happens after one that's a weak semantic mode, e.g. builder.Ensure(x => x.Property).Weak().Rich().
    ///     In this case, the compiler can't infer where to locate the extension method for Weak(), which also affects Rich()
    /// </summary>
    WeakSemanticMode
}

public record Rule(
    SemanticMode SemanticMode,
    ArgumentInfo Property,
    string MethodName,
    EquatableArray<ArgumentInfo> Arguments,
    MapToValidationRuleData? ValidationRuleData,
    RuleOverrideData RuleOverrides,
    MessageInfo? DefaultMessage,
    EquatableArray<RulePlaceholderInfo> Placeholders)
{
    private const string FallbackMessage = "\"An error has occurred\"";

    public string GetValidationMessage()
    {
        var messageInfo = RuleOverrides.OverrideMessage ?? DefaultMessage;
        if (messageInfo is null)
        {
            return FallbackMessage;
        }

        // Build a complete map of all available placeholders for this rule invocation.
        var placeholderMap = BuildPlaceholderMap();

        // Pass the template and the map to the builder.
        return BuildMessage(messageInfo, placeholderMap);
    }

    private Dictionary<string, ArgumentInfo> BuildPlaceholderMap()
    {
        var map = new Dictionary<string, ArgumentInfo>();

        // Add the standard placeholders.
        // We treat them just like any other argument.
        var propertyNameInfo = RuleOverrides.OverridePropertyName ?? new MessageInfo(Property.Value, true);
        map.Add("{PropertyName}",
            new ArgumentInfo("PropertyName", propertyNameInfo.Value, "string", propertyNameInfo.IsLiteral));
        map.Add("{PropertyValue}",
            new ArgumentInfo("PropertyValue", $"request.{Property.Value}", Property.Type, false));

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
        // Case 1: The message template is a LITERAL.
        // We can do maximum compile-time optimization and generate an efficient interpolated string.
        if (messageTemplate.IsLiteral)
        {
            return BuildMessageFromLiteral(messageTemplate, placeholderMap);
        }

        // Case 2: The message template is a DYNAMIC EXPRESSION.
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
                ? replacementInfo.Value
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

            var finalReplacementExpression = replacementInfo.Type.EndsWith("string")
                ? valueExpression
                : $"{valueExpression}?.ToString() ?? \"\"";

            expressionBuilder.Append($".Replace(\"{placeholderText}\", {finalReplacementExpression})");
        }

        return expressionBuilder.ToString();
    }
}