using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.IfConditions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Rules.Builders;

public sealed class RuleBuilder
{
    private MessageInfo? _errorCode;
    private MessageInfo? _message;
    private MessageInfo? _targetName;
    private MessageInfo? _severity;
    private IfConditionModel? _ifCondition;
    private List<MetadataEntry>? _metadata;
    private IfConditionModel IfCondition => _ifCondition ?? IfConditionModel.Blank();

    // Rule variant data
    private RuleKind _kind;
    private bool _isAsync;
    private EquatableArray<ArgumentInfo> _arguments;
    private MessageInfo? _defaultMessage;
    private MessageInfo? _defaultErrorCode;
    private EquatableArray<RulePlaceholder> _placeholders;
    private LocationInfo _location = null!;
    private string? _expressionFormat;
    private string? _body;
    private string? _parameter;
    private string? _cancellationTokenParameter;
    private EquatableArray<string> _genericArguments;
    private MapToValidationRuleData? _validationRuleData;

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
        _metadata ??= [];
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

    public Rule Build()
    {
        return new Rule(
            _kind,
            _arguments,
            _defaultMessage,
            _defaultErrorCode,
            GetRuleOverrideData(),
            IfCondition,
            _placeholders,
            _location,
            IsAsync: _isAsync,
            ExpressionFormat: _expressionFormat,
            Body: _body,
            Parameter: _parameter,
            CancellationTokenParameter: _cancellationTokenParameter,
            GenericArguments: _genericArguments,
            ValidationRuleData: _validationRuleData);
    }

    private RuleOverrideData GetRuleOverrideData()
    {
        return new RuleOverrideData(
            _message,
            _targetName,
            _errorCode,
            _severity,
            _metadata is not null ? new EquatableArray<MetadataEntry>(_metadata.ToArray()) : null);
    }

    // --- Static factory methods ---

