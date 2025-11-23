using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsInFutureOrPresent")]
[DefaultMessage("{TargetName} must be in the future or present")]
public class InFutureOrPresent : IValidationRule<DateTime>
{
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue >= DateTime.UtcNow;
    }
}
