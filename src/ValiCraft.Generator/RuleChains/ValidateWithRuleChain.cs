using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains;

public abstract record ValidateWithRuleChain(
    ValidationTarget Target,
    int Depth,
    OnFailureMode? FailureMode,
    string ValidatorExpression) : RuleChain(Target, Depth, 1, FailureMode)
{
    
}