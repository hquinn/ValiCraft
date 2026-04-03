using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;

namespace ValiCraft.Generator.RuleChains;

public record CollectionStaticValidateRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    OnFailureMode? FailureMode,
    string ValidatorTypeName,
    bool IsAsyncValidatorCall) : RuleChain(IsAsync, Object, Target, Depth, Indent, 1, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var index = $"index{context.Counter}";
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var itemRequestName = GetItemRequestParameterName();
        var childIndent = IndentModel.CreateChild(Indent);

        var methodCall = BuildValidatorMethodCall(IsAsync, IsAsyncValidatorCall, ValidatorTypeName, itemRequestName, $"{Target.TargetPath.Value}[{{{index}}}]");

        // Use a unique variable name suffix (counter) to avoid conflicts when multiple Validate calls exist
        var validatorCallCode = GenerateValidatorCallCode(childIndent, methodCall, context);

        var code = $$"""
                     {{Indent}}var {{index}} = 0;
                     {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
                     {{Indent}}{
                     {{validatorCallCode}}
                     {{Indent}}    {{index}}++;
                     {{Indent}}}
                     """;

        context.DecrementCountdown();
        // Reset because the foreach block breaks any if/else chain — the next chain cannot use 'else if'
        context.ResetIfElseMode();
        return code;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}].";
    }
}
