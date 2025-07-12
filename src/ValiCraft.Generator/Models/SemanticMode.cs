namespace ValiCraft.Generator.Models;

public enum SemanticMode
{
    /// <summary>
    ///     Rich Semantic Mode is when we're able to get semantic information about the validation rule extension method
    ///     from the invocation in the validator. This will typically happen when the validation rule extension method
    ///     exists in a separate project or the extension method has manually been created.
    /// </summary>
    RichSemanticMode,

    /// <summary>
    ///     Weak Semantic Mode is when we're not able to get semantic information about the validation rule extension method
    ///     from the invocation in the validator. This will happen when the validation rule that has
    ///     the [GenerateRuleExtension] is in the same project as the validator. This can also happen when a rule invocation
    ///     which happens after one that's a weak semantic mode, e.g., builder.Ensure(x => x.Property).Weak().Rich().
    ///     In this case, the compiler can't infer where to locate the extension method for Weak(), which also affects Rich()
    /// </summary>
    WeakSemanticMode
}