using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator;

public static class DefinedDiagnostics
{
    public static Diagnostic CouldNotFindDeclaredSyntax(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "VALC001",
                "Internal Error",
                "Could not get syntax node for class",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
    
    public static Diagnostic CouldNotFindSymbol(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "VALC002",
                "Internal Error",
                "Could not get symbol for class",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
    
    public static Diagnostic MissingValidationRuleExtensionName(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "VALC101",
                "Missing Validation Rule Extension Name",
                "Missing Validation Rule Extension Name",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
    
    public static Diagnostic MissingIValidationRuleInterface(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "VALC102",
                "Missing IValidationRule interface",
                "Missing ValiCraft.IValidationRule interface on Validation Rule marked with [GenerateRuleExtension]",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
    
    public static Diagnostic MissingIsValidMethod(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "VALC103",
                "Missing IsValid method",
                "Missing IsValid method for Validation Rule",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
    
    public static Diagnostic MissingPartialKeyword(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "VALC201",
                "Missing partial keyword",
                "Missing partial keyword on Validator marked with [GenerateValidator]",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
    
    public static Diagnostic MissingValidatorBaseClass(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "VALC202",
                "Missing Validator base class",
                "Missing Validator base class on Validator marked with [GenerateValidator]",
                "ValiCraft",
                DiagnosticSeverity.Error,
                true),
            location);
    }
}