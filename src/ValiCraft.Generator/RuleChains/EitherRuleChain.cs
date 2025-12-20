using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

/// <summary>
/// Represents an Either rule chain that validates one of multiple rule sets passes (OR logic).
/// At least one of the child rule sets must pass for the validation to succeed.
/// </summary>
public record EitherRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    int Depth,
    IndentModel Indent,
    int NumberOfRules,
    EquatableArray<EquatableArray<RuleChain>> RuleSets,
    MessageInfo? OverrideMessage,
    MessageInfo? OverrideErrorCode) : RuleChain(IsAsync, Object, null, Depth, Indent, NumberOfRules, null)
{
    protected override bool TryLinkRuleChain(
        ValidationRule[] validRules,
        SourceProductionContext context,
        out RuleChain linkedRuleChain)
    {
        var linkedRuleSets = new List<EquatableArray<RuleChain>>(RuleSets.Count);

        foreach (var ruleSet in RuleSets)
        {
            var linkedRuleChains = new List<RuleChain>(ruleSet.Count);
            
            foreach (var ruleChain in ruleSet)
            {
                if (!ruleChain.TryLinkRuleChain(linkedRuleChains, validRules, context))
                {
                    linkedRuleChain = this;
                    return false;
                }
            }
            
            linkedRuleSets.Add(linkedRuleChains.ToEquatableImmutableArray());
        }

        linkedRuleChain = this with
        {
            RuleSets = linkedRuleSets.ToEquatableImmutableArray()
        };
        
        return true;
    }

    public override bool NeedsGotoLabels()
    {
        // Either chains don't propagate goto labels since they handle their own control flow
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        if (RuleSets.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        var requestParam = GetRequestParameterName();
        var successLabel = "eitherSuccess_" + context.Counter;
        
        sb.AppendLine(Indent + "// Either validation - at least one rule set must pass");
        
        for (var i = 0; i < RuleSets.Count; i++)
        {
            var ruleSet = RuleSets[i];
            var setVarName = "eitherSet" + i + "Errors";
            
            // Create a separate context for each rule set
            var setContext = new RuleChainContext(ruleSet.Sum(rc => rc.NumberOfRules));
            setContext.TargetPath = context.TargetPath;
            
            var ruleSetCode = string.Join("\r\n", ruleSet.Select(x => x.GenerateCode(setContext)));
            
            sb.AppendLine(Indent + "{");
            sb.AppendLine(Indent + "    global::System.Collections.Generic.List<global::" + KnownNames.Interfaces.IValidationError + ">? " + setVarName + " = null;");
            sb.AppendLine(Indent + "    var originalErrors = errors;");
            sb.AppendLine(Indent + "    errors = null;");
            sb.AppendLine(ruleSetCode);
            sb.AppendLine(Indent + "    " + setVarName + " = errors;");
            sb.AppendLine(Indent + "    errors = originalErrors;");
            sb.AppendLine(Indent + "    if (" + setVarName + " is null or { Count: 0 })");
            sb.AppendLine(Indent + "    {");
            sb.AppendLine(Indent + "        goto " + successLabel + ";");
            sb.AppendLine(Indent + "    }");
            sb.AppendLine(Indent + "}");
        }

        var errorCode = OverrideErrorCode?.Value ?? "EITHER_VALIDATION_FAILED";
        var errorMessage = OverrideMessage?.Value ?? "At least one validation group must pass";
        var errorCodeStr = OverrideErrorCode?.IsLiteral == true ? "\"" + errorCode + "\"" : errorCode;
        var errorMessageStr = OverrideMessage?.IsLiteral == true ? "\"" + errorMessage + "\"" : errorMessage;

        sb.AppendLine(Indent + "// All rule sets failed - add error");
        sb.AppendLine(Indent + "{");
        sb.AppendLine(Indent + "    errors ??= new(" + context.Counter + ");");
        sb.AppendLine(Indent + "    errors.Add(new global::" + KnownNames.Types.ValidationError + "<" + Object.Type.FormattedTypeName + ">");
        sb.AppendLine(Indent + "    {");
        sb.AppendLine(Indent + "        Code = " + errorCodeStr + ",");
        sb.AppendLine(Indent + "        Message = " + errorMessageStr + ",");
        sb.AppendLine(Indent + "        Severity = global::" + KnownNames.Enums.ErrorSeverity + ".Error,");
        sb.AppendLine(Indent + "        TargetName = \"" + Object.DefaultTargetName.Value + "\",");
        sb.AppendLine(Indent + "        TargetPath = $\"{inheritedTargetPath}" + context.TargetPath + "\",");
        sb.AppendLine(Indent + "        AttemptedValue = " + requestParam + ",");
        sb.AppendLine(Indent + "    });");
        sb.AppendLine(Indent + "}");
        sb.AppendLine(Indent + successLabel + ":;");

        context.DecrementCountdown();
        return sb.ToString();
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return context.TargetPath;
    }
}
