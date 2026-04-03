using System.Linq;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        // Skip the Ensure/EnsureEach method as that's not a rule.
        var rules = RuleChainHelper.ProcessRuleInvocations(
            context.IsAsyncValidator, context.InvocationChain.Skip(1), context.Diagnostics, context.GeneratorContext);
        if (rules is null)
        {
            return null;
        }

        if (context.IsCollection)
        {
            return CreateCollectionChain(context, rules);
        }

        var config = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            rules.Count,
            context.Invocation?.GetOnFailureModeFromSyntax());

        return new TargetRuleChain(
            config,
            rules.ToEquatableImmutableArray());
    }

    private static RuleChain CreateCollectionChain(RuleChainFactoryContext context, System.Collections.Generic.List<Rules.Rule> rules)
    {
        // Resolve the element type from the EnsureEach method's TTarget generic argument.
        var elementTypeInfo = context.Invocation.GetElementTypeInfo(context.GeneratorContext);

        var childIndent = IndentModel.CreateChild(context.Indent);

        // Create an object-level target for the item within the loop.
        // Uses an empty TargetPath so the inner chain's GetTargetPath is a passthrough
        // (the collection wrapper has already set up the full path including the indexer).
        var itemTarget = RuleChain.CreateComposedItemTarget(elementTypeInfo!, context.Target!);

        var innerConfig = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            itemTarget,
            context.Depth + 1,
            childIndent,
            rules.Count,
            null);

        var innerChain = new TargetRuleChain(innerConfig, rules.ToEquatableImmutableArray());

        var collectionConfig = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            rules.Count,
            context.Invocation?.GetOnFailureModeFromSyntax());

        // TargetRuleChain doesn't need trailing dot — rules access items directly
        return new CollectionRuleChain(collectionConfig, new RuleChain[] { innerChain }.ToEquatableImmutableArray(), IncludeTrailingDot: false);
    }
}
