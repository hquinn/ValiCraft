using System.Linq.Expressions;
using ValiCraft.BuilderTypes;

namespace ValiCraft;

/// <summary>
/// Provides a fluent API for defining validation rules on properties of <typeparamref name="TRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The type of object being validated.</typeparam>
public interface IValidationRuleBuilder<TRequest> where TRequest : class
{
    /// <summary>
    /// Starts a validation rule chain for a specific property.
    /// </summary>
    /// <typeparam name="TTarget">The type of the property to validate.</typeparam>
    /// <param name="selector">An expression selecting the property to validate.</param>
    /// <param name="failureMode">Optional. Specifies behavior when validation fails (Continue or Halt).</param>
    /// <returns>A builder for chaining validation rules on the selected property.</returns>
    IEnsureBuilderType<TRequest, TTarget> Ensure<TTarget>(
        Expression<Func<TRequest, TTarget>> selector,
        OnFailureMode? failureMode = null);
    
    /// <summary>
    /// Starts a validation rule chain for each item in a collection property.
    /// </summary>
    /// <typeparam name="TTarget">The type of items in the collection.</typeparam>
    /// <param name="selector">An expression selecting the collection property.</param>
    /// <param name="failureMode">Optional. Specifies behavior when validation fails.</param>
    /// <returns>A builder for chaining validation rules on each item.</returns>
    IEnsureEachBuilderType<TRequest, TTarget> EnsureEach<TTarget>(
        Expression<Func<TRequest, IEnumerable<TTarget>>> selector,
        OnFailureMode? failureMode = null);
    
    /// <summary>
    /// Validates each item in a collection using a nested validator configuration.
    /// </summary>
    /// <typeparam name="TTarget">The type of items in the collection.</typeparam>
    /// <param name="selector">An expression selecting the collection property.</param>
    /// <param name="rules">A delegate to configure validation rules for each item.</param>
    void EnsureEach<TTarget>(
        Expression<Func<TRequest, IEnumerable<TTarget>>> selector,
        Action<IValidationRuleBuilder<TTarget>> rules) where TTarget : class;
    
    /// <summary>
    /// Validates each item in a collection using a nested validator configuration with a failure mode.
    /// </summary>
    /// <typeparam name="TTarget">The type of items in the collection.</typeparam>
    /// <param name="selector">An expression selecting the collection property.</param>
    /// <param name="failureMode">Specifies behavior when validation fails.</param>
    /// <param name="rules">A delegate to configure validation rules for each item.</param>
    void EnsureEach<TTarget>(
        Expression<Func<TRequest, IEnumerable<TTarget>>> selector,
        OnFailureMode failureMode,
        Action<IValidationRuleBuilder<TTarget>> rules) where TTarget : class;
    
    /// <summary>
    /// Groups validation rules with a specific failure mode.
    /// </summary>
    /// <param name="failureMode">The failure mode to apply to the contained rules.</param>
    /// <param name="rules">A delegate to configure the grouped rules.</param>
    void WithOnFailure(OnFailureMode failureMode, Action<IValidationRuleBuilder<TRequest>> rules);
    
    /// <summary>
    /// Conditionally applies validation rules based on a predicate.
    /// </summary>
    /// <param name="condition">The condition that must be true for rules to apply.</param>
    /// <param name="rules">A delegate to configure rules that apply when the condition is true.</param>
    void If(Func<TRequest, bool> condition, Action<IValidationRuleBuilder<TRequest>> rules);
}