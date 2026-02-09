using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains.Factories;

public class PolymorphicRuleChainFactory : IRuleChainFactory
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
        // Parse nullBehavior from Polymorphic() invocation
        var nullBehavior = GetNullBehaviorFromInvocation(invocation, context);
        var failureMode = invocation.GetOnFailureModeFromSyntax();

        var branches = new List<PolymorphicBranch>();
        PolymorphicOtherwiseBranch? otherwiseBranch = null;

        // Process the chain: WhenType<T>().ValidateWith/Allow/Fail().WhenType<T>()...Otherwise()
        var chainIndex = 1; // Skip Polymorphic() itself
        while (chainIndex < invocationChain.Count)
        {
            var currentInvocation = invocationChain[chainIndex];
            var methodName = GetMethodName(currentInvocation);

            if (methodName == KnownNames.Methods.WhenType)
            {
                // Get the derived type from WhenType<TDerived>()
                var derivedType = GetDerivedTypeFromWhenType(currentInvocation, context);
                if (derivedType is null)
                {
                    return null;
                }

                // The next invocation should be ValidateWith, Allow, or Fail
                chainIndex++;
                if (chainIndex >= invocationChain.Count)
                {
                    return null;
                }

                var branchInvocation = invocationChain[chainIndex];
                var branchMethodName = GetMethodName(branchInvocation);

                var branch = CreateBranch(branchMethodName, derivedType, branchInvocation, context);
                if (branch is null)
                {
                    return null;
                }

                branches.Add(branch);
                chainIndex++;
            }
            else if (methodName == KnownNames.Methods.Otherwise)
            {
                // The next invocation should be Allow or Fail
                chainIndex++;
                if (chainIndex >= invocationChain.Count)
                {
                    return null;
                }

                var otherwiseActionInvocation = invocationChain[chainIndex];
                var otherwiseActionMethodName = GetMethodName(otherwiseActionInvocation);

                otherwiseBranch = CreateOtherwiseBranch(otherwiseActionMethodName, otherwiseActionInvocation);
                if (otherwiseBranch is null)
                {
                    return null;
                }

                chainIndex++;
                break; // Otherwise is always last
            }
            else
            {
                // Unexpected method in chain
                return null;
            }
        }

        if (branches.Count == 0)
        {
            return null;
        }

        // Default to Allow if no Otherwise branch specified
        otherwiseBranch ??= new PolymorphicOtherwiseBranch(PolymorphicBranchBehavior.Allow, null);

        return new PolymorphicRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            failureMode,
            nullBehavior,
            branches.ToEquatableImmutableArray(),
            otherwiseBranch);
    }

    private static PolymorphicNullBehavior GetNullBehaviorFromInvocation(
        InvocationExpressionSyntax invocation,
        GeneratorAttributeSyntaxContext context)
    {
        var arguments = invocation.ArgumentList.Arguments;
        
        // Check if there's a nullBehavior argument (could be positional or named)
        foreach (var arg in arguments)
        {
            // Skip lambda expressions (the selector)
            if (arg.Expression is LambdaExpressionSyntax)
            {
                continue;
            }

            // Check if it's the nullBehavior argument
            var argName = arg.NameColon?.Name.Identifier.ValueText;
            if (argName == "nullBehavior")
            {
                return ParseNullBehavior(arg.Expression);
            }

            // Check by type - if it's PolymorphicNullBehavior enum
            var typeInfo = context.SemanticModel.GetTypeInfo(arg.Expression);
            if (typeInfo.Type?.Name == KnownNames.Enums.PolymorphicNullBehavior)
            {
                return ParseNullBehavior(arg.Expression);
            }
        }

        return PolymorphicNullBehavior.Skip; // Default
    }

    private static PolymorphicNullBehavior ParseNullBehavior(ExpressionSyntax expression)
    {
        var text = expression.ToString();
        if (text.Contains("Fail"))
        {
            return PolymorphicNullBehavior.Fail;
        }

        return PolymorphicNullBehavior.Skip;
    }

    private static string? GetMethodName(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Name.Identifier.ValueText;
        }

        return null;
    }

    private static TypeInfo? GetDerivedTypeFromWhenType(
        InvocationExpressionSyntax invocation,
        GeneratorAttributeSyntaxContext context)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return null;
        }

        if (memberAccess.Name is not GenericNameSyntax genericName)
        {
            return null;
        }

        var typeArg = genericName.TypeArgumentList.Arguments.FirstOrDefault();
        if (typeArg is null)
        {
            return null;
        }

        var typeSymbol = context.SemanticModel.GetTypeInfo(typeArg).Type;
        if (typeSymbol is null)
        {
            return null;
        }

        return new TypeInfo(
            typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            typeSymbol.NullableAnnotation == NullableAnnotation.Annotated);
    }

    private static PolymorphicBranch? CreateBranch(
        string? methodName,
        TypeInfo derivedType,
        InvocationExpressionSyntax invocation,
        GeneratorAttributeSyntaxContext context)
    {
        return methodName switch
        {
            KnownNames.Methods.ValidateWith => CreateValidateWithBranch(derivedType, invocation, context),
            KnownNames.Methods.Allow => new PolymorphicBranch(derivedType, PolymorphicBranchBehavior.Allow, null, false, null),
            KnownNames.Methods.Fail => CreateFailBranch(derivedType, invocation),
            _ => null
        };
    }

    private static PolymorphicBranch CreateValidateWithBranch(
        TypeInfo derivedType,
        InvocationExpressionSyntax invocation,
        GeneratorAttributeSyntaxContext context)
    {
        var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
        var validatorExpression = argumentExpression?.ToString() ?? string.Empty;

        var isAsyncValidatorCall = false;
        if (argumentExpression is not null)
        {
            var typeInfo = context.SemanticModel.GetTypeInfo(argumentExpression);
            isAsyncValidatorCall = IsAsyncValidatorType(typeInfo.Type);
        }

        return new PolymorphicBranch(
            derivedType,
            PolymorphicBranchBehavior.ValidateWith,
            validatorExpression,
            isAsyncValidatorCall,
            null);
    }

    private static PolymorphicBranch CreateFailBranch(
        TypeInfo derivedType,
        InvocationExpressionSyntax invocation)
    {
        var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
        MessageInfo? failMessage = null;

        if (argumentExpression is not null)
        {
            failMessage = MessageInfo.CreateFromExpression(argumentExpression);
        }

        return new PolymorphicBranch(
            derivedType,
            PolymorphicBranchBehavior.Fail,
            null,
            false,
            failMessage);
    }

    private static PolymorphicOtherwiseBranch? CreateOtherwiseBranch(
        string? methodName,
        InvocationExpressionSyntax invocation)
    {
        return methodName switch
        {
            KnownNames.Methods.Allow => new PolymorphicOtherwiseBranch(PolymorphicBranchBehavior.Allow, null),
            KnownNames.Methods.Fail => CreateOtherwiseFailBranch(invocation),
            _ => null
        };
    }

    private static PolymorphicOtherwiseBranch CreateOtherwiseFailBranch(InvocationExpressionSyntax invocation)
    {
        var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
        MessageInfo? failMessage = null;

        if (argumentExpression is not null)
        {
            failMessage = MessageInfo.CreateFromExpression(argumentExpression);
        }

        return new PolymorphicOtherwiseBranch(PolymorphicBranchBehavior.Fail, failMessage);
    }

    private static bool IsAsyncValidatorType(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedType)
        {
            return false;
        }

        // Check if the type itself IS IAsyncValidator<T>
        if (namedType.Name == KnownNames.InterfaceNames.IAsyncValidator &&
            namedType.ContainingNamespace.ToDisplayString() == KnownNames.Namespaces.Base)
        {
            return true;
        }

        // Check if the type implements IAsyncValidator<T>
        foreach (var iface in namedType.AllInterfaces)
        {
            if (iface.Name == KnownNames.InterfaceNames.IAsyncValidator &&
                iface.ContainingNamespace.ToDisplayString() == KnownNames.Namespaces.Base)
            {
                return true;
            }
        }

        return false;
    }
}
