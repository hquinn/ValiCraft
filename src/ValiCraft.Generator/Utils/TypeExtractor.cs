using System.Text.RegularExpressions;

namespace ValiCraft.Generator.Utils;

public static class TypeExtractor
{
    private static readonly Regex NullableTypeRegex = 
        new(@"^(?:global::)?System\.Nullable<(.+)>$", RegexOptions.Compiled);
    
    public static string ToIdiomaticCSharp(string typeName, bool isNullable)
    {
        var match = NullableTypeRegex.Match(typeName);
        if (match.Success)
        {
            var innerType = match.Groups[1].Value;
            return $"{innerType}?";
        }

        if (isNullable)
        {
            if (typeName.EndsWith("?"))
            {
                return typeName;
            }
            
            return $"{typeName}?";
        }

        return typeName;
    }
    
    public static string ToPureTypeName(string typeName)
    {
        var match = NullableTypeRegex.Match(typeName);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        if (typeName.EndsWith("?"))
        {
            return typeName.TrimEnd('?');
        }

        return typeName;
    }
}