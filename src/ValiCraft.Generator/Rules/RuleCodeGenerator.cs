using System.Collections.Generic;
using System.Linq;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules;

internal static class RuleCodeGenerator
{
    internal static string GenerateCodeForRule(
        Rule rule,
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context)
    {
        return rule.Kind switch
        {
            RuleKind.ExpressionFormat => GenerateExpressionFormatCode(rule, requestName, indent, @object, target, context),
            RuleKind.BlockLambda => GenerateBlockLambdaCode(rule, requestName, indent, @object, target, context),
            RuleKind.ExtensionMethod => GenerateExtensionMethodCode(rule, requestName, indent, @object, target, context),
            _ => string.Empty
        };
    }

    private static string GenerateExpressionFormatCode(
        Rule rule,
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);

        var inlinedCondition = string.Format(rule.ExpressionFormat!, targetAccessor);

        var code = $$"""
                     {{rule.IfCondition.GenerateIfBlock(@object, requestName, indent, context)}}!{{inlinedCondition}})
                     {{GetErrorCreation(rule, requestName, KnownNames.Targets.Is, indent, target, context)}}
                     """;

        context.UpdateIfElseMode();

        return code;
    }

    private static string GenerateBlockLambdaCode(
        Rule rule,
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context)
    {
        var targetAccessor = string.Format(target.AccessorExpressionFormat, requestName);

        var localFunctionName = $"__is{(rule.IsAsync ? "Async" : "")}_{context.Counter}";

        var functionParameters = rule.CancellationTokenParameter != null
            ? $"{target.Type.FormattedTypeName} {rule.Parameter}, global::System.Threading.CancellationToken {rule.CancellationTokenParameter}"
            : $"{target.Type.FormattedTypeName} {rule.Parameter}";

        var localFunction = $$"""
                              {{indent}}{{(rule.IsAsync ? "async global::System.Threading.Tasks.Task<bool>" : "bool")}} {{localFunctionName}}({{functionParameters}})
                              {{indent}}{{rule.Body}}
                              """;

        var callArguments = rule.CancellationTokenParameter != null
            ? $"{targetAccessor}, cancellationToken"
            : targetAccessor;

        var inlinedCondition = rule.IsAsync
            ? $"await {localFunctionName}({callArguments})"
            : $"{localFunctionName}({callArguments})";

        var code = $$"""
                     {{localFunction}}
                     {{rule.IfCondition.GenerateIfBlock(@object, requestName, indent, context)}}!{{inlinedCondition}})
                     {{GetErrorCreation(rule, requestName, KnownNames.Targets.Is, indent, target, context)}}
                     """;

        context.UpdateIfElseMode();

        return code;
    }

    private static string GenerateExtensionMethodCode(
        Rule rule,
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context)
    {
        var targetAccess = string.Format(target.AccessorExpressionFormat, requestName);
        var validationRuleInvocation =
            $"global::{rule.ValidationRuleData?.RuleType}.{ConstructValidationRuleGeneric(rule)}";
        var errorCode = $"global::{rule.ValidationRuleData?.RuleType}.{rule.ValidationRuleData?.MethodName}";

        var isValidCallArgs = new List<string> { targetAccess };
        isValidCallArgs.AddRange(rule.Arguments.GetArray()?.Select(x => x.Value) ?? []);
        var isValidCallArgsString = string.Join(", ", isValidCallArgs);

        var code = $"""
                     {rule.IfCondition.GenerateIfBlock(@object, requestName, indent, context)}!{validationRuleInvocation}({isValidCallArgsString}))
                     {GetErrorCreation(rule, requestName, errorCode, indent, target, context)}
                     """;

        context.UpdateIfElseMode();

        return code;
    }

    private static string? ConstructValidationRuleGeneric(Rule rule)
    {
        if (string.IsNullOrEmpty(rule.ValidationRuleData?.MethodName))
        {
            return null;
        }

        if (rule.GenericArguments.Count == 0 ||
            (rule.ValidationRuleData?.GenericArgumentIndices is null && rule.GenericArguments.Count <= 1) ||
            rule.ValidationRuleData?.GenericArgumentIndices == EquatableArray<int>.Empty)
        {
            return rule.ValidationRuleData?.MethodName;
        }

        if (rule.ValidationRuleData?.GenericArgumentIndices is null)
        {
            return $"{rule.ValidationRuleData?.MethodName}<{string.Join(", ", rule.GenericArguments.Skip(1))}>";
        }

        return $"{rule.ValidationRuleData!.MethodName}<{string.Join(", ", rule.GenericArguments.Where((_, i) => rule.ValidationRuleData?.GenericArgumentIndices.Contains(i) == true))}>";
    }

    private static string GetErrorCreation(
        Rule rule,
        string requestName,
        string validationRuleInvocation,
        IndentModel indent,
        ValidationTarget target,
        RuleChainContext context)
    {
        return ErrorCreationHelper.GetErrorCreation(
            requestName, validationRuleInvocation, indent, target, context,
            rule.RuleOverrides, rule.DefaultMessage, rule.DefaultErrorCode, rule.Placeholders, rule.Arguments);
    }
}
