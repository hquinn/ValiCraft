namespace ValiCraft.BuilderTypes;

/// <summary>
/// Builder type returned by <c>Otherwise()</c> for configuring fallback behavior
/// when none of the specified types match in a polymorphic validation.
/// </summary>
/// <typeparam name="TRequest">The type of the object being validated.</typeparam>
/// <typeparam name="TTarget">The base type of the property being validated.</typeparam>
public interface IPolymorphicOtherwiseBuilderType<TRequest, TTarget>
    where TRequest : class
    where TTarget : class
{
    /// <summary>
    /// Allows validation to pass when no specified types match.
    /// </summary>
    void Allow();

    /// <summary>
    /// Fails validation when no specified types match.
    /// </summary>
    void Fail();

    /// <summary>
    /// Fails validation with a custom message when no specified types match.
    /// </summary>
    /// <param name="message">The error message. Supports placeholders like {TargetName}.</param>
    void Fail(string message);
}
