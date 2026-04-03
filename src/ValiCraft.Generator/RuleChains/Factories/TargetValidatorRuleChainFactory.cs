using System.Linq;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

public class TargetValidatorRuleChainFactory(bool isCollection = false) : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        var validatorInvocation = context.InvocationChain.Skip(1).First();

        var resolution = ValidatorResolutionHelper.Resolve(validatorInvocation, context.GeneratorContext);
        if (resolution is null)
        {
            return null;
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
            resolution.IsAsyncValidatorCall,
            Collection: isCollection ? new CollectionConfig(default!) : null,
            HoistValidator: resolution.HoistValidator);
    }
}
