using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public abstract class RuleBuilder
{
    private MessageInfo? _errorCode;
    private MessageInfo? _message;
    private MessageInfo? _targetName;
    private MessageInfo? _severity;
    private IfConditionModel? _ifCondition;
    private List<MetadataEntry>? _metadata;
    protected IfConditionModel IfCondition => _ifCondition ?? new BlankIfConditionModel(false);

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

    public void WithSeverity(MessageInfo? severity)
    {
        _severity = severity;
    }

    public void WithMetadata(MetadataEntry entry)
    {
        _metadata ??= new List<MetadataEntry>();
        _metadata.Add(entry);
    }

    public void WithCondition(InvocationExpressionSyntax invocation)
    {
        var ifConditionModel = IfConditionFactory.Create(invocation, false);

        if (ifConditionModel is not null)
        {
            _ifCondition = ifConditionModel;
        }
    }

    protected RuleOverrideData GetRuleOverrideData()
    {
        return new RuleOverrideData(
            _message, 
            _targetName, 
            _errorCode, 
            _severity,
            _metadata is not null ? new EquatableArray<MetadataEntry>(_metadata.ToArray()) : null);
    }

    public abstract Rule Build();
}