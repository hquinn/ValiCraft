namespace ValiCraft.Generator.Models;

public abstract record RuleChain(int NumberOfRules)
{
    public abstract string GenerateCodeForRuleChain(ref int assignedErrorsCount);
}