using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Models;

/// <summary>
/// Represents the behavior for a type branch in polymorphic validation.
/// </summary>
public enum PolymorphicBranchBehavior
{
    /// <summary>
    /// Validates the target using a validator.
    /// </summary>
    ValidateWith,
    
    /// <summary>
    /// Allows validation to pass without further checks.
    /// </summary>
    Allow,
    
    /// <summary>
    /// Fails validation.
    /// </summary>
    Fail
}

/// <summary>
/// Represents a single type branch in polymorphic validation (WhenType&lt;T&gt;).
/// </summary>
/// <param name="DerivedType">The derived type for this branch.</param>
/// <param name="Behavior">The behavior for this branch.</param>
/// <param name="ValidatorExpression">The validator expression for ValidateWith branches.</param>
/// <param name="IsAsyncValidatorCall">Whether the validator is async.</param>
/// <param name="FailMessage">The fail message for Fail branches.</param>
/// <param name="IsStaticValidator">Whether this is a static validator (Validate&lt;T&gt; or ValidateAsync&lt;T&gt;).</param>
/// <param name="StaticValidatorTypeName">The fully qualified type name of the static validator.</param>
public record PolymorphicBranch(
    TypeInfo DerivedType,
    PolymorphicBranchBehavior Behavior,
    string? ValidatorExpression,
    bool IsAsyncValidatorCall,
    MessageInfo? FailMessage,
    bool IsStaticValidator = false,
    string? StaticValidatorTypeName = null);

/// <summary>
/// Represents the Otherwise branch in polymorphic validation.
/// </summary>
public record PolymorphicOtherwiseBranch(
    PolymorphicBranchBehavior Behavior,
    MessageInfo? FailMessage);

/// <summary>
/// Specifies the null behavior for polymorphic validation.
/// </summary>
public enum PolymorphicNullBehavior
{
    /// <summary>
    /// Skips validation when target is null.
    /// </summary>
    Skip,
    
    /// <summary>
    /// Fails validation when target is null.
    /// </summary>
    Fail
}
