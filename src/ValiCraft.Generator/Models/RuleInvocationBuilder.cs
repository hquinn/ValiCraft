using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public class RuleInvocationBuilder(
    NameAndTypeInfo property,
    string methodName,
    EquatableArray<NameAndTypeInfo> arguments,
    MapToValidationRuleData? mapData,
    MessageInfo? defaultMessage,
    EquatableArray<RulePlaceholderInfo> rulePlaceholders)
{
    private MessageInfo? _message;
    private MessageInfo? _errorCode;
    private MessageInfo? _propertyName;

    public void WithMessage(MessageInfo? message) => _message = message;
    public void WithErrorCode(MessageInfo? errorCode) => _errorCode = errorCode;
    public void WithPropertyName(MessageInfo? propertyName) => _propertyName = propertyName;

    public RuleInvocation Build() => new(
        property,
        methodName,
        arguments,
        mapData,
        new RuleOverrideData(_message, _propertyName, _errorCode),
        defaultMessage,
        rulePlaceholders);
}