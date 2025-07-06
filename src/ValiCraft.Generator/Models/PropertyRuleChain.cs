using System.Collections.Generic;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record PropertyRuleChain(
    ArgumentInfo Property,
    EquatableArray<Rule> Rules,
    int Depth,
    int NumberOfRules) : RuleChain(Depth, NumberOfRules)
{
    public override string GenerateCodeForRuleChain(ref int assignedErrorsCount)
    {
        var ruleCodes = new List<string>(NumberOfRules);
        var indent = GetIndent();

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