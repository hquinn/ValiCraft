namespace ValiCraft.Generator.RuleChains.Factories;

public interface IRuleChainFactory
{
    RuleChain? Create(RuleChainFactoryContext context);
}