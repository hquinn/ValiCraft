namespace ValiCraft.BuilderTypes;

public interface IBuilderType<TRequest, TTarget>
    where TRequest : class
{
    IValidationRuleBuilderType<TRequest, TTarget> Must(Func<TTarget, bool> predicate);
    
    /// <summary>
    /// Indicates that subsequent validation rules should only be applied if the target value is not null.
    /// This is useful for optional nullable properties where you want to validate the value only when present.
    /// </summary>
    /// <example>
    /// builder.Ensure(x => x.OptionalEmail)
    ///     .WhenNotNull()
    ///     .IsEmailAddress()
    ///     .HasMaxLength(255);
    /// </example>
    IWhenNotNullBuilderType<TRequest, TTarget> WhenNotNull();
}