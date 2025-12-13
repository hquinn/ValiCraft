using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Utils;

public class LambdaParameterRewriter : CSharpSyntaxRewriter
{
    // We replace the parameter with an identifier that looks like our placeholder.
    private static readonly IdentifierNameSyntax PlaceholderNode = SyntaxFactory.IdentifierName("{0}");
    private readonly string _parameterNameToReplace;

    public LambdaParameterRewriter(string parameterNameToReplace)
    {
        _parameterNameToReplace = parameterNameToReplace;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        // If this identifier's text matches the lambda parameter's name...
        if (node.Identifier.ValueText == _parameterNameToReplace)
        {
            // ...replace it with our placeholder syntax node, preserving trivia (whitespace).
            return PlaceholderNode
                .WithLeadingTrivia(node.GetLeadingTrivia())
                .WithTrailingTrivia(node.GetTrailingTrivia());
        }

        // Otherwise, continue traversing the tree without changing anything.
        return base.VisitIdentifierName(node);
    }
}