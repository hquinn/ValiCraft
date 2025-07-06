using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
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
    ///     which happens after one that's a weak semantic mode, e.g., builder.Ensure(x => x.Property).Weak().Rich().
    ///     In this case, the compiler can't infer where to locate the extension method for Weak(), which also affects Rich()
    /// </summary>
    WeakSemanticMode
}

public record Rule(
    SemanticMode SemanticMode,
    string MethodName,
    EquatableArray<ArgumentInfo> Arguments,
    MapToValidationRuleData? ValidationRuleData,
    RuleOverrideData RuleOverrides,
    MessageInfo? DefaultMessage,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location)
{
    private const string FallbackMessage = "\"An error has occurred\"";
    
    public ValidationRule? MapToValidationRule(ValidationRule[] validValidationRules)
    {
        ValidationRule? bestMatchedRule = null;

        foreach (var validRule in validValidationRules.Where(x => x.NameForExtensionMethod == MethodName))
        {
            // We skip the first parameter as this will always be the property type
            var matchesResult = validRule.IsValidSignature.MatchesTypes(Arguments);

            // Check if we have a direct match with signature types
            // If so, we can return early
            if (matchesResult == SignatureMatching.Full)
            {
                return validRule;
            }

            // We may want to do some rankings in the future
            // if we can determine if a parameter is non-generic and matches
            if (matchesResult == SignatureMatching.Partial)
            {
                bestMatchedRule = validRule;
            }
        }

        return bestMatchedRule;
    }

    public Rule EnrichRuleFromValidationRule(ValidationRule? matchedRule)
    {
        if (matchedRule is null)
        {
            return this;
        }

        return this with
        {
            Arguments = EnrichArguments(matchedRule).ToEquatableImmutableArray(),
            ValidationRuleData = matchedRule.GetMapToValidationRuleData(),
            DefaultMessage = matchedRule.DefaultMessage,
            Placeholders = matchedRule.RulePlaceholders
        };
    }

    public string GenerateCodeForRule(
        string requestName,
        string indent,
        ArgumentInfo property,
        ref int assignedErrorsCount)
    {
        const string errorTypeName = $"global::{KnownNames.Types.Error}";
        var propertyAccessString = $"{requestName}.{property.Value}";
        var validationRuleInvocation =
            $"global::{ValidationRuleData?.FullyQualifiedValidationRule}{ConstructValidationRuleGeneric()}";

        var isValidCallArgs = new List<string> { propertyAccessString };
        isValidCallArgs.AddRange(Arguments.GetArray()?.Skip(1).Select(x => x.Value) ?? []);
        var isValidCallArgsString = string.Join(", ", isValidCallArgs);

        return $$"""
                 {{indent}}if (!{{validationRuleInvocation}}.IsValid({{isValidCallArgsString}}))
                 {{indent}}{
                 {{indent}}    errors ??= new({{assignedErrorsCount}});
                 {{indent}}    errors.Add({{errorTypeName}}.Validation({{GetValidationErrorCode(validationRuleInvocation)}}, {{GetValidationMessage(requestName, property)}}));
                 {{indent}}}
                 """;
    }
    
    private IEnumerable<ArgumentInfo> EnrichArguments(ValidationRule matchedRule)
    {
        for (var i = 0; i < Arguments.Count; i++)
        {
            var argument = Arguments[i];
            var parameter = matchedRule.IsValidSignature.Parameters[i];

            yield return argument with { Name = parameter.Name };
        }
    }
    
    private string GetValidationMessage(string requestName, ArgumentInfo property)
    {
        var messageInfo = RuleOverrides.OverrideMessage ?? DefaultMessage;
        if (messageInfo is null)
        {
            return FallbackMessage;
        }

        // Build a complete map of all available placeholders for this rule invocation.
        var placeholderMap = BuildPlaceholderMap(requestName, property);

        // Pass the template and the map to the builder.
        return BuildMessage(messageInfo, placeholderMap);
    }

    private Dictionary<string, ArgumentInfo> BuildPlaceholderMap(string requestName, ArgumentInfo property)
    {
        var map = new Dictionary<string, ArgumentInfo>();

        // Add the standard placeholders.
        // We treat them just like any other argument.
        var propertyNameInfo = RuleOverrides.OverridePropertyName ?? new MessageInfo(property.Value, true);
        map.Add("{PropertyName}",
            new ArgumentInfo("PropertyName", propertyNameInfo.Value, "string", propertyNameInfo.IsLiteral));
        map.Add("{PropertyValue}",
            new ArgumentInfo("PropertyValue", $"{requestName}.{property.Value}", property.Type, false));

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

    private string GetValidationErrorCode(string validationRuleInvocation)
    {
        if (RuleOverrides.OverrideErrorCode is null)
        {
            return $"nameof({validationRuleInvocation})";
        }

        if (RuleOverrides.OverrideErrorCode.IsLiteral)
        {
            return $"\"{RuleOverrides.OverrideErrorCode.Value}\"";
        }

        return RuleOverrides.OverrideErrorCode.Value;
    }
    
    private string? ConstructValidationRuleGeneric()
    {
        if (string.IsNullOrEmpty(ValidationRuleData?.ValidationRuleGenericFormat))
        {
            return null;
        }

        var args = Arguments
            .Select(argument => argument.Type)
            .ToArray<object>();

        return string.Format(ValidationRuleData!.ValidationRuleGenericFormat, args);
    }
}