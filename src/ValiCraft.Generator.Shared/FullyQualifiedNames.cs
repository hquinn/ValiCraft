namespace ValiCraft.Generator.Shared;

public static class FullyQualifiedNames
{
    public static class Attributes
    {
        public const string GenerateValidatorAttribute = "ValiCraft.Abstractions.Attributes.GenerateValidatorAttribute";
        public const string GenerateRuleExtensionAttribute = "ValiCraft.Abstractions.Attributes.GenerateRuleExtensionAttribute";
        public const string MapToValidationRuleAttribute = "ValiCraft.Abstractions.Attributes.MapToValidationRuleAttribute";
        public const string MapToValidationRule = "ValiCraft.Abstractions.Attributes.MapToValidationRule";
    }

    public class Classes
    {
        public const string Validator = "ValiCraft.Abstractions.Validator";
    }

    public class Types
    {
        public const string Validation = "LitePrimitives.Validation";
        public const string Error = "LitePrimitives.Error";
    }

    public class Interfaces
    {
        public const string IValidator = "ValiCraft.Providers.LitePrimitives.IValidator";
        public const string IValidationRule = "ValiCraft.Abstractions.IValidationRule";
        public const string IValidationRuleBuilderType = "ValiCraft.Abstractions.BuilderTypes.IValidationRuleBuilderType";
        public const string IBuilderType = "ValiCraft.Abstractions.BuilderTypes.IBuilderType";
    }
}