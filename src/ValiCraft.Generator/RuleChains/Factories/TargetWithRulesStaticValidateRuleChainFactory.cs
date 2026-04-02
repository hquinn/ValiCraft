using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetWithRulesStaticValidateRuleChainFactory : IRuleChainFactory
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
        // The last invocation is Validate<T>() — extract the validator type
        var validateInvocation = invocationChain.Last();

        if (validateInvocation.Expression is not MemberAccessExpressionSyntax memberAccess ||
            memberAccess.Name is not GenericNameSyntax genericName ||
            genericName.TypeArgumentList.Arguments.Count == 0)
        {
            return null;
        }

        var validatorTypeArgument = genericName.TypeArgumentList.Arguments[0];
        var validatorTypeName = validatorTypeArgument.ToString();

        // Determine if this is an async validator call (ValidateAsync vs Validate)
        var isAsyncValidatorCall = genericName.Identifier.ValueText == KnownNames.Methods.ValidateAsync;

        // Get the fully qualified name if we can resolve the symbol
        var typeInfo = context.SemanticModel.GetTypeInfo(validatorTypeArgument);
        if (typeInfo.Type is INamedTypeSymbol namedType)
        {
            validatorTypeName = namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        // Process rules from index 1 to N-2 (skip Ensure and Validate<T>)
        var rules = RuleChainHelper.ProcessRuleInvocations(
            isAsyncValidator, invocationChain.Skip(1).Take(invocationChain.Count - 2), diagnostics, context);
        if (rules is null)
        {
            return null;
        }

        return new TargetWithRulesStaticValidateRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            rules.Count + 1, // +1 for the Validate<T>
            invocation.GetOnFailureModeFromSyntax(),
            rules.ToEquatableImmutableArray(),
            validatorTypeName,
            isAsyncValidatorCall);
    }
}
