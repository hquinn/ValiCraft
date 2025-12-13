using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Rules.Builders;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains.Factories;

/// <summary>
/// Factory for creating rule chains specifically for async validators.
/// Extends the standard factory to support MustAsync calls.
/// </summary>
public static class AsyncRuleChainFactory
{
    public static RuleChain? CreateFromStatement(
        ExpressionStatementSyntax statement,
        string builderArgument,
        int depth,
        IndentModel indent,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        // Use the existing factory - it will handle most cases
        // The key difference is that async validators can use MustAsync
        var ruleChain = RuleChainFactory.CreateFromStatement(
            statement, builderArgument, depth, indent, diagnostics, context);

        if (ruleChain is null)
        {
            return null;
        }

        // Check if any rules in the chain use MustAsync and mark appropriately
        return MarkAsyncRulesInChain(ruleChain);
    }

    private static RuleChain MarkAsyncRulesInChain(RuleChain ruleChain)
    {
        // For now, the existing rule chain works - we'll handle async at code generation time
        // The async source provider will generate async code for all methods
        return ruleChain;
    }
}
