using System.Collections.Generic;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;
using TypeInfo = ValiCraft.Generator.Concepts.TypeInfo;

namespace ValiCraft.Generator.RuleChains;

public record CollectionTargetRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    OnFailureMode? FailureMode,
    TypeInfo ElementType,
    EquatableArray<Rule> Rules) : RuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        // Loops have no reliable way (besides break and return) to exit loops early
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (Rules.Count == 0)
        {
            return string.Empty;
        }

        var index = $"index{context.Counter}";
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var itemRequestName = GetItemRequestParameterName();
        var childIndent = IndentModel.CreateChild(Indent);

        // Create an object-level target for the item within the loop.
        // The rules operate on each item directly (e.g., element), not on a property.
        // Uses the element type (not the collection type) for correct ValidationError<T> generation.
        var itemTarget = new ValidationTarget(
            AccessorType: AccessorType.Object,
            AccessorExpressionFormat: "{0}",
            Type: ElementType,
            DefaultTargetName: Target.DefaultTargetName,
            TargetPath: Target.TargetPath);

        var ruleCodes = new List<string>(NumberOfRules);

        foreach (var rule in Rules)
        {
            ruleCodes.Add(rule.GenerateCodeForRule(
                itemRequestName,
                childIndent,
                Object,
                itemTarget,
                context));
            context.DecrementCountdown();
        }

        var ruleChainCodes = string.Join("\r\n", ruleCodes);

        return $$"""
               {{Indent}}var {{index}} = 0;
               {{Indent}}foreach (var {{itemRequestName}} in {{requestAccessor}})
               {{Indent}}{
               {{ruleChainCodes}}
               {{Indent}}    {{index}}++;
               {{Indent}}}
               """;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}]";
    }
}
