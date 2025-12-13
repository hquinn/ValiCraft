using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is contained within a specified set of allowed values (params version).
/// This allows a more convenient syntax: IsInValues("A", "B", "C") instead of IsIn(new[] { "A", "B", "C" })
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated</typeparam>
[GenerateRuleExtension("IsInValues")]
[DefaultMessage("{TargetName} must be one of the allowed values")]
[RulePlaceholder("{AllowedValues}", "allowedValues")]
public class InValues<TTargetType> : IValidationRule<TTargetType, TTargetType[]>
    where TTargetType : IEquatable<TTargetType>
{
    public static bool IsValid(TTargetType targetValue, TTargetType[] allowedValues)
    {
        return allowedValues.Contains(targetValue);
    }
}
