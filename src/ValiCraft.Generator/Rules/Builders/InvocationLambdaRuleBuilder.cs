using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public class InvocationLambdaRuleBuilder(
    bool isAsync,
    EquatableArray<ArgumentInfo> arguments,
    MessageInfo? defaultMessage,
    MessageInfo? defaultErrorCode,
    EquatableArray<RulePlaceholder> rulePlaceholders,
    string body,
    LocationInfo location) : RuleBuilder
{
    public static InvocationLambdaRuleBuilder Create(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda,
        GeneratorAttributeSyntaxContext context)
    {
        var innerInvocation = lambda.Body switch
        {
            InvocationExpressionSyntax i => i,
            AwaitExpressionSyntax { Expression: InvocationExpressionSyntax i } => i,
            _ => null
        };
        
        var innerInvocationSymbol = context.SemanticModel.GetSymbolInfo(innerInvocation!).Symbol;

        var fullMethodName = innerInvocation!.Expression.ToString();
        
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = usesCancellationToken ? lambda.GetSecondParameterName() : null;
        
        var rewriter = new LambdaParameterRewriter(parameterName, cancellationTokenParameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);
        var body = rewrittenBody?.ToString() ?? string.Empty;

        if (isAsync && lambda.Body is InvocationExpressionSyntax)
        {
            body = $"await {body}";
        }

        return new InvocationLambdaRuleBuilder(
            isAsync,
            invocation.GetRuleArguments(context.SemanticModel).ToEquatableImmutableArray(),
            MessageInfo.CreateFromAttribute(innerInvocationSymbol, KnownNames.Attributes.DefaultMessageAttribute),
            MessageInfo.CreateFromAttribute(innerInvocationSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute) ?? new MessageInfo(fullMethodName, true),
            RulePlaceholder.CreateFromRulePlaceholderAttributes(innerInvocationSymbol!),
            body,
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new InvocationLambdaRule(
            isAsync,
            arguments,
            body,
            defaultMessage,
            defaultErrorCode,
            GetRuleOverrideData(),
            IfCondition,
            rulePlaceholders,
            location);
    }
}
