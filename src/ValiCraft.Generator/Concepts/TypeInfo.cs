using System.Linq;

namespace ValiCraft.Generator.Concepts;

public record TypeInfo(
    string TypeName,
    bool IsGeneric,
    bool IsNullable,
    bool IsLambda = false)
{
    public TypeInfo(
        string typeName,
        bool isGeneric) : this(typeName, isGeneric, typeName.EndsWith("?"))
    {
    }

    public bool Matches(TypeInfo? other)
    {
        if (other is null)
        {
            return false;
        }
        
        var thisType = TypeName;
        var otherType = other.TypeName;
        
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

        // Normalize both strings by trimming any trailing '?' character.
        if (IsNullable)
        {
            thisType = thisType.Substring(0, thisType.Length - 1);
        }

        if (other.IsNullable)
        {
            otherType = otherType.Substring(0, otherType.Length - 1);
        }

        // Compare the normalized spans for equality.
        return thisType.SequenceEqual(otherType);
    }
}