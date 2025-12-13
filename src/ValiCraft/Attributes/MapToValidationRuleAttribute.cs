namespace ValiCraft.Attributes;

/// <summary>
/// Maps a fluent extension method to its corresponding validation rule type.
/// This is used internally by the source generator to link fluent API calls
/// to their validation rule implementations.
/// </summary>
/// <remarks>
/// This attribute is primarily used in generated code and advanced scenarios
/// where manual mapping between extension methods and rule types is required.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MapToValidationRuleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapToValidationRuleAttribute"/> class.
    /// </summary>
    /// <param name="validationRuleType">The type of the validation rule class.</param>
    /// <param name="validationRuleGenericFormat">The generic format string for the rule type.</param>
    public MapToValidationRuleAttribute(
        Type validationRuleType,
        string validationRuleGenericFormat)
    {
        ValidationRuleType = validationRuleType;
        ValidationRuleGenericFormat = validationRuleGenericFormat;
    }

    /// <summary>
    /// Gets the type of the validation rule that this method maps to.
    /// </summary>
    public Type ValidationRuleType { get; }

    /// <summary>
    /// Gets the generic format string used to construct the full generic type.
    /// </summary>
    public string ValidationRuleGenericFormat { get; }
}