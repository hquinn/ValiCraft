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
        public const string Result = "MonadCraft.Result";
        public const string ValidationError = "ValiCraft.ValidationError";
    }

    public static class Interfaces
    {
        public const string IValidator = "ValiCraft.IValidator";
        public const string IValidationRule = "ValiCraft.IValidationRule";
        public const string IValidationRuleBuilderType = "ValiCraft.BuilderTypes.IValidationRuleBuilderType";
        public const string IBuilderType = "ValiCraft.BuilderTypes.IBuilderType";
        public const string IValidationError = "ValiCraft.IValidationError";
    }

    public static class Methods
    {
        public const string IsValid = "IsValid";
        public const string DefineRules = "DefineRules";
        public const string Ensure = "Ensure";
        public const string EnsureEach = "EnsureEach";
        public const string ValidateWith = "ValidateWith";
        public const string WithOnFailure = "WithOnFailure";
    }

    public static class Enums
    {
        public const string OnFailureMode = "OnFailureMode";
        public const string ErrorSeverity = "MonadCraft.Errors.ErrorSeverity";
    }

    public static class Targets
    {
        public const string Must = "Must";
    }
}