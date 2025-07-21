using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Extensions;

public static class MemberAccessExpressionSyntaxExtensions
{
    public static string GetFullPropertyPath(this MemberAccessExpressionSyntax expression)
    {
        var parts = new Stack<string>();
        ExpressionSyntax currentExpression = expression;

        while (currentExpression is MemberAccessExpressionSyntax memberAccess)
        {
            parts.Push(memberAccess.Name.Identifier.ValueText);
            currentExpression = memberAccess.Expression;
        }

        return string.Join(".", parts);
    }
}