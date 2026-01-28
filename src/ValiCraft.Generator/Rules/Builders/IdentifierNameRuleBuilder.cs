using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class IdentifierNameRuleBuilder(
    bool isAsync,
    EquatableArray<ArgumentInfo> arguments,
    MessageInfo? defaultMessage,
    MessageInfo? defaultErrorCode,
    EquatableArray<RulePlaceholder> rulePlaceholders,
    string expressionFormat,
    LocationInfo location) : RuleBuilder
{
    public static IdentifierNameRuleBuilder Create(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        IdentifierNameSyntax identifierNameSyntax,
        GeneratorAttributeSyntaxContext context)
    {
        var identifierNameSymbol = context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol;

        var fullMethodName = identifierNameSyntax.Identifier.ValueText;

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
        
        return new IdentifierNameRuleBuilder(
            isAsync,
            invocation.GetRuleArguments(context.SemanticModel).ToEquatableImmutableArray(),
            MessageInfo.CreateFromAttribute(identifierNameSymbol, KnownNames.Attributes.DefaultMessageAttribute),
            MessageInfo.CreateFromAttribute(identifierNameSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute) ?? new MessageInfo(fullMethodName, true),
            RulePlaceholder.CreateFromRulePlaceholderAttributes(identifierNameSymbol!),
            sb.ToString(),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new IdentifierNameRule(
            isAsync,
            arguments,
            expressionFormat,
            defaultMessage,
            defaultErrorCode,
            GetRuleOverrideData(),
            IfCondition,
            rulePlaceholders,
            location);
    }
}
