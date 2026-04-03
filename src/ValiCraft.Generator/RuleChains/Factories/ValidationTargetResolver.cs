using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains.Factories;

internal static class ValidationTargetResolver
{
    // Simple humanize implementation to avoid Humanizer dependency issues
    private static string Humanize(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Add spaces before capital letters that are followed by lowercase letters
        // This preserves acronyms like "SKU" while splitting "OrderNumber" to "Order Number"
        var result = System.Text.RegularExpressions.Regex.Replace(input, "(?<!^)([A-Z])(?=[a-z])", " $1");
        return result;
    }

    internal static bool TryGetValidationTargetsFromStartingChain(
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

        // We also need to handle the case where we're trying to do method validation (e.g., x => x.GetName() or x => x.Calculate(42))
        if (lambda.Body is InvocationExpressionSyntax methodInvocation)
        {
            return HandleMethodInvocationValidationTarget(
                ref validationObject,
                ref validationTarget,
                startingChainInvocation,
                methodInvocation,
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

        string humanizedTargetName = Humanize(targetName);

        validationTarget = new ValidationTarget(
            AccessorType: AccessorType.Property,
            AccessorExpressionFormat: $"{{0}}.{fullPropertyPath}",
            Type: new TypeInfo(
                propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                propertySymbol.NullableAnnotation == NullableAnnotation.Annotated),
            DefaultTargetName: new MessageInfo(humanizedTargetName, true),
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

    private static bool HandleMethodInvocationValidationTarget(
        ref ValidationTarget? validationObject,
        ref ValidationTarget? validationTarget,
        InvocationExpressionSyntax startingChainInvocation,
        InvocationExpressionSyntax methodInvocation,
        GeneratorAttributeSyntaxContext context)
    {
        if (context.SemanticModel.GetSymbolInfo(methodInvocation).Symbol is not IMethodSymbol methodSymbol)
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

        // Extract the method name from the invocation expression
        var methodName = methodSymbol.Name;

        // Reconstruct the invocation with arguments as source text
        var arguments = methodInvocation.ArgumentList.Arguments;
        var argsText = string.Join(", ", arguments.Select(a => a.ToString()));
        var invocationText = $"{methodName}({argsText})";

        string humanizedMethodName = Humanize(methodName);

        validationTarget = new ValidationTarget(
            AccessorType: AccessorType.Method,
            AccessorExpressionFormat: $"{{0}}.{invocationText}",
            Type: new TypeInfo(
                methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                methodSymbol.ReturnType.NullableAnnotation == NullableAnnotation.Annotated),
            DefaultTargetName: new MessageInfo(humanizedMethodName, true),
            TargetPath: new MessageInfo(methodName, true));

        return true;
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

        string humanizedTypeName = Humanize(requestTypeSymbol.Name);

        var target = new ValidationTarget(
            AccessorType: AccessorType.Object,
            AccessorExpressionFormat: "{0}",
            Type: new TypeInfo(
                requestTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                requestTypeSymbol.NullableAnnotation == NullableAnnotation.Annotated),
            DefaultTargetName: new MessageInfo(humanizedTypeName, true),
            TargetPath: new MessageInfo(requestTypeSymbol.Name, true));

        validationObject = target;
        validationTarget = target;

        return true;
    }
}
