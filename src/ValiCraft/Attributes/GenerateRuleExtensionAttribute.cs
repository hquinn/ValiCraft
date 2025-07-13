namespace ValiCraft.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GenerateRuleExtensionAttribute : Attribute
{
    public GenerateRuleExtensionAttribute(string name)
    {
        NameForRuleExtension = name;
    }

    public string NameForRuleExtension { get; }
}