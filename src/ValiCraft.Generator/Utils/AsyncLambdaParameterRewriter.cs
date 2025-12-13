using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ValiCraft.Generator.Utils;

/// <summary>
/// Rewriter for async lambda expressions that have two parameters: value and cancellation token.
/// Replaces the value parameter with {0} placeholder and the cancellation token parameter with "cancellationToken".
/// </summary>
public class AsyncLambdaParameterRewriter : CSharpSyntaxRewriter
{
    // We replace the value parameter with an identifier that looks like our placeholder.
    private static readonly IdentifierNameSyntax PlaceholderNode = SyntaxFactory.IdentifierName("{0}");
    
    // We replace the cancellation token parameter with the actual variable name used in generated code.
    private static readonly IdentifierNameSyntax CancellationTokenNode = SyntaxFactory.IdentifierName("cancellationToken");
    
    private readonly string _valueParameterName;
    private readonly string _cancellationTokenParameterName;

    public AsyncLambdaParameterRewriter(string valueParameterName, string cancellationTokenParameterName)
    {
        _valueParameterName = valueParameterName;
        _cancellationTokenParameterName = cancellationTokenParameterName;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        // If this identifier's text matches the value parameter's name...
        if (node.Identifier.ValueText == _valueParameterName)
        {
            // ...replace it with our placeholder syntax node, preserving trivia (whitespace).
            return PlaceholderNode
                .WithLeadingTrivia(node.GetLeadingTrivia())
                .WithTrailingTrivia(node.GetTrailingTrivia());
        }
        
        // If this identifier's text matches the cancellation token parameter's name...
        if (node.Identifier.ValueText == _cancellationTokenParameterName)
        {
            // ...replace it with the actual cancellationToken variable name, preserving trivia.
            return CancellationTokenNode
                .WithLeadingTrivia(node.GetLeadingTrivia())
                .WithTrailingTrivia(node.GetTrailingTrivia());
        }

        // Otherwise, continue traversing the tree without changing anything.
        return base.VisitIdentifierName(node);
    }
}
