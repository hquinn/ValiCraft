using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record Validator(
    ClassInfo Class,
    string RequestTypeName,
    EquatableArray<RuleChain> RuleChains);