namespace ValiCraft.Generator;

public static class KnownNames
{
    public static class Attributes
    {
        public const string DefaultMessageAttribute = "ValiCraft.Attributes.DefaultMessageAttribute";
        public const string DefaultMessage = "ValiCraft.Attributes.DefaultMessage";
        public const string DefaultErrorCodeAttribute = "ValiCraft.Attributes.DefaultErrorCodeAttribute";
        public const string DefaultErrorCode = "ValiCraft.Attributes.DefaultErrorCode";
        public const string GenerateRuleExtensionAttribute = "ValiCraft.Attributes.GenerateRuleExtensionAttribute";
        public const string GenerateValidatorAttribute = "ValiCraft.Attributes.GenerateValidatorAttribute";
        public const string GenerateAsyncValidatorAttribute = "ValiCraft.Attributes.GenerateAsyncValidatorAttribute";
        public const string GenerateAsyncRuleExtensionAttribute = "ValiCraft.Attributes.GenerateAsyncRuleExtensionAttribute";
        public const string MapToValidationRuleAttribute = "ValiCraft.Attributes.MapToValidationRuleAttribute";
        public const string MapToValidationRule = "ValiCraft.Attributes.MapToValidationRule";
        public const string RulePlaceholderAttribute = "ValiCraft.Attributes.RulePlaceholderAttribute";
        public const string RulePlaceholder = "ValiCraft.Attributes.RulePlaceholder";
    }

    public static class Classes
    {
        public const string Validator = "ValiCraft.Validator";
        public const string AsyncValidator = "ValiCraft.AsyncValidator";
    }

    public static class Types
    {
        public const string Result = "MonadCraft.Result";
        public const string ValidationError = "ValiCraft.ValidationError";
    }

    public static class Interfaces
    {
        public const string IValidator = "ValiCraft.IValidator";
        public const string IAsyncValidator = "ValiCraft.IAsyncValidator";
        public const string IValidationRule = "ValiCraft.IValidationRule";
        public const string IAsyncValidationRule = "ValiCraft.IAsyncValidationRule";
        public const string IValidationRuleBuilderType = "ValiCraft.BuilderTypes.IValidationRuleBuilderType";
        public const string IAsyncValidationRuleBuilderType = "ValiCraft.BuilderTypes.IAsyncValidationRuleBuilderType";
        public const string IBuilderType = "ValiCraft.BuilderTypes.IBuilderType";
        public const string IAsyncBuilderType = "ValiCraft.BuilderTypes.IAsyncBuilderType";
        public const string IValidationError = "ValiCraft.IValidationError";
    }

    public static class Methods
    {
        public const string IsValid = "IsValid";
        public const string IsValidAsync = "IsValidAsync";
        public const string DefineRules = "DefineRules";
        public const string Ensure = "Ensure";
        public const string EnsureEach = "EnsureEach";
        public const string ValidateWith = "ValidateWith";
        public const string WithOnFailure = "WithOnFailure";
        public const string If = "If";
        public const string WhenNotNull = "WhenNotNull";
        public const string Either = "Either";
        public const string MustAsync = "MustAsync";
    }

    public static class Enums
    {
        public const string OnFailureMode = "OnFailureMode";
        public const string ErrorSeverity = "ValiCraft.ErrorSeverity";
    }

    public static class Targets
    {
        public const string Must = "Must";
        public const string MustAsync = "MustAsync";
    }
}