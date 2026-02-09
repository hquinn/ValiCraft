using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public class LambdaRuleBuilder(
    bool isAsync,
    string expressionFormat,
    LocationInfo location) : RuleBuilder
{
    public static LambdaRuleBuilder Create(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = usesCancellationToken ? lambda.GetSecondParameterName() : null;

        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName, cancellationTokenParameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);
        var body = rewrittenBody?.ToString() ?? string.Empty;

        if (isAsync)
        {
            // Check if the rewritten body contains an await expression by parsing it
            var hasAwait = rewrittenBody?.DescendantNodesAndSelf()
                .Any(n => n.IsKind(SyntaxKind.AwaitExpression)) ?? false;

            if (!hasAwait)
            {
                body = $"await {body}";
            }
        }

        return new LambdaRuleBuilder(
            isAsync,
            body,
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new LambdaRule(
            isAsync,
            expressionFormat,
            null,
            new MessageInfo(KnownNames.Targets.Is, true),
            GetRuleOverrideData(),
            IfCondition,
            EquatableArray<RulePlaceholder>.Empty,
            location);
    }
}
