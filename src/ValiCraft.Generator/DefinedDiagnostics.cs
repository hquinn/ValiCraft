using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator;

public static class DefinedDiagnostics
{
    public static DiagnosticInfo CouldNotFindDeclaredSyntax(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC001",
                "Internal Error",
                "Could not get syntax node for class",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo CouldNotFindSymbol(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC002",
                "Internal Error",
                "Could not get symbol for class",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo MissingPartialKeyword(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC201",
                "Missing partial keyword",
                "Missing partial keyword on Validator marked with [GenerateValidator]",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo MissingValidatorBaseClass(bool isAsync, Location location)
    {
        var validatorBaseClass = isAsync ? KnownNames.ClassNames.AsyncValidator : KnownNames.ClassNames.Validator;
        var generateValidator = isAsync ? KnownNames.AttributeNames.AsyncGenerateValidator : KnownNames.AttributeNames.GenerateValidator;
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC202",
                $"Missing {validatorBaseClass} base class",
                $"Missing {validatorBaseClass} base class on Validator marked with [{generateValidator}]",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo InvalidRuleInvocation(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC203",
                "Invalid rule invocation",
                $"Invalid rule invocation. Either use the .Is() method or define an extension method.",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo InvalidLambdaDefined(string ruleChainName, Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC204",
                "Invalid lambda defined",
                $"{ruleChainName} expects a lambda as the last parameter.",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    // Not sure how to test this scenario, but is there as a fallback
    public static DiagnosticInfo MissingLambdaParameterName(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC205",
                "Invalid lambda defined",
                "Cannot retrieve the parameter name from lambda definition.",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo InvalidBuilderArgumentUsedInScope(
        string expectedBuilderArgument,
        string actualBuilderArgument,
        Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC206",
                "Invalid builder argument",
                $"'{actualBuilderArgument}' cannot be used in this scope. Try using '{expectedBuilderArgument}'.",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo MissingMapToValidationRuleAttribute(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC207",
                "Missing MapToValidationRule attribute on extenstion method",
                "Missing MapToValidationRule attribute on extenstion method.",
                KnownNames.Namespaces.Base,
                DiagnosticSeverity.Error,
                true),
            location);
    }
}