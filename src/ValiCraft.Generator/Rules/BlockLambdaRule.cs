using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

public record BlockLambdaRule(
    bool IsAsync,
    string Body,
    string Parameter,
    string? CancellationTokenParameter,
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
    public override string GenerateCodeForRule(
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);

        var localFunctionName = $"__is{(IsAsync ? "Async" : "")}_{context.Counter}";

        var functionParameters = CancellationTokenParameter != null
            ? $"{target.Type.FormattedTypeName} {Parameter}, global::System.Threading.CancellationToken {CancellationTokenParameter}"
            : $"{target.Type.FormattedTypeName} {Parameter}";

        var localFunction = $$"""
                              {{indent}}{{(IsAsync ? "async global::System.Threading.Tasks.Task<bool>" : "bool")}} {{localFunctionName}}({{functionParameters}})
                              {{indent}}{{Body}}
                              """;

        var callArguments = CancellationTokenParameter != null
            ? $"{targetAccessor}, cancellationToken"
            : targetAccessor;

        var inlinedCondition = IsAsync
            ? $"await {localFunctionName}({callArguments})"
            : $"{localFunctionName}({callArguments})";

        var code = $$"""
                     {{localFunction}}
                     {{IfCondition.GenerateIfBlock(@object, requestName, indent, context)}}!{{inlinedCondition}})
                     {{GetErrorCreation(requestName, KnownNames.Targets.Is, indent, target, context)}}
                     """;
        
        context.UpdateIfElseMode();

        return code;
    }

    protected override string GetErrorCode(string validationRuleInvocation)
    {
        if (RuleOverrides.OverrideErrorCode is null)
        {
            return $"\"{KnownNames.Targets.Is}\"";
        }
        
        return base.GetErrorCode(validationRuleInvocation);
    }
}