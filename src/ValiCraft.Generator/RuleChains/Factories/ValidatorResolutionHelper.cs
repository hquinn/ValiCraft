using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

internal record ValidatorResolution(
    string ValidatorCallTarget,
    bool IsAsyncValidatorCall,
    bool HoistValidator);

internal static class ValidatorResolutionHelper
{
    internal static ValidatorResolution? Resolve(
        InvocationExpressionSyntax validatorInvocation,
        GeneratorAttributeSyntaxContext context)
    {
        if (validatorInvocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name is GenericNameSyntax genericName &&
            genericName.TypeArgumentList.Arguments.Count > 0)
        {
            // StaticValidate path: Validate<TValidator>() or ValidateAsync<TValidator>()
            var validatorTypeArgument = genericName.TypeArgumentList.Arguments[0];
            var validatorCallTarget = validatorTypeArgument.ToString();

            var isAsyncValidatorCall = genericName.Identifier.ValueText == KnownNames.Methods.ValidateAsync;

            // Get the fully qualified name if we can resolve the symbol
            var typeInfo = context.SemanticModel.GetTypeInfo(validatorTypeArgument);
            if (typeInfo.Type is INamedTypeSymbol namedType)
            {
                validatorCallTarget = namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }

            return new ValidatorResolution(validatorCallTarget, isAsyncValidatorCall, HoistValidator: false);
        }
        else
        {
            // ValidateWith path: ValidateWith(validator)
            var argumentExpression = validatorInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

            if (argumentExpression is null)
            {
                return null;
            }

            var validatorCallTarget = argumentExpression.ToString();

            // Determine if the validator argument is an IAsyncValidator
            var typeInfo = context.SemanticModel.GetTypeInfo(argumentExpression);
            var isAsyncValidatorCall = typeInfo.Type.IsAsyncValidatorType();

            return new ValidatorResolution(validatorCallTarget, isAsyncValidatorCall, HoistValidator: true);
        }
    }
}
