using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Rules.Builders;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(
        ValidationTarget @object,
        ValidationTarget? target,
        InvocationExpressionSyntax invocation,
        List<InvocationExpressionSyntax> invocationChain,
        int depth,
        IndentModel indent,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        RuleBuilder? ruleBuilder = null;
        var rules = new List<Rule>();
        var whenNotNull = false;
    
        // Skip the Ensure method as that's not a rule.
        foreach (var ruleInvocation in invocationChain.Skip(1))
        {
            ruleBuilder = ProcessNextInChain(ruleBuilder, ruleInvocation, rules, context, ref whenNotNull);
        }
    
        // Add the last rule into the rule list
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }
        
        // Now that we have all the rules in the chain, we can now create the rule chain
        return new TargetRuleChain(
            @object,
            target!,
            depth,
            indent,
            rules.Count,
            invocation?.GetOnFailureModeFromSyntax(),
            rules.ToEquatableImmutableArray(),
            whenNotNull);
    }
    
    private static RuleBuilder? ProcessNextInChain(
        RuleBuilder? ruleBuilder,
        InvocationExpressionSyntax invocation,
        List<Rule> rules,
        GeneratorAttributeSyntaxContext context,
        ref bool whenNotNull)
    {
        var ruleMemberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
        var memberName = ruleMemberAccess.Name.Identifier.ValueText;
        var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
    
        if (InvocationIsRuleOverride(ruleBuilder, memberName, argumentExpression, invocation, ref whenNotNull))
        {
            return ruleBuilder;
        }
    
        // If we were building a previous rule, then we can add it to the list of rules.
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }
    
        // We are specifically handling Must differently, as we want to generate specific logic for handling
        // the inlining of Must.
        if (memberName == "Must" &&
            invocation.ArgumentList.Arguments.Count == 1)
        {
            switch (argumentExpression)
            {
                case LambdaExpressionSyntax { Body: IsPatternExpressionSyntax or BinaryExpressionSyntax } patternLambda:
                    return PatternLambdaMustRuleBuilder.Create(invocation, patternLambda);
                case LambdaExpressionSyntax { Body: PrefixUnaryExpressionSyntax } prefixUnaryLambda:
                    return PatternLambdaMustRuleBuilder.Create(invocation, prefixUnaryLambda);
                case LambdaExpressionSyntax { Body: BlockSyntax } blockLambda:
                    return BlockLambdaMustRuleBuilder.Create(invocation, blockLambda);
                case LambdaExpressionSyntax { Body: InvocationExpressionSyntax } invocationLambda:
                    return InvocationLambdaMustRuleBuilder.Create(invocation, invocationLambda);
                case IdentifierNameSyntax identifierNameSyntax:
                    return IdentifierNameMustRuleBuilder.Create(invocation, identifierNameSyntax);
            }
        }
    
        // We usually get a value here if the invocation is a validation rule which:
        // 1) Exists in a separate project or the extension method has been manually created
        // 2) The invocation does not follow another invocation which cannot be resolved.
        //    This generally happens when a validation rule is created in the same project as the validator,
        //    and they used the [GenerateRuleValidation] attribute.
        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol methodSymbol)
        {
            return RichSemanticValidationRuleBuilder.Create(methodSymbol, invocation, memberName, context.SemanticModel);
        }
    
        return WeakSemanticValidationRuleBuilder.Create(invocation, memberName, context.SemanticModel);
    }
    
    private static bool InvocationIsRuleOverride(
        RuleBuilder? ruleBuilder,
        string memberName,
        ExpressionSyntax? argumentExpression,
        InvocationExpressionSyntax invocation,
        ref bool whenNotNull)
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
            case "WithTargetName":
                if (argumentExpression is not null)
                {
                    ruleBuilder?.WithTargetName(MessageInfo.CreateFromExpression(argumentExpression));
                }
    
                return true;
            case "WithSeverity":
                if (argumentExpression is not null)
                {
                    ruleBuilder?.WithSeverity(MessageInfo.CreateFromExpression(argumentExpression));
                }
    
                return true;
            case "If":
                if (argumentExpression is not null)
                {
                    ruleBuilder?.WithCondition(invocation);
                }
                
                return true;
            case "WhenNotNull":
                whenNotNull = true;
                return true;
        }
    
        return false;
    }
}