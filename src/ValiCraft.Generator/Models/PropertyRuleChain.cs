using System.Collections.Generic;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record PropertyRuleChain(
    ArgumentInfo Property,
    int Depth,
    int NumberOfRules,
    EquatableArray<Rule> Rules) : RuleChain(Property, Depth, NumberOfRules)
{
    public override string GenerateCodeForRuleChain(ref int assignedErrorsCount)
    {
        var ruleCodes = new List<string>(NumberOfRules);

        foreach (var rule in Rules)
        {
            ruleCodes.Add(rule.GenerateCodeForRule(
                GetRequestParameterName(),
                GetIndent(),
                Property,
                ref assignedErrorsCount));
            assignedErrorsCount--;
        }
        
        return string.Join("\r\n", ruleCodes);
    }
}