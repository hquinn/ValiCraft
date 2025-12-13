using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public record WeakSemanticValidationRule(
    string MethodName,
    MapToValidationRuleData? ValidationRuleData,
    EquatableArray<ArgumentInfo> Arguments,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    RuleOverrideData RuleOverrides,
    IfConditionModel  IfCondition,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location) : InvokedValidationRule(
    MethodName,
    ValidationRuleData,
    Arguments,
    DefaultMessage,
    DefaultErrorCode,
    RuleOverrides,
    IfCondition,
    Placeholders,
    Location)
{
    public override Rule? EnrichRule(
        ValidationTarget target,
        ValidationRule[] validRules,
        SourceProductionContext context)
    {
        var (matchedValidationRule, typeMismatchInfo) = MapToValidationRule(target, validRules);

        if (matchedValidationRule is null)
        {
            // If we found rules with the same name but types didn't match, provide a better error
            if (typeMismatchInfo is not null)
            {
                var diagnostics = DefinedDiagnostics.TypeMismatchForValidationRule(
                    MethodName,
                    typeMismatchInfo.Value.ExpectedType,
                    typeMismatchInfo.Value.ActualType,
                    typeMismatchInfo.Value.Suggestion,
                    Location.ToLocation());
                context.ReportDiagnostic(diagnostics.CreateDiagnostic());
            }
            else
            {
                var diagnostics =
                    DefinedDiagnostics.UnrecognizableRuleInvocation(Location.ToLocation());
                context.ReportDiagnostic(diagnostics.CreateDiagnostic());
            }

            return null;
        }
        
        return EnrichRuleFromValidationRule(matchedValidationRule);
    }
    
    private record struct TypeMismatchInfo(string ExpectedType, string ActualType, string? Suggestion);
    
    private (ValidationRule? Rule, TypeMismatchInfo? TypeMismatch) MapToValidationRule(ValidationTarget target, ValidationRule[] validRules)
    {
        ValidationRule? bestMatchedRule = null;
        TypeMismatchInfo? typeMismatchInfo = null;

        var matchingNameRules = validRules.Where(x => x.NameForExtensionMethod == MethodName).ToList();
        
        foreach (var validRule in matchingNameRules)
        {
            var argumentTypes = Arguments.Select(x => x.Type).Prepend(target.Type).ToEquatableImmutableArray();
            // We skip the first parameter as this will always be the property type
            var matchesResult = validRule.IsValidSignature.MatchesTypes(argumentTypes);

            // Check if we have a direct match with signature types
            // If so, we can return early
            if (matchesResult == SignatureMatching.Full)
            {
                return (validRule, null);
            }

            // We may want to do some rankings in the future
            // if we can determine if a parameter is non-generic and matches
            if (matchesResult == SignatureMatching.Partial)
            {
                bestMatchedRule = validRule;
            }
        }

        // If we found rules with the same name but didn't find a match, it's likely a type mismatch
        if (bestMatchedRule is null && matchingNameRules.Count > 0)
        {
            var firstRule = matchingNameRules[0];
            var expectedType = firstRule.IsValidSignature.Parameters.Count > 0 
                ? firstRule.IsValidSignature.Parameters[0].Type.FormattedTypeName 
                : "unknown";
            
            // Try to provide a helpful suggestion based on the types
            string? suggestion = GetTypeMismatchSuggestion(target.Type.FormattedTypeName, expectedType);
            
            typeMismatchInfo = new TypeMismatchInfo(expectedType, target.Type.FormattedTypeName, suggestion);
        }

        return (bestMatchedRule, typeMismatchInfo);
    }
    
    private static string? GetTypeMismatchSuggestion(string actualType, string expectedType)
    {
        // String rules applied to non-strings
        if (expectedType.Contains("string") && !actualType.Contains("string"))
        {
            return "Consider using a numeric or comparison validation rule instead.";
        }
        
        // Numeric rules applied to strings
        if ((expectedType.Contains("int") || expectedType.Contains("decimal") || expectedType.Contains("double") || 
             expectedType.Contains("float") || expectedType.Contains("long")) && actualType.Contains("string"))
        {
            return "Consider using a string validation rule like HasMinLength or HasMaxLength instead.";
        }
        
        // DateTime rules applied to non-DateTime
        if (expectedType.Contains("DateTime") && !actualType.Contains("DateTime"))
        {
            return "This rule is only valid for DateTime properties.";
        }
        
        // Collection rules applied to non-collections
        if (expectedType.Contains("IEnumerable") && !actualType.Contains("IEnumerable") && 
            !actualType.Contains("List") && !actualType.Contains("[]"))
        {
            return "This rule is only valid for collection properties.";
        }
        
        return null;
    }

    private WeakSemanticValidationRule EnrichRuleFromValidationRule(ValidationRule? matchedRule)
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
            DefaultErrorCode = matchedRule.DefaultErrorCode,
            Placeholders = matchedRule.RulePlaceholders
        };
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
}