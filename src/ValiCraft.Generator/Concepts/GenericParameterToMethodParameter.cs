using System.Collections.Generic;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Concepts;

public record GenericParameterToMethodParameter
{
    public GenericParameterToMethodParameter(
        GenericParameterInfo genericParameter,
        MethodSignature methodSignature)
    {
        GenericParameter = genericParameter;
        MethodSignature = methodSignature;
        MethodParameterIndexes = GetMethodParameterIndexes(genericParameter, methodSignature);
    }

    public GenericParameterInfo GenericParameter { get; init; }
    public MethodSignature MethodSignature { get; init; }
    public EquatableArray<int> MethodParameterIndexes { get; init; }

    private static EquatableArray<int> GetMethodParameterIndexes(
        GenericParameterInfo genericParameter,
        MethodSignature methodSignature)
    {
        var indexes = new List<int>();

        for (var index = 0; index < methodSignature.Parameters.Count; index++)
        {
            var methodParameter = methodSignature.Parameters[index];

            if (methodParameter.TypeIsGeneric &&
                TypeComparisonUtils.AreEquivalent(methodParameter.TypeName, genericParameter.Name))
            {
                indexes.Add(index);
            }
        }

        return indexes.ToEquatableImmutableArray();
    }
}