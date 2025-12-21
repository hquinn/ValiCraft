using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is not contained within a specified set of forbidden values (params version).
/// This allows a more convenient syntax: IsNotInValues("A", "B", "C") instead of IsNotIn(new[] { "A", "B", "C" })
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated</typeparam>
[GenerateRuleExtension("IsNotInValues")]
[DefaultMessage("{TargetName} must not be one of the forbidden values")]
[RulePlaceholder("{ForbiddenValues}", "forbiddenValues")]
public class NotInValues<TTargetType> : IValidationRule<TTargetType, TTargetType[]>
    where TTargetType : IEquatable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue, TTargetType[] forbiddenValues)
    {
        return !forbiddenValues.Contains(targetValue);
    }
}
