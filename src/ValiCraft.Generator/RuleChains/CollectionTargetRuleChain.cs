using System.Collections.Generic;
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
    EquatableArray<Rule> Rules) : CollectionItemRuleChain(IsAsync, Object, Target, Depth, Indent, NumberOfRules, FailureMode)
{
    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (Rules.Count == 0)
        {
            return string.Empty;
        }

        var index = $"index{context.Counter}";
        var childIndent = IndentModel.CreateChild(Indent);

        // Create an object-level target for the item within the loop.
        // The rules operate on each item directly (e.g., element), not on a property.
        // Uses the element type (not the collection type) for correct ValidationError<T> generation.
        var itemTarget = CreateItemTarget(ElementType, Target!);

        var ruleCodes = GenerateRulesCode(Rules, GetItemRequestParameterName(), childIndent, Object, itemTarget, context);

        return GenerateForEachLoop(index, string.Join("\r\n", ruleCodes));
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        return $"{context.TargetPath}{Target!.TargetPath.Value}[{{{indexer}}}]";
    }
}
