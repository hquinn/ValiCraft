using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Extensions;

public static class TypeSymbolExtensions
{
    public static bool IsAsyncValidatorType(this ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedType)
        {
            return false;
        }

        // Check if the type itself IS IAsyncValidator<T>
        if (namedType.Name == KnownNames.InterfaceNames.IAsyncValidator &&
            namedType.ContainingNamespace.ToDisplayString() == KnownNames.Namespaces.Base)
        {
            return true;
        }

        // Check if the type implements IAsyncValidator<T>
        foreach (var iface in namedType.AllInterfaces)
        {
            if (iface.Name == KnownNames.InterfaceNames.IAsyncValidator &&
                iface.ContainingNamespace.ToDisplayString() == KnownNames.Namespaces.Base)
            {
                return true;
            }
        }

        return false;
    }
}
