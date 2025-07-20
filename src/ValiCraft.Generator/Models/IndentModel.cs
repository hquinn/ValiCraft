namespace ValiCraft.Generator.Models;

public record IndentModel
{
    private const int TabIndexSize = 4;
    private const int BaseLineIndent = TabIndexSize * 3;
    
    private IndentModel(int indentLevel)
    {
        IndentLevel = indentLevel;
        Indent =  new string(' ', BaseLineIndent + IndentLevel * TabIndexSize);
    }

    public static IndentModel CreateNew()
    {
        return new IndentModel(0);
    }
    
    public static IndentModel CreateChild(IndentModel parent)
    {
        return new IndentModel(parent.IndentLevel + 1);
    }

    private int IndentLevel { get; }
    private string Indent { get; }
    
    public override string ToString() => Indent;
}