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
   /// <param name="ruleType">The type of the validation rule class.</param>
   /// <param name="methodName">The name of the method.</param>
   public MapToValidationRuleAttribute(
        Type ruleType,
        string methodName)
    {
        RuleType = ruleType;
        MethodName = methodName;
    }
   
    /// <summary>
    /// Initializes a new instance of the <see cref="MapToValidationRuleAttribute"/> class.
    /// </summary>
    /// <param name="ruleType">The type of the validation rule class.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="genericArgumentIndices">The generic argument indices to use when mapping to the static method.</param>
    public MapToValidationRuleAttribute(
        Type ruleType,
        string methodName,
        int[] genericArgumentIndices)
    {
        RuleType = ruleType;
        MethodName = methodName;
        GenericArgumentIndices = genericArgumentIndices;
    }

    /// <summary>
    /// Gets the type of the validation rule that this method maps to.
    /// </summary>
    public Type RuleType { get; }

   /// <summary>
   /// Gets the name of the method that this attribute is applied to.
   /// </summary>
   public string MethodName { get; }

   /// <summary>
   /// Specifies which generic argument indices to use when mapping to the static method.
   /// If null, uses convention (skip first argument which is TRequest).
   /// Examples:
   /// - [1] means use only the 2nd type argument (index 1)
   /// - [1, 2] means use 2nd and 3rd type arguments
   /// - [2] means use only the 3rd type argument
   /// </summary>
   public int[]? GenericArgumentIndices { get; set; }
}