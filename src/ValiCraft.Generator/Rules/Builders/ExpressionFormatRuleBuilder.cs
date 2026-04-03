using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class ExpressionFormatRuleBuilder(
    bool isAsync,
    EquatableArray<ArgumentInfo> arguments,
    MessageInfo? defaultMessage,
    MessageInfo? defaultErrorCode,
    EquatableArray<RulePlaceholder> rulePlaceholders,
    string expressionFormat,
    LocationInfo location) : RuleBuilder
{
    public static ExpressionFormatRuleBuilder Create(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        string fullMethodName,
        ISymbol? methodSymbol,
        GeneratorAttributeSyntaxContext context)
    {
        var extraArgs = invocation.ArgumentList.Arguments
            .Skip(1)
            .Select(a => a.ToString())
            .ToList();

        var sb = new StringBuilder($"{(isAsync ? "await " : string.Empty)}{fullMethodName}({{0}}");

        foreach (var arg in extraArgs)
        {
            sb.Append($", {arg}");
        }

        if (usesCancellationToken)
        {
            sb.Append(", cancellationToken");
        }

        sb.Append(")");

        return new ExpressionFormatRuleBuilder(
            isAsync,
            invocation.GetRuleArguments(context.SemanticModel).ToEquatableImmutableArray(),
            MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultMessageAttribute),
            MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute) ?? new MessageInfo(fullMethodName, true),
            RulePlaceholder.CreateFromRulePlaceholderAttributes(methodSymbol!),
            sb.ToString(),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new Rule(
            RuleKind.ExpressionFormat,
            arguments,
            defaultMessage,
            defaultErrorCode,
            GetRuleOverrideData(),
            IfCondition,
            rulePlaceholders,
            location,
            IsAsync: isAsync,
            ExpressionFormat: expressionFormat);
    }
}
