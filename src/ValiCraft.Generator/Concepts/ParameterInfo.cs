namespace ValiCraft.Generator.Concepts;

public record ParameterInfo(
    string TypeName,
    string Name,
    bool TypeIsGeneric,
    bool IsNullable)
{
    public override string ToString()
    {
        return $"{TypeName} {Name}";
    }
}