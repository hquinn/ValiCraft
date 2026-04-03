using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class WithOnFailureRuleChainFactory : IRuleChainFactory
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
        var onFailureArgument = invocation.GetOnFailureModeFromSyntax();

        if (onFailureArgument is null)
        {
            return null;
        }
        
        var ruleChains = RuleChainHelper.CreateChildRuleChains(
            isAsyncValidator, invocation, KnownNames.Methods.WithOnFailure,
            depth, indent, diagnostics, context);
        if (ruleChains is null)
        {
            return null;
        }

        return new WithOnFailureRuleChain(
            isAsyncValidator,
            @object,
            depth,
            indent,
            ruleChains.Sum(x => x.NumberOfRules),
            onFailureArgument,
            ruleChains.ToEquatableImmutableArray());
    }
}