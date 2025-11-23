using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsInFuture")]
[DefaultMessage("{TargetName} must be in the future")]
public class InFuture : IValidationRule<DateTime>
{
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue > DateTime.UtcNow;
    }
}
