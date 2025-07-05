using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.SyntaxProviders;

public static class RuleChainsSyntaxProvider
{
    public static EquatableArray<RuleChain> DiscoverRuleChains(
        ClassDeclarationSyntax classDeclarationSyntax,
        GeneratorAttributeSyntaxContext context)
    {
        // Check to see if we have the DefineRules method, otherwise we can return.
        var defineRulesMethodBody = GetDefineRulesMethodBody(classDeclarationSyntax);

        if (defineRulesMethodBody is null)
        {
            return EquatableArray<RuleChain>.Empty;
        }

        var ruleChains = new List<RuleChain>();

        foreach (var statement in defineRulesMethodBody.Statements.OfType<ExpressionStatementSyntax>())
        {
            GetRuleChainFromStatement(statement, ruleChains, context);
        }

        return ruleChains.ToEquatableImmutableArray();
    }

    private static BlockSyntax? GetDefineRulesMethodBody(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => method.Identifier.ValueText == KnownNames.Methods.DefineRules)?
            .Body;
    }

    private static void GetRuleChainFromStatement(
        ExpressionStatementSyntax statement,
        List<RuleChain> ruleChains,
        GeneratorAttributeSyntaxContext context)
    {
        var invocationChain = GetRuleInvocationsFromStatement(statement);

        if (!TryGetEnsureInvocation(invocationChain, out var ensureInvocation))
        {
            return;
        }

        if (!TryGetPropertyFromEnsureMethod(ensureInvocation!, context, out var property))
        {
            return;
        }

        RuleBuilder? ruleBuilder = null;
        var rules = new List<Rule>();
        
        // Skip the Ensure method as that's not a rule.
        foreach (var ruleInvocation in invocationChain.Skip(1))
        {
            ruleBuilder = ProcessNextInChain(ruleBuilder, ruleInvocation, rules, property!, context);
        }

        // Add the last rule into the rules list
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }
        
        // Now that we have all the rules in the chain, we can now create the rule chain
        ruleChains.Add(new PropertyRuleChain(property!, rules.ToEquatableImmutableArray(), rules.Count));
    }

    private static List<InvocationExpressionSyntax> GetRuleInvocationsFromStatement(ExpressionStatementSyntax statement)
    {
        // Invocation chains will always start at the last method invocation.
        // We want to "climb up" the invocation chain until we reach the first method invocation.
        var invocationChain = new List<InvocationExpressionSyntax>();

        if (statement.Expression is not InvocationExpressionSyntax outermostInvocation)
        {
            return invocationChain;
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
            else
                // We've reached the start of the chain
            {
                break;
            }
        }

        // Since we started at the end of the invocation chain,
        // reverse the list so we get something easier to work with.
        invocationChain.Reverse();

        return invocationChain;
    }

    private static bool TryGetEnsureInvocation(
        List<InvocationExpressionSyntax> invocationChain,
        out InvocationExpressionSyntax? ensureInvocation)
    {
        // We don't have a valid rule chain if we have zero or one method invocations
        // As the first invocation should be the Ensure method.
        if (invocationChain.Count <= 1)
        {
            ensureInvocation = null;
            return false;
        }

        ensureInvocation = invocationChain[0];
        var ensureMemberAccess = ensureInvocation.Expression as MemberAccessExpressionSyntax;

        if (ensureMemberAccess?.Name.Identifier.ValueText != KnownNames.Methods.Ensure)
        {
            return false;
        }

        return true;
    }

    private static bool TryGetPropertyFromEnsureMethod(
        InvocationExpressionSyntax ensureInvocation,
        GeneratorAttributeSyntaxContext context,
        out ArgumentInfo? property)
    {
        var ensureArgument = ensureInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        // Check that the argument is a lambda expression and get the property (e.g., x => x.Property)
        if (ensureArgument is not SimpleLambdaExpressionSyntax { Body: MemberAccessExpressionSyntax propertyAccess })
        {
            property = null;
            return false;
        }

        if (context.SemanticModel.GetSymbolInfo(propertyAccess).Symbol is not IPropertySymbol propertySymbol)
        {
            property = null;
            return false;
        }

        property = new ArgumentInfo(
            "selector",
            propertyAccess.Name.Identifier.ValueText,
            propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            true);

        return true;
    }

    private static RuleBuilder? ProcessNextInChain(
        RuleBuilder? ruleBuilder,
        InvocationExpressionSyntax invocation,
        List<Rule> rules,
        ArgumentInfo property,
        GeneratorAttributeSyntaxContext context)
    {
        var ruleMemberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
        var memberName = ruleMemberAccess.Name.Identifier.ValueText;
        var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        if (InvocationIsRuleOverride(ruleBuilder, memberName, argumentExpression))
        {
            return ruleBuilder;
        }

        // If we were building a previous rule, then we can add it to the list of rules.
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }

        // We usually get a value here if the invocation is a validation rule which:
        // 1) Exists in a separate project or the extension method has been manually created
        // 2) The invocation does not follow another invocation which cannot be resolved.
        //    This generally happens when a validation rule is created in the same project as the validator,
        //    and they used the [GenerateRuleValidation] attribute.
        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol methodSymbol)
        {
            return CreateFromRichSemantics(
                methodSymbol, invocation, memberName, property, context.SemanticModel);
        }

        return CreateFromWeakSemantics(invocation, memberName, property, context.SemanticModel);
    }

    private static bool InvocationIsRuleOverride(
        RuleBuilder? ruleBuilder,
        string memberName,
        ExpressionSyntax? argumentExpression)
    {
        switch (memberName)
        {
            case "WithMessage":
                if (argumentExpression is not null)
                {
                    ruleBuilder?.WithMessage(MessageInfo.CreateFromExpression(argumentExpression));
                }

                return true;
            case "WithErrorCode":
                if (argumentExpression is not null)
                {
                    ruleBuilder?.WithErrorCode(MessageInfo.CreateFromExpression(argumentExpression));
                }

                return true;
            case "WithPropertyName":
                if (argumentExpression is not null)
                {
                    ruleBuilder?.WithPropertyName(MessageInfo.CreateFromExpression(argumentExpression));
                }

                return true;
        }

        return false;
    }

    private static RuleBuilder CreateFromRichSemantics(
        IMethodSymbol methodSymbol,
        InvocationExpressionSyntax invocation,
        string methodName,
        ArgumentInfo property,
        SemanticModel semanticModel)
    {
        var containingType = methodSymbol.ContainingType;

        return new RuleBuilder(
            SemanticMode.RichSemanticMode,
            methodName,
            invocation.GetArguments(methodSymbol, semanticModel, [property]).ToEquatableImmutableArray(),
            MapToValidationRuleData.CreateFromMethodAndAttribute(
                methodSymbol, KnownNames.Attributes.MapToValidationRuleAttribute),
            MessageInfo.CreateFromAttribute(containingType, KnownNames.Attributes.DefaultMessageAttribute),
            RulePlaceholder.CreateFromRulePlaceholderAttributes(containingType),
            LocationInfo.CreateFrom(invocation)!);
    }

    private static RuleBuilder CreateFromWeakSemantics(
        InvocationExpressionSyntax invocation,
        string methodName,
        ArgumentInfo property,
        SemanticModel semanticModel)
    {
        // We can't provide a lot of information from weak semantics currently, however, this can be considered the discovery phase.
        // We'll be able to (hopefully) add all the necessary information when it comes time later in the pipeline
        // when we have access to the generated validation rules (unique to the weak semantics mode).
        return new RuleBuilder(
            SemanticMode.WeakSemanticMode,
            methodName,
            invocation.GetArguments(null, semanticModel, [property]).ToEquatableImmutableArray(),
            null,
            null,
            EquatableArray<RulePlaceholder>.Empty,
            LocationInfo.CreateFrom(invocation)!);
    }
}