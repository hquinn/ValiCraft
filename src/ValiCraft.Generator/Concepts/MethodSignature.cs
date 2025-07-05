using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Concepts;

public enum SignatureMatching
{
    None,
    Full,
    Partial
}

public record MethodSignature(string MethodName, EquatableArray<ParameterInfo> Parameters)
{
    public SignatureMatching MatchesTypes(EquatableArray<ArgumentInfo> arguments)
    {
        if (Parameters.Count != arguments.Count)
        {
            return SignatureMatching.None;
        }

        var signatureMatching = SignatureMatching.Full;

        for (var i = 0; i < Parameters.Count; i++)
        {
            var parameter = Parameters[i];
            var argument = arguments[i];

            if (TypeComparisonUtils.AreEquivalent(parameter.TypeName, argument.Type))
            {
                continue;
            }

            // If our concrete type doesn't match, then we can return early
            if (!parameter.TypeIsGeneric)
            {
                return SignatureMatching.None;
            }

            signatureMatching = SignatureMatching.Partial;
        }

        return signatureMatching;
    }
}