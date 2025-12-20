using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Utils;

public class LambdaParameterRewriter : CSharpSyntaxRewriter
{
    // We replace the parameter with an identifier that looks like our placeholder.
    private static readonly IdentifierNameSyntax PlaceholderNode = SyntaxFactory.IdentifierName("{0}");
    private static readonly IdentifierNameSyntax CancellationTokenNode = SyntaxFactory.IdentifierName("cancellationToken");
    private readonly string _parameterNameToReplace;
    private readonly string? _cancellationTokenParameterName;

    public LambdaParameterRewriter(string parameterNameToReplace, string? cancellationTokenParameterName = null)
    {
        _parameterNameToReplace = parameterNameToReplace;
        _cancellationTokenParameterName = cancellationTokenParameterName;
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

        // If this identifier matches the CancellationToken parameter name...
        if (_cancellationTokenParameterName != null && node.Identifier.ValueText == _cancellationTokenParameterName)
        {
            // ...replace it with the actual cancellationToken parameter from the generated method
            return CancellationTokenNode
                .WithLeadingTrivia(node.GetLeadingTrivia())
                .WithTrailingTrivia(node.GetTrailingTrivia());
        }

        // Otherwise, continue traversing the tree without changing anything.
        return base.VisitIdentifierName(node);
    }
}