    public static RuleBuilder CreateExpressionFormat(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        string fullMethodName,
        ISymbol? methodSymbol,
        GeneratorAttributeSyntaxContext context)
    {
        var extraArgs = invocation.ArgumentList.Arguments
            .Skip(1)
            .Select(a => a.ToString())
            .ToList();

        var sb = new StringBuilder($"{(isAsync ? "await " : string.Empty)}{fullMethodName}({{0}}");

        foreach (var arg in extraArgs)
        {
            sb.Append($", {arg}");
        }

        if (usesCancellationToken)
        {
            sb.Append(", cancellationToken");
        }

        sb.Append(")");

        return new RuleBuilder
        {
            _kind = RuleKind.ExpressionFormat,
            _isAsync = isAsync,
            _arguments = invocation.GetRuleArguments(context.SemanticModel).ToEquatableImmutableArray(),
            _defaultMessage = MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultMessageAttribute),
            _defaultErrorCode = MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute) ?? new MessageInfo(fullMethodName, true),
            _placeholders = RulePlaceholder.CreateFromRulePlaceholderAttributes(methodSymbol!),
            _expressionFormat = sb.ToString(),
            _location = LocationInfo.CreateFrom(invocation)!
        };
    }

    public static RuleBuilder CreateBlockLambda(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = usesCancellationToken ? lambda.GetSecondParameterName() : null;

        return new RuleBuilder
        {
            _kind = RuleKind.BlockLambda,
            _isAsync = isAsync,
            _arguments = EquatableArray<ArgumentInfo>.Empty,
            _defaultErrorCode = new MessageInfo(KnownNames.Targets.Is, true),
            _placeholders = EquatableArray<RulePlaceholder>.Empty,
            _body = lambda.Body.ToString(),
            _parameter = parameterName,
            _cancellationTokenParameter = cancellationTokenParameterName,
            _location = LocationInfo.CreateFrom(invocation)!
        };
    }

    public static RuleBuilder CreateInvocationLambda(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda,
        GeneratorAttributeSyntaxContext context)
    {
        var innerInvocation = lambda.Body switch
        {
            InvocationExpressionSyntax i => i,
            AwaitExpressionSyntax { Expression: InvocationExpressionSyntax i } => i,
            _ => null
        };

        var innerInvocationSymbol = context.SemanticModel.GetSymbolInfo(innerInvocation!).Symbol;

        var fullMethodName = innerInvocation!.Expression.ToString();

        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = usesCancellationToken ? lambda.GetSecondParameterName() : null;

        var rewriter = new LambdaParameterRewriter(parameterName, cancellationTokenParameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);
        var body = rewrittenBody?.ToString() ?? string.Empty;

        if (isAsync && lambda.Body is InvocationExpressionSyntax)
        {
            body = $"await {body}";
        }

        return new RuleBuilder
        {
            _kind = RuleKind.ExpressionFormat,
            _isAsync = isAsync,
            _arguments = invocation.GetRuleArguments(context.SemanticModel).ToEquatableImmutableArray(),
            _defaultMessage = MessageInfo.CreateFromAttribute(innerInvocationSymbol, KnownNames.Attributes.DefaultMessageAttribute),
            _defaultErrorCode = MessageInfo.CreateFromAttribute(innerInvocationSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute) ?? new MessageInfo(fullMethodName, true),
            _placeholders = RulePlaceholder.CreateFromRulePlaceholderAttributes(innerInvocationSymbol!),
            _expressionFormat = body,
            _location = LocationInfo.CreateFrom(invocation)!
        };
    }

    public static RuleBuilder CreateLambda(
        bool isAsync,
        bool usesCancellationToken,
        InvocationExpressionSyntax invocation,
        LambdaExpressionSyntax lambda)
    {
        var parameterName = lambda.GetParameterName();
        var cancellationTokenParameterName = usesCancellationToken ? lambda.GetSecondParameterName() : null;

        // Use our rewriter to visit the lambda body and replace the parameter.
        var rewriter = new LambdaParameterRewriter(parameterName, cancellationTokenParameterName);
        var rewrittenBody = rewriter.Visit(lambda.Body);
        var body = rewrittenBody?.ToString() ?? string.Empty;

        if (isAsync)
        {
            // Check if the rewritten body contains an await expression by parsing it
            var hasAwait = rewrittenBody?.DescendantNodesAndSelf()
                .Any(n => n.IsKind(SyntaxKind.AwaitExpression)) ?? false;

            if (!hasAwait)
            {
                body = $"await {body}";
            }
        }

        return new RuleBuilder
        {
            _kind = RuleKind.ExpressionFormat,
            _isAsync = isAsync,
            _arguments = EquatableArray<ArgumentInfo>.Empty,
            _defaultErrorCode = new MessageInfo(KnownNames.Targets.Is, true),
            _placeholders = EquatableArray<RulePlaceholder>.Empty,
            // Wrap the expression in parentheses so the negation applies correctly:
            // ExpressionFormatRule generates !{condition}, and we need !({condition})
            _expressionFormat = $"({body})",
            _location = LocationInfo.CreateFrom(invocation)!
        };
    }

    public static RuleBuilder? CreateExtensionMethod(
        IMethodSymbol methodSymbol,
        InvocationExpressionSyntax invocation,
        List<DiagnosticInfo> diagnostics,
        SemanticModel semanticModel)
    {
        var mapToValidationRuleAttribute = MapToValidationRuleData.CreateFromMethodAndAttribute(
            methodSymbol, KnownNames.Attributes.MapToValidationRuleAttribute);

        if (mapToValidationRuleAttribute is null)
        {
            diagnostics.Add(DefinedDiagnostics.MissingMapToValidationRuleAttribute(invocation.GetLocation()));
            return null;
        }

        return new RuleBuilder
        {
            _kind = RuleKind.ExtensionMethod,
            _arguments = invocation.GetArguments(methodSymbol, semanticModel).ToEquatableImmutableArray(),
            _defaultMessage = MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultMessageAttribute),
            _defaultErrorCode = MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute),
            _placeholders = RulePlaceholder.CreateFromRulePlaceholderAttributes(methodSymbol),
            _genericArguments = invocation.GetGenericArguments(),
            _validationRuleData = MapToValidationRuleData.CreateFromMethodAndAttribute(
                methodSymbol, KnownNames.Attributes.MapToValidationRuleAttribute),
            _location = LocationInfo.CreateFrom(invocation)!
        };
    }
}
