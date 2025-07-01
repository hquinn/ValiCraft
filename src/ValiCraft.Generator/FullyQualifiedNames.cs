namespace ValiCraft.Generator;

public static class FullyQualifiedNames
{
    public static class Attributes
    {
        public const string DefaultMessageAttribute = "ValiCraft.Attributes.DefaultMessageAttribute";
        public const string GenerateRuleExtensionAttribute = "ValiCraft.Attributes.GenerateRuleExtensionAttribute";
        public const string GenerateValidatorAttribute = "ValiCraft.Attributes.GenerateValidatorAttribute";
        public const string MapToValidationRuleAttribute = "ValiCraft.Attributes.MapToValidationRuleAttribute";
        public const string MapToValidationRule = "ValiCraft.Attributes.MapToValidationRule";
    }

    public class Classes
    {
        public const string Validator = "ValiCraft.Validator";
    }

    public class Types
    {
        public const string Validation = "LitePrimitives.Validation";
        public const string Error = "LitePrimitives.Error";
    }

    public class Interfaces
    {
        public const string IValidator = "ValiCraft.IValidator";
        public const string IValidationRule = "ValiCraft.IValidationRule";
        public const string IValidationRuleBuilderType = "ValiCraft.BuilderTypes.IValidationRuleBuilderType";
        public const string IBuilderType = "ValiCraft.BuilderTypes.IBuilderType";
    }
}