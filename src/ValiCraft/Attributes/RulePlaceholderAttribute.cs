namespace ValiCraft.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class RulePlaceholderAttribute : Attribute
{
    public string PlaceholderName { get; }
    public string ParameterName { get; }

    public RulePlaceholderAttribute(string placeholderName, string parameterName)
    {
        PlaceholderName = placeholderName;
        ParameterName = parameterName;
    }
}