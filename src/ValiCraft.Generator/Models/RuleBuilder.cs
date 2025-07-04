using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public class RuleBuilder(
    SemanticMode semanticMode,
    string methodName,
    EquatableArray<ArgumentInfo> arguments,
    MapToValidationRuleData? mapData,
    MessageInfo? defaultMessage,
    EquatableArray<RulePlaceholder> rulePlaceholders,
    LocationInfo location)
{
    private MessageInfo? _errorCode;
    private MessageInfo? _message;
    private MessageInfo? _propertyName;

    public void WithMessage(MessageInfo? message)
    {
        _message = message;
    }

    public void WithErrorCode(MessageInfo? errorCode)
    {
        _errorCode = errorCode;
    }

    public void WithPropertyName(MessageInfo? propertyName)
    {
        _propertyName = propertyName;
    }

    public Rule Build()
    {
        return new Rule(
            semanticMode,
            methodName,
            arguments,
            mapData,
            new RuleOverrideData(_message, _propertyName, _errorCode),
            defaultMessage,
            rulePlaceholders,
            location);
    }
}