using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value does not equal another specified value.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared</typeparam>
[GenerateRuleExtension("IsNotEqualTo")]
[DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class NotEqual<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IEquatable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return !property.Equals(parameter);
    }
}
