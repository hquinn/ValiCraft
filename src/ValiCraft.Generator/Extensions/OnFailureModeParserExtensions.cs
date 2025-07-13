using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.Extensions;

public static class OnFailureModeParserExtensions
{
    public static OnFailureMode? GetOnFailureModeFromSyntax(this InvocationExpressionSyntax invocation)
    {
        foreach (var argument in invocation.ArgumentList.Arguments)
        {
            // We expect the syntax to be a MemberAccessExpression, like 'EnumTypeName.EnumMemberName'.
            if (argument.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                // Check if the left side of the dot looks like our enum's type name.
                // This is a simple string comparison.
                if (memberAccess.Expression.ToString().EndsWith(KnownNames.Enums.OnFailureMode))
                {
                    // Get the name of the member being accessed (e.g., "Halt" or "Continue").
                    var memberName = memberAccess.Name.Identifier.ValueText;

                    switch (memberName)
                    {
                        case "Halt":
                            return OnFailureMode.Halt;
                        case "Continue":
                            return OnFailureMode.Continue;
                    }
                }
            }
        }

        // No argument matching the specific syntax 'OnFailureMode.Member' was found.
        return null;
    }
}