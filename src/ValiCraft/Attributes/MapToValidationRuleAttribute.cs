namespace ValiCraft.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MapToValidationRuleAttribute : Attribute
{
    public MapToValidationRuleAttribute(
        Type validationRuleType,
        string validationRuleGenericFormat,
        string? defaultMessage)
    {
        ValidationRuleType = validationRuleType;
        ValidationRuleGenericFormat = validationRuleGenericFormat;
        DefaultMessage = defaultMessage;
    }
    
    public Type ValidationRuleType { get; }
    public string ValidationRuleGenericFormat { get; }
    public string? DefaultMessage { get; }
}