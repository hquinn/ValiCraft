using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Providers.LitePrimitives.Generator.Concepts;

public record ValidatorInfo(
    ClassInfo Class,
    string RequestTypeName,
    EquatableArray<RuleInvocation> Rules);