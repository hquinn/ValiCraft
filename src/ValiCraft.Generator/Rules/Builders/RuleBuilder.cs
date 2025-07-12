using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.Rules.Builders;

public abstract class RuleBuilder
{
    private MessageInfo? _errorCode;
    private MessageInfo? _message;
    private MessageInfo? _targetName;

    public static RuleBuilder CreateMustRule(
        InvocationExpressionSyntax invocation,
        string methodName,
        SimpleLambdaExpressionSyntax lambda,
        SemanticModel semanticModel)
    {
        return default;
    }

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

    protected RuleOverrideData GetRuleOverrideData(
        string? message = null,
        string? targetName = null,
        string? errorCode = null)
    {
        
        
        return new RuleOverrideData(_message, _targetName, _errorCode);
    }

    public abstract Rule Build();
}