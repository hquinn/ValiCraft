namespace ValiCraft.Generator.Concepts;

public record ArgumentInfo(
    string Name,
    string Value,
    string Type,
    bool IsLiteral)
{
    public string SubstitutionalValue =>
        IsLiteral && Type == "string"
            ? Value.Substring(1, Value.Length - 2)
            : Value;
}