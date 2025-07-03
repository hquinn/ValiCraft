namespace ValiCraft.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RulePlaceholderAttribute : Attribute
{
    public RulePlaceholderAttribute(string placeholderName, string parameterName)
    {
        PlaceholderName = placeholderName;
        ParameterName = parameterName;
    }

    public string PlaceholderName { get; }
    public string ParameterName { get; }
}