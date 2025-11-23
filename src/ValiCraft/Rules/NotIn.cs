using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is not contained within a specified set of forbidden values.
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated</typeparam>
[GenerateRuleExtension("IsNotIn")]
[DefaultMessage("{TargetName} must not be one of the forbidden values")]
[RulePlaceholder("{ForbiddenValues}", "forbiddenValues")]
public class NotIn<TTargetType> : IValidationRule<TTargetType, IEnumerable<TTargetType>>
    where TTargetType : IEquatable<TTargetType>
{
    public static bool IsValid(TTargetType targetValue, IEnumerable<TTargetType> forbiddenValues)
    {
        return !forbiddenValues.Contains(targetValue);
    }
}
