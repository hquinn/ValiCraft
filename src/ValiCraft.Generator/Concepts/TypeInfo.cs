using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Concepts;

public record TypeInfo
{
    public TypeInfo(string typeName, bool isNullable)
    {
        PureTypeName = TypeExtractor.ToPureTypeName(typeName);
        FormattedTypeName = TypeExtractor.ToIdiomaticCSharp(typeName, isNullable);
    }

    public string PureTypeName { get; }
    public string FormattedTypeName { get; }
}