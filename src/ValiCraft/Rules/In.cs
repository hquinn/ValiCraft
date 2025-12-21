using ValiCraft.Attributes;

namespace ValiCraft.Rules;

/// <summary>
/// Validates that a value is contained within a specified set of allowed values.
/// </summary>
/// <typeparam name="TTargetType">The type of value being validated</typeparam>
[GenerateRuleExtension("IsIn")]
[DefaultMessage("{TargetName} must be one of the allowed values")]
[RulePlaceholder("{AllowedValues}", "allowedValues")]
public class In<TTargetType> : IValidationRule<TTargetType, IEnumerable<TTargetType>>
    where TTargetType : IEquatable<TTargetType>
{
    /// <inheritdoc />
    public static bool IsValid(TTargetType targetValue, IEnumerable<TTargetType> allowedValues)
    {
        if (allowedValues is ICollection<TTargetType> collection)
        {
            return collection.Contains(targetValue);
        }

        return allowedValues.Contains(targetValue);
    }
}
