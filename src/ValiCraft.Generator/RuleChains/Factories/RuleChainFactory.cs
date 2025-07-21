using System.Collections.Generic;
using System.Linq;
using Humanizer;
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
    TargetValidateWith,
    Collection,
    CollectionValidateWith,
    WithOnFailure,
    If
}

public static class RuleChainFactory
{
    private static readonly Dictionary<RuleChainKind, IRuleChainFactory> RuleChainFactories;

    static RuleChainFactory()
    {
        RuleChainFactories = new Dictionary<RuleChainKind, IRuleChainFactory>
        {
            [RuleChainKind.Target] = new TargetRuleChainFactory(),
            [RuleChainKind.TargetValidateWith] = new TargetValidateWithRuleChainFactory(),
            [RuleChainKind.Collection] = new CollectionRuleChainFactory(),
            [RuleChainKind.CollectionValidateWith] = new CollectionValidateWithRuleChainFactory(),
            [RuleChainKind.WithOnFailure] = new WithOnFailureRuleChainFactory(),
            [RuleChainKind.If] = new IfRuleChainFactory()
        };
    }
    
    public static RuleChain? CreateFromStatement(
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
            out var startingInvocation);
        
        if (ruleChainKind is null)
        {
            return null;
        }

        if (!TryGetValidationTargetsFromStartingChain(
                startingInvocation!,
                context,
                ruleChainKind.Value,
                out var validationObject,
                out var validationTarget))
        {
            return null;
        }

        var factory = GetRuleChainFactory(ruleChainKind.Value);

        return factory.Create(validationObject!, validationTarget, startingInvocation!, invocationChain, depth, indent, diagnostics, context);
    }

    public static IRuleChainFactory GetRuleChainFactory(RuleChainKind ruleChainKind)
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
        out InvocationExpressionSyntax? firstInvocation)
    {
        firstInvocation = invocationChain.FirstOrDefault();

        if (firstInvocation is null)
        {
            return null;
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
                ? secondInvocationIsValidateWith ? RuleChainKind.TargetValidateWith : RuleChainKind.Target 
                : null,
            KnownNames.Methods.EnsureEach => invocationChain.Count > 1
                ? secondInvocationIsValidateWith ? RuleChainKind.CollectionValidateWith : null
                : RuleChainKind.Collection,
            KnownNames.Methods.WithOnFailure => RuleChainKind.WithOnFailure,
            KnownNames.Methods.If => RuleChainKind.If,
            _ => null
        };
    }

    private static bool TryGetValidationTargetsFromStartingChain(
        InvocationExpressionSyntax startingChainInvocation,
        GeneratorAttributeSyntaxContext context,
        RuleChainKind ruleChainKind,
        out ValidationTarget? validationObject,
        out ValidationTarget? validationTarget)
    {
        validationObject = null;
        validationTarget = null;

        if (ruleChainKind is RuleChainKind.WithOnFailure or RuleChainKind.If)
        {
            return GetValidationTargetFromBuilder(
                startingChainInvocation,
                context,
                out validationObject,
                out validationTarget);
        }
        
        var validationTargetArgument = startingChainInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        // We expect a lambda for selecting the validation target
        if (validationTargetArgument is not LambdaExpressionSyntax lambda)
        {
            return false;
        }

        // We need to handle the case where we're trying to do property validation (e.g., x => x.Property)
        if (lambda.Body is MemberAccessExpressionSyntax propertyAccess)
        {
            return HandlePropertyAccessValidationTarget(
                ref validationObject,
                ref validationTarget,
                startingChainInvocation,
                propertyAccess,
                context);
        }

        // We also need to handle the case where we're trying to do object validation (e.g., x => x)
        if (lambda.Body is IdentifierNameSyntax identifierAccess)
        {
            return HandleObjectAccessValidationTarget(
                ref validationObject,
                ref validationTarget,
                lambda,
                identifierAccess, 
                startingChainInvocation, 
                context);
        }
        
        return false;
    }

    private static bool HandlePropertyAccessValidationTarget(
        ref ValidationTarget? validationObject,
        ref ValidationTarget? validationTarget,
        InvocationExpressionSyntax startingChainInvocation,
        MemberAccessExpressionSyntax propertyAccess,
        GeneratorAttributeSyntaxContext context)
    {
        if (context.SemanticModel.GetSymbolInfo(propertyAccess).Symbol is not IPropertySymbol propertySymbol)
        {
            return false;
        }

        if (!GetValidationTargetFromBuilder(
                startingChainInvocation,
                context,
                out validationObject,
                out validationTarget))
        {
            return false;
        }
            
        var targetName = propertyAccess.Name.Identifier.ValueText;
        var fullPropertyPath = propertyAccess.GetFullPropertyPath();
            
        validationTarget = new ValidationTarget(
            AccessorType: AccessorType.Property,
            AccessorExpressionFormat: $"{{0}}.{fullPropertyPath}",
            Type: new TypeInfo(
                propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                propertySymbol.Type.TypeKind == TypeKind.TypeParameter,
                propertySymbol.NullableAnnotation == NullableAnnotation.Annotated),
            DefaultTargetName: new MessageInfo(targetName.Humanize(), true),
            TargetPath: new MessageInfo(fullPropertyPath, true));

        return true;
    }

    private static bool HandleObjectAccessValidationTarget(
        ref ValidationTarget? validationObject,
        ref ValidationTarget? validationTarget,
        LambdaExpressionSyntax lambda,
        IdentifierNameSyntax identifierAccess,
        InvocationExpressionSyntax startingChainInvocation,
        GeneratorAttributeSyntaxContext context)
    {
        if (lambda.GetParameterName() != identifierAccess.Identifier.ValueText)
        {
            return false;
        }

        return GetValidationTargetFromBuilder(
            startingChainInvocation,
            context,
            out validationObject,
            out validationTarget);
    }

    private static bool GetValidationTargetFromBuilder(
        InvocationExpressionSyntax startingChainInvocation,
        GeneratorAttributeSyntaxContext context,
        out ValidationTarget? validationObject,
        out ValidationTarget? validationTarget)
    {
        // Get the TRequest type from the builder that the starting invocation chain is called on.
        if (startingChainInvocation.Expression is not MemberAccessExpressionSyntax startingChainMemberAccess)
        {
            validationObject = null;
            validationTarget = null;
            return false;
        }
            
        var builderTypeInfo = context.SemanticModel.GetTypeInfo(startingChainMemberAccess.Expression);
        if (builderTypeInfo.Type is not INamedTypeSymbol { TypeArguments.Length: > 0 } builderTypeSymbol)
        {
            validationObject = null;
            validationTarget = null;
            return false;
        }
            
        var requestTypeSymbol = builderTypeSymbol.TypeArguments[0];

        var target = new ValidationTarget(
            AccessorType: AccessorType.Object,
            AccessorExpressionFormat: "{0}",
            Type: new TypeInfo(
                requestTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                requestTypeSymbol.TypeKind == TypeKind.TypeParameter,
                requestTypeSymbol.NullableAnnotation == NullableAnnotation.Annotated),
            DefaultTargetName: new MessageInfo(requestTypeSymbol.Name.Humanize(), true),
            TargetPath: new MessageInfo(requestTypeSymbol.Name, true));

        validationObject = target;
        validationTarget = target;
        
        return true;
    }
}