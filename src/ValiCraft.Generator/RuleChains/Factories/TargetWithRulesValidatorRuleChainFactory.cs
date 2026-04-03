using System.Linq;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetWithRulesValidatorRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        // The last invocation is the validator call — extract validator info
        var validatorInvocation = context.InvocationChain.Last();

        var resolution = ValidatorResolutionHelper.Resolve(validatorInvocation, context.GeneratorContext);
        if (resolution is null)
        {
            return null;
        }

        // Process rules from index 1 to N-2 (skip Ensure/EnsureEach and the validator call)
        var rules = RuleChainHelper.ProcessRuleInvocations(
            context.IsAsyncValidator,
            context.InvocationChain.Skip(1).Take(context.InvocationChain.Count - 2),
            context.Diagnostics,
            context.GeneratorContext);
        if (rules is null)
        {
            return null;
        }

        if (context.IsCollection)
        {
            return CreateCollectionChain(context, rules, resolution);
        }

        var config = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            rules.Count + 1, // +1 for the validator call
            context.Invocation.GetOnFailureModeFromSyntax());

        return new TargetWithRulesValidatorRuleChain(
            config,
            rules.ToEquatableImmutableArray(),
            resolution.ValidatorCallTarget,
            resolution.IsAsyncValidatorCall);
    }

    private static RuleChain CreateCollectionChain(
        RuleChainFactoryContext context,
        System.Collections.Generic.List<Rules.Rule> rules,
        ValidatorResolution resolution)
    {
        // Resolve the element type from EnsureEach
        var elementTypeInfo = context.Invocation.GetElementTypeInfo(context.GeneratorContext);

        var childIndent = IndentModel.CreateChild(context.Indent);

        // Create an object-level target for the item within the loop
        var itemTarget = RuleChain.CreateComposedItemTarget(elementTypeInfo!, context.Target!);

        var innerConfig = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            itemTarget,
            context.Depth + 1,
            childIndent,
            rules.Count + 1, // +1 for the validator call
            null);

        var innerChain = new TargetWithRulesValidatorRuleChain(
            innerConfig,
            rules.ToEquatableImmutableArray(),
            resolution.ValidatorCallTarget,
            resolution.IsAsyncValidatorCall,
            ValidatorCallGotoIndent: IndentModel.CreateChild(childIndent));

        var collectionConfig = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            rules.Count + 1, // +1 for the validator call
            context.Invocation.GetOnFailureModeFromSyntax());

        return new CollectionRuleChain(
            collectionConfig,
            new RuleChain[] { innerChain }.ToEquatableImmutableArray(),
            resolution.HoistValidator ? new HoistInfo(resolution.ValidatorCallTarget) : null);
    }
}
