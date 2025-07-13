using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.Rules.Builders;

public abstract class RuleBuilder
{
    private MessageInfo? _errorCode;
    private MessageInfo? _message;
    private MessageInfo? _targetName;

    public void WithMessage(MessageInfo? message)
    {
        _message = message;
    }

    public void WithErrorCode(MessageInfo? errorCode)
    {
        _errorCode = errorCode;
    }

    public void WithTargetName(MessageInfo? targetName)
    {
        _targetName = targetName;
    }

    protected RuleOverrideData GetRuleOverrideData()
    {
        return new RuleOverrideData(_message, _targetName, _errorCode);
    }

    public abstract Rule Build();
}