using System.Linq;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetWithRulesValidatorRuleChainFactory(bool isCollection = false) : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        CollectionConfig? collection = null;
        if (isCollection)
        {
            // Resolve the element type from EnsureEach
            var elementTypeInfo = context.Invocation.GetElementTypeInfo(context.GeneratorContext);
            if (elementTypeInfo is null)
            {
                return null;
            }

            collection = new CollectionConfig(elementTypeInfo);
        }

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
            resolution.IsAsyncValidatorCall,
            collection,
            resolution.HoistValidator);
    }
}
