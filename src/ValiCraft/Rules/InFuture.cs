using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a DateTime value is in the future (after the current UTC time).
/// </summary>
[GenerateRuleExtension("IsInFuture")]
[DefaultMessage("{TargetName} must be in the future")]
public class InFuture : IValidationRule<DateTime>
{
    public static bool IsValid(DateTime targetValue)
    {
        return targetValue > DateTime.UtcNow;
    }
}
