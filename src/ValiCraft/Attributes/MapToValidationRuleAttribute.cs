namespace ValiCraft.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MapToValidationRuleAttribute : Attribute
{
    public MapToValidationRuleAttribute(
        Type validationRuleType,
        string validationRuleGenericFormat)
    {
        ValidationRuleType = validationRuleType;
        ValidationRuleGenericFormat = validationRuleGenericFormat;
    }

    public Type ValidationRuleType { get; }
    public string ValidationRuleGenericFormat { get; }
}