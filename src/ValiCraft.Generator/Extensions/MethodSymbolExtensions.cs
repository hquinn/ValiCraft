using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.Extensions;

public static class MethodSymbolExtensions
{
    public static MethodSignature GetMethodSignature(this IMethodSymbol methodSymbol)
    {
        return new MethodSignature(methodSymbol.GetMethodParameters());
    }

    public static EquatableArray<ParameterInfo> GetMethodParameters(this IMethodSymbol methodSymbol)
    {
        return methodSymbol.Parameters
            .Select(parameter => new ParameterInfo(
                parameter.Name,
                new TypeInfo(parameter.Type.ToDisplayString(), parameter.IsNullable())))
            .ToEquatableImmutableArray();
    }
}