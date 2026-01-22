using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.RuleChains;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record Validator(
    bool IsAsync,
    ClassInfo Class,
    SymbolNameInfo RequestTypeName,
    EquatableArray<RuleChain> RuleChains,
    EquatableArray<string> UsingDirectives);