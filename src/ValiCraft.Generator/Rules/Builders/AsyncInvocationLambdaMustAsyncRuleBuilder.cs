using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

/// <summary>
/// Builder for async MustAsync rules where the lambda body is an invocation or await expression.
/// Handles lambdas like: async (email, ct) => await CheckEmailAsync(email, ct)
/// </summary>
public class AsyncInvocationLambdaMustAsyncRuleBuilder(
    string body,
    LocationInfo location) : RuleBuilder
{
    public static AsyncInvocationLambdaMustAsyncRuleBuilder? Create(
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterNames = lambda.GetParameterNames();
        
        // MustAsync lambdas should have exactly 2 parameters: value and cancellation token
        if (parameterNames.Count != 2)
        {
            return null;
        }
        
        var valueParameterName = parameterNames[0];
        var cancellationTokenParameterName = parameterNames[1];
        
        var rewriter = new AsyncLambdaParameterRewriter(valueParameterName, cancellationTokenParameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);

        return new AsyncInvocationLambdaMustAsyncRuleBuilder(
            rewrittenBody.ToString(),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new AsyncInvocationLambdaMustAsyncRule(
            body,
            new MessageInfo("'{TargetName}' doesn't satisfy the async condition", true),
            new MessageInfo("MustAsync", true),
            GetRuleOverrideData(),
            IfCondition,
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}
