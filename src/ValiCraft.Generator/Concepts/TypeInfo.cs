using System.Linq;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Concepts;

public record TypeInfo
{
    public TypeInfo(
        string typeName,
        bool isGeneric,
        bool isNullable,
        bool isLambda = false)
    {
        PureTypeName = TypeExtractor.ToPureTypeName(typeName);
        FormattedTypeName = TypeExtractor.ToIdiomaticCSharp(typeName, isNullable);
        IsGeneric = isGeneric;
        IsNullable = isNullable;
        IsLambda = isLambda;
    }
    
    public TypeInfo(
        string typeName,
        bool isGeneric) : this(typeName, isGeneric, typeName.EndsWith("?"))
    {
    }
    
    public string PureTypeName { get; }
    public string FormattedTypeName { get; }
    public bool IsGeneric { get; }
    public bool IsNullable { get; }
    public bool IsLambda { get; }

    public bool Matches(TypeInfo? other)
    {
        if (other is null)
        {
            return false;
        }
        
        var thisType = PureTypeName;
        var otherType = other.PureTypeName;
        
        // This is a case where we cannot resolve the lambda and just need to check against a known Func
        if ((thisType.Contains("System.Func") && other.IsLambda) ||
            (otherType.Contains("System.Func") && IsLambda))
        {
            return true;
        }
        
        if (string.IsNullOrEmpty(thisType) || string.IsNullOrEmpty(otherType))
        {
            return false;
        }

        // Compare the normalized spans for equality.
        return thisType.SequenceEqual(otherType);
    }
}