using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public record InvocationLambdaMustRule(
    string ExpressionFormat,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    RuleOverrideData RuleOverrides,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location) : Rule(
    EquatableArray<ArgumentInfo>.Empty, 
    DefaultMessage,
    DefaultErrorCode,
    RuleOverrides,
    Placeholders,
    Location)
{
    public override Rule EnrichRule(
        ValidationTarget target,
        ValidationRule[] validRules,
        SourceProductionContext context)
    {
        return this;
    }

    public override string GenerateCodeForRule(
        string requestName,
        string indent,
        ValidationTarget target,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);

        var inlinedCondition = string.Format(ExpressionFormat, targetAccessor);

        var code = $$"""
                     {{indent}}{{GetIfElseIfKeyword(context)}} (!{{inlinedCondition}})
                     {{GetErrorCreation(requestName, KnownNames.Targets.Must, indent, target, context)}}
                     """;
        
        context.UpdateIfElseMode();

        return code;
    }

    protected override string GetErrorCode(string validationRuleInvocation)
    {
        if (RuleOverrides.OverrideErrorCode is null)
        {
            return "\"Must\"";
        }
        
        return base.GetErrorCode(validationRuleInvocation);
    }
}