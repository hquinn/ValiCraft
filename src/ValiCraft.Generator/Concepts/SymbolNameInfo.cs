using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Concepts;

public record SymbolNameInfo
{
    public SymbolNameInfo(ITypeSymbol typeSymbol)
    {
        Name = typeSymbol.Name;
        FullyQualifiedName = $"global::{typeSymbol.ToDisplayString()}";
    }
    
    public string Name { get; }
    public string FullyQualifiedName { get; }
}