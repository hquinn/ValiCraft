using System.Collections.Generic;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

/// <summary>
/// Optional hoisting information for validator calls inside collection loops.
/// When present, the validator expression is captured into a local variable before the loop
/// to avoid repeated evaluation on each iteration.
/// </summary>
public record HoistInfo(string OriginalCallTarget);

public record CollectionRuleChain(
    RuleChainConfig Config,
    EquatableArray<RuleChain> InnerChains,
    HoistInfo? Hoist = null,
    bool IncludeTrailingDot = true) : RuleChain(Config)
{
    public override bool NeedsGotoLabels()
    {
        // Loops have no reliable way (besides break and return) to exit loops early
        return true;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        var indexer = $"index{context.Counter}";
        var suffix = IncludeTrailingDot ? "." : "";
        return $"{context.TargetPath}{Config.Target!.TargetPath.Value}[{{{indexer}}}]{suffix}";
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (InnerChains.Count == 0)
        {
            return string.Empty;
        }

        var index = $"index{context.Counter}";

        string? hoistLine = null;
        var chainsToUse = InnerChains;

        if (Hoist is not null)
        {
            var validatorVar = $"validator{context.Counter}";
            hoistLine = $"{Config.Indent}var {validatorVar} = {Hoist.OriginalCallTarget};\r\n";

            // Replace the call target in the inner chain with the hoisted variable
            chainsToUse = ReplaceValidatorCallTarget(InnerChains, validatorVar);
        }

        var innerCodes = new List<string>(chainsToUse.Count);

        foreach (var chain in chainsToUse)
        {
            innerCodes.Add(chain.GenerateCode(context));
        }

        return GenerateForEachLoop(index, string.Join("\r\n", innerCodes), hoistLine);
    }

    private static EquatableArray<RuleChain> ReplaceValidatorCallTarget(
        EquatableArray<RuleChain> chains,
        string newCallTarget)
    {
        var modified = new RuleChain[chains.Count];

        for (var i = 0; i < chains.Count; i++)
        {
            modified[i] = chains[i] switch
            {
                TargetValidatorRuleChain v => v with { ValidatorCallTarget = newCallTarget },
                TargetWithRulesValidatorRuleChain v => v with { ValidatorCallTarget = newCallTarget },
                _ => chains[i]
            };
        }

        return new EquatableArray<RuleChain>(modified);
    }
}
