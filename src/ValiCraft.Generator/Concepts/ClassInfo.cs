using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public record ContainingTypeInfo(
    string Name,
    string Modifiers);

public record ClassInfo(
    string Name,
    string Namespace,
    string Modifiers,
    EquatableArray<ContainingTypeInfo> ContainingTypes)
{
    public static ClassInfo CreateFromSyntaxAndSymbols(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol)
    {
        var containingTypes = GetContainingTypes(classDeclarationSyntax);
        
        return new ClassInfo(
            classSymbol.Name,
            classSymbol.GetNamespace(),
            classDeclarationSyntax.GetFullModifiers(),
            containingTypes);
    }
    
    private static EquatableArray<ContainingTypeInfo> GetContainingTypes(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var containingTypes = new List<ContainingTypeInfo>();
        var parent = classDeclarationSyntax.Parent;
        
        while (parent is ClassDeclarationSyntax parentClass)
        {
            containingTypes.Add(new ContainingTypeInfo(
                parentClass.Identifier.Text,
                parentClass.GetFullModifiers()));
            parent = parentClass.Parent;
        }
        
        // Reverse so outermost class is first
        containingTypes.Reverse();
        return containingTypes.ToEquatableImmutableArray();
    }
}