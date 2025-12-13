using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

/// <summary>
/// Represents an async Must rule where the lambda body is an invocation (e.g., async (x, ct) => SomeAsyncMethod(x, ct))
/// or an await expression (e.g., async (x, ct) => await CheckAsync(x, ct)).
/// </summary>
public record AsyncInvocationLambdaMustAsyncRule(
    string ExpressionFormat,
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
        var inlinedCondition = string.Format(ExpressionFormat, targetAccessor);

        // The expression format may already contain 'await' if the lambda was like:
        // async (x, ct) => await SomeMethod(x, ct)
        // In this case, inlinedCondition is "await SomeMethod(request.Email, cancellationToken)"
        // We need to extract the expression after await and wrap it properly.
        //
        // Or it may not contain 'await' if the lambda returns Task<bool> directly:
        // async (x, ct) => SomeMethod(x, ct)
        // In this case, we need to await it ourselves.
        
        var trimmedCondition = inlinedCondition.TrimStart();
        string asyncExpression;
        
        if (trimmedCondition.StartsWith("await "))
        {
            // Strip the "await " prefix and wrap with ConfigureAwait
            var expressionWithoutAwait = trimmedCondition.Substring(6); // Skip "await "
            asyncExpression = $"await ({expressionWithoutAwait}).ConfigureAwait(false)";
        }
        else
        {
            // Expression returns Task<bool>, need to add await and ConfigureAwait
            asyncExpression = $"await ({inlinedCondition}).ConfigureAwait(false)";
        }

        var code = $$"""
                     {{IfCondition.GenerateIfBlock(@object, requestName, indent, context)}}!{{asyncExpression}})
                     {{GetErrorCreation(requestName, KnownNames.Targets.Must, indent, target, context)}}
                     """;
        
        context.UpdateIfElseMode();

        return code;
    }

    protected override string GetErrorCode(string validationRuleInvocation)
    {
        if (RuleOverrides.OverrideErrorCode is null)
        {
            return "\"MustAsync\"";
        }
        
        return base.GetErrorCode(validationRuleInvocation);
    }
}
