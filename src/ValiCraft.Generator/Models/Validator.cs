using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.RuleChains;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record Validator(
    bool IsAsync,
    bool IsStatic,
    ClassInfo Class,
    SymbolNameInfo RequestTypeName,
    EquatableArray<RuleChain> RuleChains,
    EquatableArray<string> UsingDirectives,
    bool IncludeDefaultMetadata);