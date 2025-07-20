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
        var matchedValidationRule = MapToValidationRule(target, validRules);

        if (matchedValidationRule is null)
        {
            var diagnostics =
                DefinedDiagnostics.UnrecognizableRuleInvocation(Location.ToLocation());
            context.ReportDiagnostic(diagnostics.CreateDiagnostic());

            return null;
        }
        
        return EnrichRuleFromValidationRule(matchedValidationRule);
    }
    
    private ValidationRule? MapToValidationRule(ValidationTarget target, ValidationRule[] validRules)
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