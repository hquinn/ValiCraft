using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public class RuleBuilder
{
    private MessageInfo? _errorCode;
    private MessageInfo? _message;
    private MessageInfo? _propertyName;
    private readonly SemanticMode _semanticMode;
    private readonly string _methodName;
    private readonly EquatableArray<ArgumentInfo> _arguments;
    private readonly MapToValidationRuleData? _mapData;
    private readonly MessageInfo? _defaultMessage;
    private readonly EquatableArray<RulePlaceholder> _rulePlaceholders;
    private readonly LocationInfo _location;

    public RuleBuilder(
        SemanticMode semanticMode,
        string methodName,
        EquatableArray<ArgumentInfo> arguments,
        MapToValidationRuleData? mapData,
        MessageInfo? defaultMessage,
        EquatableArray<RulePlaceholder> rulePlaceholders,
        LocationInfo location)
    {
        _semanticMode = semanticMode;
        _methodName = methodName;
        _arguments = arguments;
        _mapData = mapData;
        _defaultMessage = defaultMessage;
        _rulePlaceholders = rulePlaceholders;
        _location = location;
    }

    public static RuleBuilder CreateRichSematicRule(
        IMethodSymbol methodSymbol,
        InvocationExpressionSyntax invocation,
        string methodName,
        SemanticModel semanticModel)
    {
        var containingType = methodSymbol.ContainingType;

        return new RuleBuilder(
            SemanticMode.RichSemanticMode,
            methodName,
            invocation.GetArguments(methodSymbol, semanticModel).ToEquatableImmutableArray(),
            MapToValidationRuleData.CreateFromMethodAndAttribute(
                methodSymbol, KnownNames.Attributes.MapToValidationRuleAttribute),
            MessageInfo.CreateFromAttribute(containingType, KnownNames.Attributes.DefaultMessageAttribute),
            RulePlaceholder.CreateFromRulePlaceholderAttributes(containingType),
            LocationInfo.CreateFrom(invocation)!);
    }

    public static RuleBuilder CreateWeakSemanticRule(
        InvocationExpressionSyntax invocation,
        string methodName,
        SemanticModel semanticModel)
    {
        // We can't provide a lot of information from weak semantics currently, however, this can be considered the discovery phase.
        // We'll be able to (hopefully) add all the necessary information when it comes time later in the pipeline
        // when we have access to the generated validation rules (unique to the weak semantics mode).
        return new RuleBuilder(
            SemanticMode.WeakSemanticMode,
            methodName,
            invocation.GetArguments(null, semanticModel).ToEquatableImmutableArray(),
            null,
            null,
            EquatableArray<RulePlaceholder>.Empty,
            LocationInfo.CreateFrom(invocation)!);
    }

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
            _semanticMode,
            _methodName,
            _arguments,
            _mapData,
            new RuleOverrideData(_message, _propertyName, _errorCode),
            _defaultMessage,
            _rulePlaceholders,
            _location);
    }
}