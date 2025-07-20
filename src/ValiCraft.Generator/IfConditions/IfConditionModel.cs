using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.IfConditions;

public abstract record IfConditionModel(bool IsRuleChainCondition)
{
    public abstract string GenerateIfBlock(ValidationTarget target, string requestName, IndentModel indent, RuleChainContext context);
}