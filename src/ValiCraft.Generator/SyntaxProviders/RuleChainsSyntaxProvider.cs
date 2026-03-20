using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains;
using ValiCraft.Generator.RuleChains.Factories;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.SyntaxProviders;

public static class RuleChainsSyntaxProvider
{
    public static EquatableArray<RuleChain> DiscoverRuleChains(
        bool isAsyncValidator,
        List<DiagnosticInfo> diagnostics,
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        GeneratorAttributeSyntaxContext context)
    {
        if (!TryGetDefineRulesMethodSyntax(classDeclarationSyntax, out var defineRulesMethodSyntax) ||
            defineRulesMethodSyntax?.Body?.Statements is null ||
            !TryGetDefineRulesMethodSymbol(classSymbol, out var defineRulesMethodSymbol))
        {
            return EquatableArray<RuleChain>.Empty;
        }

        var builderArgument = defineRulesMethodSymbol!.Parameters.First().Name;

        var statements = defineRulesMethodSyntax.Body!.Statements;

        foreach (var statement in statements)
        {
            if (statement is ExpressionStatementSyntax)
            {
                continue;
            }

            var statementKind = statement switch
            {
                LocalDeclarationStatementSyntax => "Local variable declarations",
                IfStatementSyntax => "If statements",
                ForStatementSyntax => "For statements",
                ForEachStatementSyntax => "ForEach statements",
                WhileStatementSyntax => "While statements",
                DoStatementSyntax => "Do-while statements",
                SwitchStatementSyntax => "Switch statements",
                TryStatementSyntax => "Try statements",
                ThrowStatementSyntax => "Throw statements",
                ReturnStatementSyntax => "Return statements",
                UsingStatementSyntax => "Using statements",
                LockStatementSyntax => "Lock statements",
                _ => $"{statement.Kind()} statements"
            };

            diagnostics.Add(DefinedDiagnostics.InvalidStatementInDefineRules(statementKind, statement.GetLocation()));
        }

        var ruleChains = statements
            .OfType<ExpressionStatementSyntax>()
            .Select(statement => RuleChainFactory.CreateFromStatement(isAsyncValidator, statement, builderArgument, 0, IndentModel.CreateNew(), diagnostics, context))
            .Where(ruleChain => ruleChain is not null)
            .OfType<RuleChain>()
            .ToEquatableImmutableArray();

        return ruleChains;
    }

    private static bool TryGetDefineRulesMethodSyntax(
        ClassDeclarationSyntax classDeclarationSyntax, 
        out MethodDeclarationSyntax? defineRulesMethodSyntax)
    {
        defineRulesMethodSyntax = classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => method.Identifier.ValueText == KnownNames.Methods.DefineRules);
        
        return defineRulesMethodSyntax is not null;
    }
    
    private static bool TryGetDefineRulesMethodSymbol(
        INamedTypeSymbol classSymbol,
        out MethodSignature? defineRulesMethodSignature)
    {
        var defineRulesMethod = classSymbol.GetMembers(KnownNames.Methods.DefineRules)
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m is { IsOverride: true, DeclaredAccessibility: Accessibility.Protected, Parameters.Length: 1 });

        if (defineRulesMethod is null)
        {
            // No need for diagnostics, as this is not valid C# code anyway
            defineRulesMethodSignature = null;
            return false;
        }

        defineRulesMethodSignature = defineRulesMethod.GetMethodSignature();
        return true;
    }
}