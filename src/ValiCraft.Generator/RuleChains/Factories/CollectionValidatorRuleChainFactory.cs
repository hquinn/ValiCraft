using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionValidatorRuleChainFactory : IRuleChainFactory
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
        var validatorInvocation = invocationChain.Skip(1).First();

        string validatorCallTarget;
        bool isAsyncValidatorCall;
        bool hoistValidator;

        if (validatorInvocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name is GenericNameSyntax genericName &&
            genericName.TypeArgumentList.Arguments.Count > 0)
        {
            // StaticValidate path: Validate<TValidator>() or ValidateAsync<TValidator>()
            var validatorTypeArgument = genericName.TypeArgumentList.Arguments[0];
            validatorCallTarget = validatorTypeArgument.ToString();

            isAsyncValidatorCall = genericName.Identifier.ValueText == KnownNames.Methods.ValidateAsync;

            // Get the fully qualified name if we can resolve the symbol
            var typeInfo = context.SemanticModel.GetTypeInfo(validatorTypeArgument);
            if (typeInfo.Type is INamedTypeSymbol namedType)
            {
                validatorCallTarget = namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }

            hoistValidator = false;
        }
        else
        {
            // ValidateWith path: ValidateWith(validator)
            var argumentExpression = validatorInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

            if (argumentExpression is null)
            {
                return null;
            }

            validatorCallTarget = argumentExpression.ToString();

            // Determine if the validator argument is an IAsyncValidator
            var typeInfo = context.SemanticModel.GetTypeInfo(argumentExpression);
            isAsyncValidatorCall = typeInfo.Type.IsAsyncValidatorType();

            hoistValidator = true;
        }

        return new CollectionValidatorRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            invocation.GetOnFailureModeFromSyntax(),
            validatorCallTarget,
            isAsyncValidatorCall,
            HoistValidator: hoistValidator);
    }
}
