using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public enum SignatureMatching
{
    None,
    Full,
    Partial
}

public record MethodSignature(EquatableArray<ParameterInfo> Parameters)
{
    public SignatureMatching MatchesTypes(EquatableArray<TypeInfo> arguments)
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

            if (argument.Matches(parameter.Type))
            {
                continue;
            }

            // If our concrete type doesn't match, then we can return early
            if (!parameter.Type.IsGeneric)
            {
                return SignatureMatching.None;
            }

            signatureMatching = SignatureMatching.Partial;
        }

        return signatureMatching;
    }
}