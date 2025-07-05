using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.Concepts;

public record MessageInfo(string Value, bool IsLiteral)
{
    public static MessageInfo? CreateFromAttribute(INamedTypeSymbol? symbol, string attribute)
    {
        var message = symbol?.GetAttributeStringArgument(attribute);

        return message is not null ? new MessageInfo(message, true) : null;
    }

    public static MessageInfo CreateFromExpression(ExpressionSyntax expression)
    {
        if (expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
            // It's a string literal, so we capture its raw value.
        {
            return new MessageInfo(literal.Token.ValueText, true);
        }

        // It's any other expression. We capture its source text.
        return new MessageInfo(expression.ToString(), false);
    }

    public override string ToString()
    {
        return IsLiteral ? $"\"{Value}\"" : Value;
    }
}