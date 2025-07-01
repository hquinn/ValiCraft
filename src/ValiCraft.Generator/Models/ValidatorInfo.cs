using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record ValidatorInfo(
    ClassInfo Class,
    string RequestTypeName,
    EquatableArray<RuleInvocation> Rules);