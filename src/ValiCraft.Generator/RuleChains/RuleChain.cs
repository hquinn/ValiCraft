using System.Collections.Generic;
using System.Text;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record RuleChainConfig(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget? Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode);

public abstract record RuleChain(RuleChainConfig Config)
{
    public string GenerateCode(RuleChainContext context)
    {
        var code = new StringBuilder();

        var contextToUse = DetermineRuleChainContext(context);
        var currentTargetPath = contextToUse.TargetPath;
        
        contextToUse.TargetPath = GetTargetPath(contextToUse);
        code.Append(HandleCodeGeneration(contextToUse));
        contextToUse.TargetPath = currentTargetPath;
        
        AppendGotoLabelIfNeeded(code, contextToUse, context);
        
        return code.ToString();
    }

    public abstract bool NeedsGotoLabels();
    
    protected abstract string HandleCodeGeneration(RuleChainContext context);
    
    protected abstract string GetTargetPath(RuleChainContext context);
    
    protected static string BuildTargetPath(
        string currentTargetPath,
        string targetPathValue,
        bool isCollection,
        Counter counter,
        string collectionSuffix = "")
    {
        if (isCollection)
        {
            var indexer = $"index{counter}";
            return $"{currentTargetPath}{targetPathValue}[{{{indexer}}}]{collectionSuffix}";
        }

        return $"{currentTargetPath}{targetPathValue}";
    }

    protected string GetRequestParameterName()
    {
        return CalculateRequestParameterName(Config.Depth);
    }

    protected string GetItemRequestParameterName()
    {
        return CalculateRequestParameterName(Config.Depth + 1);
    }

    private static string CalculateRequestParameterName(int depth)
    {
        return depth switch
        {
            0 => "request",
            1 => "element",
            2 => "subElement",
            _ => $"subElement{depth - 1}"
        };
    }

    private RuleChainContext DetermineRuleChainContext(RuleChainContext context)
    {
        return context.ParentFailureMode is null
            ? HandleNoParentFailureMode(context)
            : HandleWithParentFailureMode(context);
    }

    private RuleChainContext HandleNoParentFailureMode(RuleChainContext context)
    {
        return Config.FailureMode is OnFailureMode.Halt
            ? context.CreateHaltContext(NeedsGotoLabels())
            : context;
    }

    private RuleChainContext HandleWithParentFailureMode(RuleChainContext context)
    {
        return Config.FailureMode switch
        {
            OnFailureMode.Halt => HandleHaltFailureMode(context),
            OnFailureMode.Continue => HandleContinueFailureMode(context),
            _ => context
        };
    }

    private RuleChainContext HandleHaltFailureMode(RuleChainContext context)
    {
        return context.ParentFailureMode is OnFailureMode.Continue
            ? context.CreateHaltContext(NeedsGotoLabels())
            : context;
    }

    private RuleChainContext HandleContinueFailureMode(RuleChainContext context)
    {
        return context.ParentFailureMode is OnFailureMode.Continue
            ? context
            : context.CreateContinueContext();
    }

    protected static ValidationTarget CreateItemTarget(
        Concepts.TypeInfo elementType,
        ValidationTarget collectionTarget)
    {
        return new ValidationTarget(
            AccessorType: AccessorType.Object,
            AccessorExpressionFormat: "{0}",
            Type: elementType,
            DefaultTargetName: collectionTarget.DefaultTargetName,
            TargetPath: collectionTarget.TargetPath);
    }

    protected static List<string> GenerateRulesCode(
        EquatableArray<Rule> rules,
        string requestName,
        IndentModel indent,
        ValidationTarget @object,
        ValidationTarget target,
        RuleChainContext context,
        int extraCapacity = 0)
    {
        var ruleCodes = new List<string>(rules.Count + extraCapacity);

        foreach (var rule in rules)
        {
            ruleCodes.Add(RuleCodeGenerator.GenerateCodeForRule(
                rule,
                requestName,
                indent,
                @object,
                target,
                context));
            context.DecrementCountdown();
        }

        return ruleCodes;
    }

    protected static string GenerateValidatorCallCode(
        IndentModel indent,
        string methodCall,
        RuleChainContext context,
        IndentModel? gotoIndent = null)
    {
        var effectiveGotoIndent = gotoIndent ?? indent;
        return $$"""
                 {{indent}}var errors{{context.Counter}} = {{methodCall}};
                 {{indent}}if (errors{{context.Counter}} is not null)
                 {{indent}}{
                 {{indent}}    if (errors is null)
                 {{indent}}    {
                 {{indent}}        errors = errors{{context.Counter}};
                 {{GetValidatorGotoLabelIfNeeded(effectiveGotoIndent, context)}}{{indent}}    }
                 {{indent}}    else
                 {{indent}}    {
                 {{indent}}        errors.AddRange(errors{{context.Counter}});
                 {{GetValidatorGotoLabelIfNeeded(effectiveGotoIndent, context)}}{{indent}}    }
                 {{indent}}}
                 """;
    }

    protected string GenerateForEachLoop(
        string index,
        string bodyContent,
        string? hoistLine = null)
    {
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Config.Target!.AccessorExpressionFormat, requestName);
        var itemRequestName = GetItemRequestParameterName();

        return $$"""
               {{hoistLine}}{{Config.Indent}}var {{index}} = 0;
               {{Config.Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
               {{Config.Indent}}{
               {{bodyContent}}
               {{Config.Indent}}    {{index}}++;
               {{Config.Indent}}}
               """;
    }

    protected static string GetValidatorGotoLabelIfNeeded(IndentModel indent, RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                    {indent}        goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }

    protected static (string CallTarget, string? HoistLine) ResolveHoistTarget(
        bool hoistValidator,
        string validatorCallTarget,
        IndentModel indent,
        RuleChainContext context)
    {
        if (!hoistValidator)
        {
            return (validatorCallTarget, null);
        }

        var validatorVar = $"validator{context.Counter}";
        return (validatorVar, $"{indent}var {validatorVar} = {validatorCallTarget};\r\n");
    }

    protected static string BuildValidatorMethodCall(
        bool isAsync,
        bool isAsyncCall,
        string callTarget,
        string requestAccessor,
        string targetPath)
    {
        if (isAsync && isAsyncCall)
        {
            return $"await {callTarget}.RunValidationAsync({requestAccessor}, $\"{{inheritedTargetPath}}{targetPath}.\", cancellationToken)";
        }

        return $"{callTarget}.RunValidation({requestAccessor}, $\"{{inheritedTargetPath}}{targetPath}.\")";
    }

    private void AppendGotoLabelIfNeeded(
        StringBuilder code,
        RuleChainContext contextToUse,
        RuleChainContext parentContext)
    {
        if (parentContext.HaltLabel is null && contextToUse.HaltLabel is not null)
        {
            code.Append($"\r\n\r\n{Config.Indent}{contextToUse.HaltLabel}:");
        }
    }
}