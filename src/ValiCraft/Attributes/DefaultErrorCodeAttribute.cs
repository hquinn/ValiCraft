namespace ValiCraft.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DefaultErrorCodeAttribute : Attribute
{
    public DefaultErrorCodeAttribute(string errorCode)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}