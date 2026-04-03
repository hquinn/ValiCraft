using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Rules;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record TargetRuleChain(
    RuleChainConfig Config,
    EquatableArray<Rule> Rules,
    CollectionConfig? Collection = null) : RuleChain(Config)
{

    public override bool NeedsGotoLabels()
    {
        // Collection loops have no reliable way (besides break and return) to exit loops early
        // Property Rule Chains themselves don't require the need for goto labels,
        // but they will need to implement goto labels if other rule chains from its parent rule chain do.
        return Collection is not null;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return BuildTargetPath(context.TargetPath, Config.Target!.TargetPath.Value, Collection is not null, context.Counter);
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (Collection is not null)
        {
            return HandleCollectionCodeGeneration(context);
        }

        var ruleCodes = GenerateRulesCode(Rules, GetRequestParameterName(), Config.Indent, Config.Object, Config.Target!, context);

        return string.Join("\r\n", ruleCodes);
    }

    private string HandleCollectionCodeGeneration(RuleChainContext context)
    {
        if (Rules.Count == 0)
        {
            return string.Empty;
        }

        var index = $"index{context.Counter}";
        var childIndent = IndentModel.CreateChild(Config.Indent);

        // Create an object-level target for the item within the loop.
        // The rules operate on each item directly (e.g., element), not on a property.
        // Uses the element type (not the collection type) for correct ValidationError<T> generation.
        var itemTarget = CreateItemTarget(Collection!.ElementType, Config.Target!);

        var ruleCodes = GenerateRulesCode(Rules, GetItemRequestParameterName(), childIndent, Config.Object, itemTarget, context);

        return GenerateForEachLoop(index, string.Join("\r\n", ruleCodes));
    }

}
