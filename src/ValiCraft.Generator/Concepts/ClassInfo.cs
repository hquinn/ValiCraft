using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Concepts;

public record ClassInfo(
    string Name,
    string Namespace,
    string Accessibility,
    string Modifiers,
    EquatableArray<GenericParameterInfo> GenericParameters,
    EquatableArray<GenericArgumentInfo> InterfaceGenericParameters,
    string FullyQualifiedWithoutGenerics,
    string FullyQualifiedUnboundedName)
{
    /// <summary>
    /// Gets the class name formatted for use in XML doc cref attributes.
    /// Generic parameters use curly brace syntax: e.g., "MyClass{T, U}"
    /// Uses fully qualified name to avoid ambiguity with method names.
    /// </summary>
    public string XmlDocCref
    {
        get
        {
            if (GenericParameters.Count == 0)
                return FullyQualifiedWithoutGenerics;
            
            var genericParams = string.Join(", ", GenericParameters.Select(p => p.Type.FormattedTypeName));
            return $"{FullyQualifiedWithoutGenerics}{{{genericParams}}}";
        }
    }

    public static ClassInfo CreateFromSyntaxAndSymbols(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol? implementedInterfaceSymbol)
    {
        return new ClassInfo(
            classSymbol.Name,
            classSymbol.GetNamespace(),
            classSymbol.DeclaredAccessibility.ToCSharpKeyword(),
            classDeclarationSyntax.GetFullModifiers(),
            classSymbol.GetClassGenericParameters(implementedInterfaceSymbol),
            implementedInterfaceSymbol.GetImplementedInterfaceGenericArguments(),
            classSymbol.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric),
            classSymbol.GetFullyQualifiedUnboundedName());
    }
}