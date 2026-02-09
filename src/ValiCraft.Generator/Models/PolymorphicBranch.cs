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
public record PolymorphicBranch(
    TypeInfo DerivedType,
    PolymorphicBranchBehavior Behavior,
    string? ValidatorExpression,
    bool IsAsyncValidatorCall,
    MessageInfo? FailMessage);

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
