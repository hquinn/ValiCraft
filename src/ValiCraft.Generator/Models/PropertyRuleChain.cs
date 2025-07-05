using System.Collections.Generic;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record PropertyRuleChain(
    ArgumentInfo Property,
    EquatableArray<Rule> Rules,
    int NumberOfRules) : RuleChain(NumberOfRules)
{
    public override string GenerateCodeForRuleChain(ref int assignedErrorsCount)
    {
        var ruleCodes = new List<string>(NumberOfRules);

        foreach (var rule in Rules)
        {
            ruleCodes.Add(rule.GenerateCodeForRule(Property, ref assignedErrorsCount));
            assignedErrorsCount--;
        }
        
        return string.Join("\r\n", ruleCodes);
    }
}