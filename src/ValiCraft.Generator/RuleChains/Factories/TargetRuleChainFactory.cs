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
        RuleBuilder? ruleBuilder = null;
        var rules = new List<Rule>();
    
        // Skip the Ensure method as that's not a rule.
        foreach (var ruleInvocation in invocationChain.Skip(1))
        {
            var result = ProcessNextInChain(isAsyncValidator, ruleBuilder, ruleInvocation, rules, diagnostics, context);
            
            // We don't have a valid rule, return early
            if (result is null)
            {
                return null;
            }
            
            ruleBuilder = result;
        }
    
        // Add the last rule into the rule list
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }
        
        // Now that we have all the rules in the chain, we can now create the rule chain
        return new TargetRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count,
            invocation?.GetOnFailureModeFromSyntax(),
            rules.ToEquatableImmutableArray());
    }
    
    private static RuleBuilder? ProcessNextInChain(
        bool isAsyncValidator,
        RuleBuilder? ruleBuilder,
        InvocationExpressionSyntax invocation,
        List<Rule> rules,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        var ruleMemberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
        var memberName = ruleMemberAccess.Name.Identifier.ValueText;
        var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
    
        if (InvocationIsRuleOverride(ruleBuilder, memberName, argumentExpression, invocation))
        {
            return ruleBuilder;
        }
    
        // If we were building a previous rule, then we can add it to the list of rules.
        if (ruleBuilder is not null)
        {
            rules.Add(ruleBuilder.Build());
        }
        
        if (memberName is KnownNames.Targets.Is)
        {
            var memberSymbol = context.SemanticModel.GetSymbolInfo(ruleMemberAccess).Symbol;

            var (isMethodAsync, usesCancellationToken) =
                GetIsRuleSignatureInfo(isAsyncValidator, memberSymbol, context.SemanticModel.Compilation);
            switch (argumentExpression)
            {
                // Method groups (e.g. .Is(NotEmpty))
                case IdentifierNameSyntax identifierNameSyntax:
                    return IdentifierNameRuleBuilder.Create(
                        isMethodAsync,
                        usesCancellationToken,
                        invocation,
                        identifierNameSyntax,
                        context);
                
                // Method groups that are accessed (e.g. .Is(Rules.NotEmpty))
                case MemberAccessExpressionSyntax memberAccessExpressionSyntax:
                    return MemberAccessRuleBuilder.Create(
                        isMethodAsync,
                        usesCancellationToken,
                        invocation,
                        memberAccessExpressionSyntax,
                        context);
                
                // Block lambdas (e.g. .Is(x => { return string.IsNullOrEmpty(x) }))
                case LambdaExpressionSyntax { Body: BlockSyntax } blockLambda:
                    return BlockLambdaRuleBuilder.Create(
                        isMethodAsync,
                        usesCancellationToken,
                        invocation,
                        blockLambda);
                
                // Invocations (e.g. .Is(x => NotEmpty(x), .Is(async x => await NotEmpty(x))
                // Note: .Is(x => !NotEmpty(x)) is not an invocation lambda
                case LambdaExpressionSyntax { Body: InvocationExpressionSyntax or AwaitExpressionSyntax } invocationLambda:
                    return InvocationLambdaRuleBuilder.Create(
                        isMethodAsync,
                        usesCancellationToken,
                        invocation,
                        invocationLambda,
                        context);

                // Everything else (Binary, Unary, Patterns, Properties, Arrays, Parentheses, etc)
                // If it's a lambda with an expression body that isn't a method call, it goes here
                case LambdaExpressionSyntax lambda:
                    return LambdaRuleBuilder.Create(
                        isMethodAsync,
                        usesCancellationToken,
                        invocation,
                        lambda);
            }
        }
    
        // We usually get a value here if the invocation is a validation rule the extension method has been created for
        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol methodSymbol)
        {
            return ExtensionMethodRuleBuilder.Create(methodSymbol, invocation, diagnostics, context.SemanticModel);
        }

        // We don't have something valid here
        diagnostics.Add(DefinedDiagnostics.InvalidRuleInvocation(invocation.GetLocation()));
        return null;
    }

    private static (bool IsAsync, bool UsesCancellationToken) GetIsRuleSignatureInfo(
        bool isAsyncValidator,
        ISymbol? memberSymbol,
        Compilation compilation)
    {
        if (!isAsyncValidator)
        {
            return (false, false);
        }
        
        if (memberSymbol is not IMethodSymbol methodSymbol)
        {
            return (false, false);
        }

        var ruleParameter = methodSymbol.Parameters.FirstOrDefault();
        if (ruleParameter?.Type is not INamedTypeSymbol
            {
                TypeKind: TypeKind.Delegate,
                DelegateInvokeMethod: { } invokeMethod
            })
        {
            return (false, false);
        }

        var cancellationTokenType = compilation.GetTypeByMetadataName("System.Threading.CancellationToken");
        var taskType = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

        var usesCancellationToken = cancellationTokenType != null &&
            invokeMethod.Parameters.Any(parameter =>
                SymbolEqualityComparer.Default.Equals(parameter.Type, cancellationTokenType));

        var isAsync = taskType != null &&
            invokeMethod.ReturnType is INamedTypeSymbol returnType &&
            SymbolEqualityComparer.Default.Equals(returnType.OriginalDefinition, taskType) &&
            returnType.TypeArguments.Length == 1 &&
            returnType.TypeArguments[0].SpecialType == SpecialType.System_Boolean;

        return (isAsync, usesCancellationToken);
    }
    
    private static bool InvocationIsRuleOverride(
        RuleBuilder? ruleBuilder,
        string memberName,
        ExpressionSyntax? argumentExpression,
        InvocationExpressionSyntax invocation)
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
            case "WithMetadata":
                if (invocation.ArgumentList.Arguments.Count >= 2)
                {
                    var keyArg = invocation.ArgumentList.Arguments[0].Expression;
                    var valueArg = invocation.ArgumentList.Arguments[1].Expression;
                    
                    var keyInfo = MessageInfo.CreateFromExpression(keyArg);
                    var valueInfo = MessageInfo.CreateFromExpression(valueArg);
                    
                    if (keyInfo is not null && valueInfo is not null && keyInfo.IsLiteral)
                    {
                        ruleBuilder?.WithMetadata(new MetadataEntry(
                            keyInfo.Value,
                            valueInfo.Value,
                            GetValueType(valueArg),
                            valueInfo.IsLiteral));
                    }
                }
    
                return true;
            case "If":
                if (argumentExpression is not null)
                {
                    ruleBuilder?.WithCondition(invocation);
                }
                
                return true;
        }
    
        return false;
    }

    private static string GetValueType(ExpressionSyntax expression)
    {
        return expression switch
        {
            LiteralExpressionSyntax literal => literal.Token.Value switch
            {
                string => "string",
                int => "int",
                uint => "uint",
                long => "long",
                ulong => "ulong",
                short => "short",
                ushort => "ushort",
                byte => "byte",
                sbyte => "sbyte",
                double => "double",
                float => "float",
                decimal => "decimal",
                bool => "bool",
                _ => "object"
            },
            _ => "object"
        };
    }
}
