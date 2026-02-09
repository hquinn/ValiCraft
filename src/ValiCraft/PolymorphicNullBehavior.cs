namespace ValiCraft;

/// <summary>
/// Specifies the behavior when a polymorphic validation target is null.
/// </summary>
public enum PolymorphicNullBehavior
{
    /// <summary>
    /// Skips validation and allows null values to pass.
    /// </summary>
    Skip,
    
    /// <summary>
    /// Fails validation when the target is null.
    /// </summary>
    Fail
}
