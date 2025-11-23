using ValiCraft.Attributes;

namespace ValiCraft.Rules;

[GenerateRuleExtension("IsInPast")]
[DefaultMessage("{TargetName} must be in the past")]
public class InPast : IValidationRule<DateTime>
{
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue < DateTime.UtcNow;
    }
}
