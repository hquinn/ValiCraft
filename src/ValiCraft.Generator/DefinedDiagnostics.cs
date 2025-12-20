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
                "ValiCraft",
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
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo MissingValidationRuleExtensionName(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC101",
                "Missing Validation Rule Extension Name",
                "Missing Validation Rule Extension Name",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo MissingIValidationRuleInterface(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC102",
                "Missing IValidationRule interface",
                "Missing ValiCraft.IValidationRule interface on Validation Rule marked with [GenerateRuleExtension]",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo InvalidRulePlaceholderConstructorArgument(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC103",
                "Invalid Rule Placeholder Constructor Argument",
                $"Placeholder constructor argument must be a string literal.",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo InvalidRulePlaceholderParameterName(
        string parameterName,
        Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC104",
                "Invalid Rule Placeholder Parameter Name",
                $"Parameter name '{parameterName}' is invalid. It must match a parameter name from the IsValid method.",
                "ValiCraft",
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
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo MissingValidatorBaseClass(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC202",
                "Missing Validator base class",
                "Missing Validator base class on Validator marked with [GenerateValidator]",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo UnrecognizableRuleInvocation(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC203",
                "Unrecognizable Rule Invocation",
                "Rule cannot be mapped to a validation rule. Try moving the rule out of the invocation chain.",
                "ValiCraft",
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
                "ValiCraft",
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
                "ValiCraft",
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
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }

    public static DiagnosticInfo TypeMismatchForValidationRule(
        string ruleName,
        string expectedType,
        string actualType,
        string? suggestion,
        Location location)
    {
        var message = $"'{ruleName}' expects '{expectedType}' but property is of type '{actualType}'.";
        if (!string.IsNullOrEmpty(suggestion))
        {
            message += $" {suggestion}";
        }

        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC207",
                "Type mismatch for validation rule",
                message,
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
}