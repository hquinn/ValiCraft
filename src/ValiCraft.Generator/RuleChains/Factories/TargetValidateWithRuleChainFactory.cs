using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetValidateWithRuleChainFactory : IRuleChainFactory
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
        var validateWithInvocation = invocationChain.Skip(1).First();
        var argumentExpression = validateWithInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

        if (argumentExpression is null)
        {
            return null;
        }

        var validatorExpression = argumentExpression.ToString();
        
        // Determine if the validator argument is an IAsyncValidator
        var typeInfo = context.SemanticModel.GetTypeInfo(argumentExpression);
        var isAsyncValidatorCall = IsAsyncValidatorType(typeInfo.Type);

        return new TargetValidateWithRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            invocation.GetOnFailureModeFromSyntax(),
            validatorExpression,
            isAsyncValidatorCall);
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