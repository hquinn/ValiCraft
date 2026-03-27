using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Rules.Builders;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionTargetRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(
        bool isAsyncValidator,
        ValidationTarget @object,
        ValidationTarget? target,
        InvocationExpressionSyntax invocation,
        List<InvocationExpressionSyntax> invocationChain,
        int depth,
        IndentModel indent,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        // Resolve the element type from the EnsureEach method's TTarget generic argument.
        var elementTypeInfo = GetElementTypeInfo(invocation, context);

        if (elementTypeInfo is null)
        {
            return null;
        }

        // Skip the EnsureEach method as that's not a rule.
        var rules = RuleChainHelper.ProcessRuleInvocations(
            isAsyncValidator, invocationChain.Skip(1), diagnostics, context);
        if (rules is null)
        {
            return null;
        }

        return new CollectionTargetRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count,
            invocation.GetOnFailureModeFromSyntax(),
            elementTypeInfo,
            rules.ToEquatableImmutableArray());
    }

    private static TypeInfo? GetElementTypeInfo(
        InvocationExpressionSyntax invocation,
        GeneratorAttributeSyntaxContext context)
    {
        // Get the method symbol for EnsureEach to extract TTarget (the element type)
        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol)
        {
            return null;
        }

        // EnsureEach<TTarget> — TTarget is the first type argument
        if (methodSymbol.TypeArguments.Length == 0)
        {
            return null;
        }

        var elementType = methodSymbol.TypeArguments[0];

        return new TypeInfo(
            elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            elementType.NullableAnnotation == NullableAnnotation.Annotated);
    }
}
