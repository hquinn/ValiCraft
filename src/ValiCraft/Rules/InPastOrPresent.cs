using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsInPastOrPresent")]
[DefaultMessage("{TargetName} must be in the past or present")]
public class InPastOrPresent : IValidationRule<DateTime>
{
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue <= DateTime.UtcNow;
    }
}
