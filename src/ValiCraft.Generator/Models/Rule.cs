using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.RuleChains.Context;
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
    
    public ValidationRule? MapToValidationRule(ValidationTarget target, ValidationRule[] validRules)
    {
        ValidationRule? bestMatchedRule = null;

        foreach (var validRule in validRules.Where(x => x.NameForExtensionMethod == MethodName))
        {
            var argumentTypes = Arguments.Select(x => x.Type).Prepend(target.Type).ToEquatableImmutableArray();
            // We skip the first parameter as this will always be the property type
            var matchesResult = validRule.IsValidSignature.MatchesTypes(argumentTypes);

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
        ValidationTarget target,
        RuleChainContext context)
    {
        const string errorTypeName = $"global::{KnownNames.Types.ValidationError}";
        const string errorSeverity = $"global::{KnownNames.Enums.ErrorSeverity}";
        var targetAccess = string.Format(target.AccessorExpressionFormat, requestName);
        var validationRuleInvocation =
            $"global::{ValidationRuleData?.FullyQualifiedValidationRule}{ConstructValidationRuleGeneric(target)}";

        var isValidCallArgs = new List<string> { targetAccess };
        isValidCallArgs.AddRange(Arguments.GetArray()?.Select(x => x.Value) ?? []);
        var isValidCallArgsString = string.Join(", ", isValidCallArgs);
        var targetNameInfo = RuleOverrides.OverrideTargetName ?? target.DefaultTargetName;

        var code = $$"""
                 {{indent}}{{GetIfElseIfKeyword(context)}} (!{{validationRuleInvocation}}.IsValid({{isValidCallArgsString}}))
                 {{indent}}{
                 {{indent}}    errors ??= new({{context.Counter}});
                 {{indent}}    errors.Add(new {{errorTypeName}}<{{target.Type.FormattedTypeName}}>
                 {{indent}}    {
                 {{indent}}        Code = {{GetValidationErrorCode(validationRuleInvocation)}},
                 {{indent}}        Message = {{GetValidationMessage(requestName, target, targetNameInfo)}},
                 {{indent}}        Severity = {{errorSeverity}}.Error,
                 {{indent}}        TargetName = "{{targetNameInfo.Value}}",
                 {{indent}}        AttemptedValue = {{targetAccess}},
                 {{indent}}        Cause = null,
                 {{indent}}    });
                 {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}}
                 """;
        
        context.UpdateIfElseMode();
        
        return code;
    }

    private string GetIfElseIfKeyword(RuleChainContext context)
    {
        return context.IfElseMode switch
        {
            IfElseMode.ElseIf => "else if",
            _ => "if"
        };
    }

    private string GetGotoLabelIfNeeded(string indent, RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                   {indent}    goto {context.HaltLabel};
                   
                   """;
        }

        return string.Empty;
    }
    
    private IEnumerable<ArgumentInfo> EnrichArguments(ValidationRule matchedRule)
    {
        // Property is not in the argument list, so skip the first one
        for (var i = 1; i < matchedRule.IsValidSignature.Parameters.Count; i++)
        {
            var argument = Arguments[i - 1];
            var parameter = matchedRule.IsValidSignature.Parameters[i];

            yield return argument with { Name = parameter.Name };
        }
    }
    
    private string GetValidationMessage(string requestName, ValidationTarget target, MessageInfo targetNameInfo)
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
        var map = new Dictionary<string, ArgumentInfo>();

        // Add the standard placeholders.
        // We treat them just like any other argument.
        map.Add("{TargetName}",
            new ArgumentInfo(
                "TargetName",
                targetNameInfo.Value,
                new TypeInfo("string", false, false),
                targetNameInfo.IsLiteral,
                null));
        map.Add("{TargetValue}",
            new ArgumentInfo(
                "TargetValue",
                string.Format(target.AccessorExpressionFormat, requestName),
                target.Type,
                false,
                null));

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