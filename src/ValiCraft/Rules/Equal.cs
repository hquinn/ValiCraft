using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value equals another specified value.
/// </summary>
/// <typeparam name="TTargetType">The type of value being compared</typeparam>
[GenerateRuleExtension("IsEqualTo")]
[DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
[RulePlaceholder("{ValueToCompare}", "parameter")]
public class Equal<TTargetType> : IValidationRule<TTargetType, TTargetType>
    where TTargetType : IEquatable<TTargetType>
{
    public static bool IsValid(TTargetType property, TTargetType parameter)
    {
        return property.Equals(parameter);
    }
}
