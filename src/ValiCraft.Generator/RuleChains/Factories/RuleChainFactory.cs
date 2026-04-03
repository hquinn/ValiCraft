using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains.Factories;

public enum RuleChainKind
{
    Target,
    TargetValidator,
    TargetWithRulesValidator,
    Collection,
    WithOnFailure,
    If,
    Polymorphic
}

public static class RuleChainFactory
{
    private static readonly Dictionary<RuleChainKind, IRuleChainFactory> RuleChainFactories;

    static RuleChainFactory()
    {
        RuleChainFactories = new Dictionary<RuleChainKind, IRuleChainFactory>
        {
            [RuleChainKind.Target] = new TargetRuleChainFactory(),
            [RuleChainKind.TargetValidator] = new TargetValidatorRuleChainFactory(),
            [RuleChainKind.TargetWithRulesValidator] = new TargetWithRulesValidatorRuleChainFactory(),
            [RuleChainKind.Collection] = new CollectionRuleChainFactory(),
            [RuleChainKind.WithOnFailure] = new WithOnFailureRuleChainFactory(),
            [RuleChainKind.If] = new IfRuleChainFactory(),
            [RuleChainKind.Polymorphic] = new PolymorphicRuleChainFactory(),
        };
    }

    public static RuleChain? CreateFromStatement(
        bool isAsyncValidator,
        ExpressionStatementSyntax statement,
        string builderArgument,
        int depth,
        IndentModel indent,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        if (!TryGetRuleInvocationsFromStatement(statement, builderArgument, diagnostics, out var invocationChain))
        {
            return null;
        }

        var ruleChainKind = TryGetValidStartingInvocation(
            invocationChain,
            out var startingInvocation,
            out var isCollection);

        if (ruleChainKind is null)
        {
            return null;
        }

        if (!ValidationTargetResolver.TryGetValidationTargetsFromStartingChain(
            startingInvocation!,
            context,
            ruleChainKind.Value,
            isCollection,
            out var validationObject,
            out var validationTarget))
        {
            return null;
        }

        var factory = GetRuleChainFactory(ruleChainKind.Value);

        var factoryContext = new RuleChainFactoryContext(
            isAsyncValidator, validationObject!, validationTarget, startingInvocation!, invocationChain, depth, indent, diagnostics, context, isCollection);

        return factory.Create(factoryContext);
    }

    private static IRuleChainFactory GetRuleChainFactory(RuleChainKind ruleChainKind)
    {
        return RuleChainFactories[ruleChainKind];
    }

     private static bool TryGetRuleInvocationsFromStatement(
        ExpressionStatementSyntax statement,
        string expectedBuilderArgument,
        List<DiagnosticInfo> diagnostics,
        out List<InvocationExpressionSyntax> invocationChain)
    {
        // Invocation chains will always start at the last method invocation.
        // We want to "climb up" the invocation chain until we reach the first method invocation.
        invocationChain = new List<InvocationExpressionSyntax>();

        if (statement.Expression is not InvocationExpressionSyntax outermostInvocation)
        {
            return false;
        }

        ExpressionSyntax currentExpression = outermostInvocation;

        // Perform the invocation climb, adding all the method invocations.
        while (currentExpression is InvocationExpressionSyntax currentInvocation)
        {
            invocationChain.Add(currentInvocation);

            if (currentInvocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                currentExpression = memberAccess.Expression;
            }
            else // We've reached the start of the chain
            {
                break;
            }
        }

        // We expect at the very top of the chain is an identifier
        if (currentExpression is IdentifierNameSyntax identifier)
        {
            var actualBuilderArgument = identifier.Identifier.ValueText;

            if (actualBuilderArgument != expectedBuilderArgument)
            {
                diagnostics.Add(DefinedDiagnostics.InvalidBuilderArgumentUsedInScope(
                    expectedBuilderArgument,
                    actualBuilderArgument,
                    identifier.GetLocation()));
                return false;
            }
        }
        else
        {
            // This shouldn't be legal C# anyway, so just return
            return false;
        }

        // Since we started at the end of the invocation chain,
        // reverse the list so we get something easier to work with.
        invocationChain.Reverse();

        return true;
    }

    private static RuleChainKind? TryGetValidStartingInvocation(
        List<InvocationExpressionSyntax> invocationChain,
        out InvocationExpressionSyntax? firstInvocation,
        out bool isCollection)
    {
        firstInvocation = invocationChain.FirstOrDefault();
        isCollection = false;

        if (firstInvocation is null)
        {
            return null;
        }

        var firstMemberAccess = firstInvocation.Expression as MemberAccessExpressionSyntax;
        var firstMethodName = firstMemberAccess?.Name.Identifier.ValueText;

        var secondInvocation = invocationChain.Skip(1).FirstOrDefault();
        var secondInvocationIsValidator = IsValidatorInvocation(secondInvocation);

        // Check if the last invocation is ValidateWith or Validate<T> (for hybrid chains with rules before validate)
        var lastInvocation = invocationChain.LastOrDefault();
        var lastInvocationIsValidator = IsValidatorInvocation(lastInvocation);

        switch (firstMethodName)
        {
            case KnownNames.Methods.Ensure when invocationChain.Count > 1:
                return secondInvocationIsValidator
                    ? RuleChainKind.TargetValidator
                    : lastInvocationIsValidator
                        ? RuleChainKind.TargetWithRulesValidator
                        : RuleChainKind.Target;

            case KnownNames.Methods.EnsureEach when invocationChain.Count > 1:
                isCollection = true;
                return secondInvocationIsValidator
                    ? RuleChainKind.TargetValidator
                    : lastInvocationIsValidator
                        ? RuleChainKind.TargetWithRulesValidator
                        : RuleChainKind.Target;

            case KnownNames.Methods.EnsureEach when invocationChain.Count <= 1:
                return RuleChainKind.Collection;

            case KnownNames.Methods.WithOnFailure:
                return RuleChainKind.WithOnFailure;

            case KnownNames.Methods.If:
                return RuleChainKind.If;

            case KnownNames.Methods.Polymorphic when invocationChain.Count > 1:
                return RuleChainKind.Polymorphic;

            default:
                return null;
        }
    }

    private static bool IsValidatorInvocation(InvocationExpressionSyntax? invocation)
    {
        if (invocation?.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        // Check for ValidateWith(expr)
        if (memberAccess.Name.Identifier.ValueText == KnownNames.Methods.ValidateWith)
        {
            return true;
        }

        // Check for Validate<T>() or ValidateAsync<T>()
        return memberAccess.Name is GenericNameSyntax
        {
            Identifier.ValueText: KnownNames.Methods.Validate or KnownNames.Methods.ValidateAsync
        };
    }

}
