using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public record BlockLambdaMustRule(
    string Body,
    string Parameter,
    MessageInfo? DefaultMessage,
    MessageInfo? DefaultErrorCode,
    RuleOverrideData RuleOverrides,
    IfConditionModel IfCondition,
    EquatableArray<RulePlaceholder> Placeholders,
    LocationInfo Location) : Rule(
    EquatableArray<ArgumentInfo>.Empty, 
    DefaultMessage,
    DefaultErrorCode,
    RuleOverrides,
    IfCondition,
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
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);

        var localFunctionName = $"__must_{context.Counter}";
        var localFunction = $$"""
                              {{indent}}bool {{localFunctionName}}({{target.Type.FormattedTypeName}} {{Parameter}})
                              {{indent}}{{Body}}
                              """;
        
        var inlinedCondition = $"{localFunctionName}({targetAccessor})";

        var code = $$"""
                     {{localFunction}}
                     {{IfCondition.GenerateIfBlock(@object, requestName, indent, context)}}!{{inlinedCondition}})
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