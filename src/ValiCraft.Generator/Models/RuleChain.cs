using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Models;

public abstract record RuleChain(
    ArgumentInfo Property,
    int Depth,
    int NumberOfRules)
{
    private const int TabIndexSize = 4;
    private const int BaseLineIndent = TabIndexSize * 3;
    
    public abstract string GenerateCodeForRuleChain(ref int assignedErrorsCount);

    public string GetRequestParameterName()
    {
        return Depth switch
        {
            0 => "request",
            1 => "element",
            2 => "subElement",
            _ => $"subElement{Depth - 1}"
        };
    }

    protected string GetIndent()
    {
        return new string(' ', BaseLineIndent + (Depth * TabIndexSize));
    }
}