namespace ValiCraft.Generator.Concepts;

public record ArgumentInfo(
    string Name,
    string Value,
    TypeInfo Type,
    bool IsLiteral,
    object? ConstantValue);