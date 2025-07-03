using ValiCraft;
using ValiCraft.Attributes;

namespace Sample.Rules;

[GenerateRuleExtension("HasLength")]
[DefaultMessage("'{PropertyName}' must have a length of {Length}.")]
[RulePlaceholder("{Length}", "length")]
public class LengthRule : IValidationRule<string?, int>
{
    public static bool IsValid(string? value, int length) => value?.Length == length;
}