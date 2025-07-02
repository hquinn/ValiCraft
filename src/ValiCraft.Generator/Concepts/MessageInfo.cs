using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.Concepts;

public record MessageInfo(string Value, bool IsLiteral)
{
    public static MessageInfo? CreateFromAttribute(INamedTypeSymbol? symbol, string attribute)
    {
        var message = symbol?.GetAttributeStringArgument(attribute);
        
        return message is not null ? new MessageInfo(message, true) : null;
    }
    
    public override string ToString()
    {
        return IsLiteral ? $"\"{Value}\"" : Value;
    }
}