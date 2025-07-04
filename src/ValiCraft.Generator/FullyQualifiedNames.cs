namespace ValiCraft.Generator;

public static class KnownNames
{
    public static class Attributes
    {
        public const string DefaultMessageAttribute = "ValiCraft.Attributes.DefaultMessageAttribute";
        public const string DefaultMessage = "ValiCraft.Attributes.DefaultMessage";
        public const string GenerateRuleExtensionAttribute = "ValiCraft.Attributes.GenerateRuleExtensionAttribute";
        public const string GenerateValidatorAttribute = "ValiCraft.Attributes.GenerateValidatorAttribute";
        public const string MapToValidationRuleAttribute = "ValiCraft.Attributes.MapToValidationRuleAttribute";
        public const string MapToValidationRule = "ValiCraft.Attributes.MapToValidationRule";
        public const string RulePlaceholderAttribute = "ValiCraft.Attributes.RulePlaceholderAttribute";
        public const string RulePlaceholder = "ValiCraft.Attributes.RulePlaceholder";
    }

    public static class Classes
    {
        public const string Validator = "ValiCraft.Validator";
    }

    public static class Types
    {
        public const string Validation = "LitePrimitives.Validation";
        public const string Error = "LitePrimitives.Error";
    }

    public static class Interfaces
    {
        public const string IValidator = "ValiCraft.IValidator";
        public const string IValidationRule = "ValiCraft.IValidationRule";
        public const string IValidationRuleBuilderType = "ValiCraft.BuilderTypes.IValidationRuleBuilderType";
        public const string IBuilderType = "ValiCraft.BuilderTypes.IBuilderType";
    }

    public static class Methods
    {
        public const string DefineRules = "DefineRules";
        public const string Ensure = "Ensure";
    }
}