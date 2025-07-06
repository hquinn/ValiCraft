using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Models;

public record ValidateWithRuleChain(
    ArgumentInfo Property,
    int Depth,
    string ValidatorExpression,
    bool FromCollection) : RuleChain(Property, Depth, 1)
{
    public override string GenerateCodeForRuleChain(ref int assignedErrorsCount)
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
                   {{indent}}if ({{ValidatorExpression}}.Validate({{requestParameter}}).Errors is {} errors{{assignedErrorsCount}})
                   {{indent}}{
                   {{indent}}    if (errors is null)
                   {{indent}}    {
                   {{indent}}        errors = new(errors{{assignedErrorsCount}});
                   {{indent}}    }
                   {{indent}}    else
                   {{indent}}    {
                   {{indent}}        errors.AddRange(errors{{assignedErrorsCount}});
                   {{indent}}    }
                   {{indent}}}
                   """;

        assignedErrorsCount--;
        return code;
    }
}