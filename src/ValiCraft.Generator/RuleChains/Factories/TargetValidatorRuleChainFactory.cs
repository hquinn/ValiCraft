using System.Linq;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetValidatorRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        var validatorInvocation = context.InvocationChain.Skip(1).First();

        var resolution = ValidatorResolutionHelper.Resolve(validatorInvocation, context.GeneratorContext);
        if (resolution is null)
        {
            return null;
        }

        if (context.IsCollection)
        {
            return CreateCollectionChain(context, resolution);
        }

        var config = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            1,
            context.Invocation.GetOnFailureModeFromSyntax());

        return new TargetValidatorRuleChain(
            config,
            resolution.ValidatorCallTarget,
            resolution.IsAsyncValidatorCall);
    }

    private static RuleChain CreateCollectionChain(RuleChainFactoryContext context, ValidatorResolution resolution)
    {
        var childIndent = IndentModel.CreateChild(context.Indent);

        // Create an object-level target for the item within the loop
        var itemTarget = RuleChain.CreateComposedItemTarget(default!, context.Target!);

        var innerConfig = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            itemTarget,
            context.Depth + 1,
            childIndent,
            1,
            null);

        var innerChain = new TargetValidatorRuleChain(
            innerConfig,
            resolution.ValidatorCallTarget,
            resolution.IsAsyncValidatorCall);

        var collectionConfig = new RuleChainConfig(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            1,
            context.Invocation.GetOnFailureModeFromSyntax());

        return new CollectionRuleChain(
            collectionConfig,
            new RuleChain[] { innerChain }.ToEquatableImmutableArray(),
            resolution.HoistValidator ? new HoistInfo(resolution.ValidatorCallTarget) : null);
    }
}
