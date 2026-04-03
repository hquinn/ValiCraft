namespace ValiCraft.Generator.Concepts;

public readonly record struct InlineExpression(string Format)
{
    public string Inline(string targetAccessor) => string.Format(Format, targetAccessor);

    public override string ToString() => Format;
}
