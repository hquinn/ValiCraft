namespace ValiCraft.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DefaultMessageAttribute : Attribute
{
    public DefaultMessageAttribute(string message)
    {
        Message = message;
    }

    public string Message { get; }
}