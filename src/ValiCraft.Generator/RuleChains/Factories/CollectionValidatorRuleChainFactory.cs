using System.Linq;
using ValiCraft.Generator.Extensions;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionValidatorRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(RuleChainFactoryContext context)
    {
        var validatorInvocation = context.InvocationChain.Skip(1).First();

        var resolution = ValidatorResolutionHelper.Resolve(validatorInvocation, context.GeneratorContext);
        if (resolution is null)
        {
            return null;
        }

        return new CollectionValidatorRuleChain(
            context.IsAsyncValidator,
            context.Object,
            context.Target!,
            context.Depth,
            context.Indent,
            context.Invocation.GetOnFailureModeFromSyntax(),
            resolution.ValidatorCallTarget,
            resolution.IsAsyncValidatorCall,
            HoistValidator: resolution.HoistValidator);
    }
}
