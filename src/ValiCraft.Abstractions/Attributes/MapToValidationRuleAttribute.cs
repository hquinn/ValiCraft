namespace ValiCraft.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MapToValidationRuleAttribute : Attribute
{
    public MapToValidationRuleAttribute(Type validationRuleType)
    {
        ValidationRuleType = validationRuleType;
    }
    
    public Type ValidationRuleType { get; }
}