using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Generator.Shared.Extensions;

public static class MethodSymbolExtensions
{
    public static MethodSignature GetMethodSignature(this IMethodSymbol methodSymbol)
    {
        return new MethodSignature(methodSymbol.Name, methodSymbol.GetMethodParameters());
    }

    public static EquatableArray<ParameterInfo> GetMethodParameters(this IMethodSymbol methodSymbol)
    {
        return methodSymbol.Parameters
            .Select(parameter => new ParameterInfo(
                parameter.Type.ToDisplayString(),
                parameter.Name,
                parameter.Type.TypeKind == TypeKind.TypeParameter,
                parameter.IsNullable()))
            .ToEquatableImmutableArray();
    }
}