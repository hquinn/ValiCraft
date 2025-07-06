using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains;

public record ValidateWithRuleChain(
    int Depth,
    OnFailureMode? FailureMode,
    ArgumentInfo Property,
    string ValidatorExpression,
    bool FromCollection) : RuleChain(Depth, 1, FailureMode)
{

    public override bool NeedsGotoLabels()
    {
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var indent = GetIndent();
        string requestParameter;
        if (FromCollection)
        {
            requestParameter = GetRequestParameterName();
        }
        else
        {
            requestParameter = $"{GetRequestParameterName()}.{Property.Value}";
        }
        
        // A little hacky with the assigned errors, but it's a quick fix to get uniqueness.
        var code = $$"""
                   {{indent}}{{GetIfElseIfKeyword(context)}} ({{ValidatorExpression}}.Validate({{requestParameter}}).Errors is {} errors{{context.Counter}})
                   {{indent}}{
                   {{indent}}    if (errors is null)
                   {{indent}}    {
                   {{indent}}        errors = new(errors{{context.Counter}});
                   {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}    }
                   {{indent}}    else
                   {{indent}}    {
                   {{indent}}        errors.AddRange(errors{{context.Counter}});
                   {{GetGotoLabelIfNeeded(indent, context)}}{{indent}}    }
                   {{indent}}}
                   """;

        context.DecrementCountdown();
        context.UpdateIfElseMode();
        return code;
    }

    private string GetIfElseIfKeyword(RuleChainContext context)
    {
        return context.IfElseMode switch
        {
            IfElseMode.ElseIf => "else if",
            _ => "if"
        };
    }

    private string GetGotoLabelIfNeeded(string indent, RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                    {indent}        goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }
}