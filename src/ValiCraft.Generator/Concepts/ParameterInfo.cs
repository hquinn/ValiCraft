namespace ValiCraft.Generator.Concepts;

public record ParameterInfo(string Name, TypeInfo Type)
{
    public override string ToString()
    {
        return $"{Type.FormattedTypeName} {Name}";
    }
}