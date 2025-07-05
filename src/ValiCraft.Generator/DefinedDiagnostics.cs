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

    public static DiagnosticInfo MissingIsValidMethod(Location location)
    {
        return new DiagnosticInfo(
            new DiagnosticDescriptor(
                "VALC103",
                "Missing IsValid method",
                "Missing IsValid method for Validation Rule",
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
}