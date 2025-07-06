using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains;
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

        var ruleChains = defineRulesMethodBody.Statements
            .OfType<ExpressionStatementSyntax>()
            .Select(statement => GetRuleChainFromStatement(statement, 0, context))
            .OfType<RuleChain>()
            .ToEquatableImmutableArray();

        return ruleChains;
    }

    private static BlockSyntax? GetDefineRulesMethodBody(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => method.Identifier.ValueText == KnownNames.Methods.DefineRules)?
            .Body;
    }

    private static RuleChain? GetRuleChainFromStatement(
        ExpressionStatementSyntax statement,
        int depth,
        GeneratorAttributeSyntaxContext context)
    {
        var invocationChain = GetRuleInvocationsFromStatement(statement);

        var startingInvocationKind = TryGetValidStartingInvocation(invocationChain, out var startingInvocation); 
        if (startingInvocationKind == ChainStartKind.Invalid)
        {
            return null;
        }

        if (!TryGetPropertyFromEnsureMethod(startingInvocation!, context, out var property) &&
            startingInvocationKind != ChainStartKind.Composite)
        {
            return null;
        }

        return startingInvocationKind switch
        {
            ChainStartKind.Ensure => GetPropertyRuleChain(startingInvocation, invocationChain, property, depth, context),
            ChainStartKind.EnsureValidateWith => GetValidateWithRuleChain(startingInvocation, invocationChain, property, depth, false),
            ChainStartKind.EnsureEach => GetInlineCollectionRuleChain(startingInvocation, property, depth, context),
            ChainStartKind.EnsureEachValidateWith => GetValidateWithCollectionRuleChain(startingInvocation, invocationChain, property, depth),
            ChainStartKind.Composite => GetCompositeRuleChain(startingInvocation, depth, context),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static PropertyRuleChain GetPropertyRuleChain(
        InvocationExpressionSyntax? ensureInvocation,
        List<InvocationExpressionSyntax> invocationChain,
        ArgumentInfo? property,
        int depth,
        GeneratorAttributeSyntaxContext context)
    {
        RuleBuilder? ruleBuilder = null;
        var rules = new List<Rule>();

        // Skip the Ensure method as that's not a rule.
        foreach (var ruleInvocation in invocationChain.Skip(1))
        {
            ruleBuilder = ProcessNextInChain(ruleBuilder, ruleInvocation, rules, property!, context);
        }

        // Add the last rule into the rule list
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }
        
        // Now that we have all the rules in the chain, we can now create the rule chain
        return new PropertyRuleChain(
            depth,
            rules.Count,
            ensureInvocation?.GetOnFailureModeFromSyntax(),
            property!,
            rules.ToEquatableImmutableArray());
    }

    private static ValidateWithRuleChain? GetValidateWithRuleChain(
        InvocationExpressionSyntax? ensureInvocation,
        List<InvocationExpressionSyntax> invocationChain,
        ArgumentInfo? property,
        int depth,
        bool fromCollection)
    {
        var validateWithInvocation = invocationChain.Skip(1).First();
        var argumentExpression = validateWithInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        if (argumentExpression is null)
        {
            return null;
        }

        var validatorExpression = argumentExpression.ToString();

        return new ValidateWithRuleChain(
            depth,
            ensureInvocation?.GetOnFailureModeFromSyntax(),
            property!,
            validatorExpression,
            fromCollection);
    }

    private static CollectionRuleChain? GetInlineCollectionRuleChain(
        InvocationExpressionSyntax? ensureInvocation,
        ArgumentInfo? property,
        int depth,
        GeneratorAttributeSyntaxContext context)
    {
        var statements = ensureInvocation!.GetRuleStatementsFromLastArgument();
        var ruleChains = new List<RuleChain>();
        var elementDepth = depth + 1;

        foreach (var statement in statements)
        {
            var ruleChain = GetRuleChainFromStatement(statement, elementDepth, context);

            if (ruleChain is not null)
            {
                ruleChains.Add(ruleChain);
            }
        }

        // If we don't have any rule chains in the collection, then don't bother
        if (ruleChains.Count == 0)
        {
            return null;
        }
        
        return new CollectionRuleChain(
            depth,
            ruleChains.Sum(x => x.NumberOfRules),
            ensureInvocation?.GetOnFailureModeFromSyntax(),
            property!,
            ruleChains.ToEquatableImmutableArray());
    }

    private static CollectionRuleChain? GetValidateWithCollectionRuleChain(
        InvocationExpressionSyntax? ensureInvocation,
        List<InvocationExpressionSyntax> invocationChain,
        ArgumentInfo? property,
        int depth)
    {
        var validateWithRuleChain = GetValidateWithRuleChain(ensureInvocation, invocationChain, property, depth + 1, true);

        if (validateWithRuleChain is null)
        {
            return null;
        }

        var validateWithRuleChainArray = new EquatableArray<RuleChain>([validateWithRuleChain]);
    
        // Create and return the existing ValidateWithRuleChain model.
        return new CollectionRuleChain(
            depth,
            1,
            ensureInvocation?.GetOnFailureModeFromSyntax(),
            property!,
            validateWithRuleChainArray);
    }

    private static CompositeRuleChain? GetCompositeRuleChain(
        InvocationExpressionSyntax? withOnFailureInvocation,
        int depth,
        GeneratorAttributeSyntaxContext context)
    {
        var onFailureArgument = withOnFailureInvocation?.GetOnFailureModeFromSyntax();

        if (onFailureArgument is null)
        {
            return null;
        }
        
        var statements = withOnFailureInvocation?.GetRuleStatementsFromLastArgument() ?? [];
        var ruleChains = new List<RuleChain>();

        foreach (var statement in statements)
        {
            var ruleChain = GetRuleChainFromStatement(statement, depth, context);

            if (ruleChain is not null)
            {
                ruleChains.Add(ruleChain);
            }
        }

        // If we don't have any rule chains, then don't bother
        if (ruleChains.Count == 0)
        {
            return null;
        }
        
        return new CompositeRuleChain(
            depth,
            ruleChains.Sum(x => x.NumberOfRules),
            onFailureArgument,
            ruleChains.ToEquatableImmutableArray());
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
            else // We've reached the start of the chain
            {
                break;
            }
        }

        // Since we started at the end of the invocation chain,
        // reverse the list so we get something easier to work with.
        invocationChain.Reverse();

        return invocationChain;
    }

    private enum ChainStartKind
    {
        Invalid,
        Ensure,
        EnsureValidateWith,
        EnsureEach,
        EnsureEachValidateWith,
        Composite
    }
    
    private static ChainStartKind TryGetValidStartingInvocation(
        List<InvocationExpressionSyntax> invocationChain,
        out InvocationExpressionSyntax? firstInvocation)
    {
        firstInvocation = invocationChain.FirstOrDefault();

        if (firstInvocation is null)
        {
            return ChainStartKind.Invalid;
        }
        
        var firstMemberAccess = firstInvocation.Expression as MemberAccessExpressionSyntax;
        var firstMethodName = firstMemberAccess?.Name.Identifier.ValueText;
        
        var secondInvocation = invocationChain.Skip(1).FirstOrDefault();
        var secondInvocationIsValidateWith = secondInvocation is
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name.Identifier.ValueText: KnownNames.Methods.ValidateWith
            }
        };

        return firstMethodName switch
        {
            // We don't have a valid rule chain if we have zero or one method invocations
            // As the first invocation should be the Ensure method.
            KnownNames.Methods.Ensure => invocationChain.Count > 1 
                ? secondInvocationIsValidateWith ? ChainStartKind.EnsureValidateWith : ChainStartKind.Ensure 
                : ChainStartKind.Invalid,
            KnownNames.Methods.EnsureEach => invocationChain.Count > 1
                ? secondInvocationIsValidateWith ? ChainStartKind.EnsureEachValidateWith : ChainStartKind.Invalid
                : ChainStartKind.EnsureEach,
            KnownNames.Methods.WithOnFailure => ChainStartKind.Composite,
            _ => ChainStartKind.Invalid
        };
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
            return CreateFromRichSemantics(methodSymbol, invocation, memberName, property, context.SemanticModel);
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