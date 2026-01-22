namespace ValiCraft.Generator;

public static class KnownNames
{
    public static class Namespaces
    {
        public const string Base = "ValiCraft";
        public const string Attributes =  $"{Base}.Attributes";
        public const string BuilderTypes =  $"{Base}.BuilderTypes";
        public const string AsyncBuilderTypes =  $"{Base}.AsyncBuilderTypes";
        public const string MonadCraft = "MonadCraft";
        public const string ErrorCraft = "ErrorCraft";
    }

    public static class AttributeNames
    {
        public const string GenerateValidator = "GenerateValidator";
        public const string AsyncGenerateValidator = "AsyncGenerateValidator";
    }
    
    public static class Attributes
    {
        public const string DefaultMessageAttribute = $"{Namespaces.Attributes}.DefaultMessageAttribute";
        public const string DefaultErrorCodeAttribute = $"{Namespaces.Attributes}.DefaultErrorCodeAttribute";
        public const string GenerateValidatorAttribute = $"{Namespaces.Attributes}.{AttributeNames.GenerateValidator}Attribute";
        public const string AsyncGenerateValidatorAttribute = $"{Namespaces.Attributes}.{AttributeNames.AsyncGenerateValidator}Attribute";
        public const string MapToValidationRuleAttribute = $"{Namespaces.Attributes}.MapToValidationRuleAttribute";
        public const string RulePlaceholderAttribute = $"{Namespaces.Attributes}.RulePlaceholderAttribute";
    }

    public static class ClassNames
    {
        public const string Validator = "Validator";
        public const string AsyncValidator = "AsyncValidator";
    }
    
    public static class Classes
    {
        public const string Validator = $"{Namespaces.Base}.{ClassNames.Validator}";
        public const string AsyncValidator = $"{Namespaces.Base}.{ClassNames.AsyncValidator}";
    }

    public static class Types
    {
        public const string Result = $"{Namespaces.MonadCraft}.Result";
        public const string ValidationError = $"{Namespaces.ErrorCraft}.ValidationError";
        public const string ValidationErrors = $"{Namespaces.ErrorCraft}.ValidationErrors";
    }

    public static class Interfaces
    {
        public const string IValidator = $"{Namespaces.Base}.IValidator";
        public const string IAsyncValidator = $"{Namespaces.Base}.IAsyncValidator";
        public const string IValidationRule = $"{Namespaces.Base}.IValidationRule";
        public const string IValidationRuleBuilderType = $"{Namespaces.BuilderTypes}.IValidationRuleBuilderType";
        public const string IAsyncValidationRuleBuilderType = $"{Namespaces.AsyncBuilderTypes}.IAsyncValidationRuleBuilderType";
        public const string IBuilderType = $"{Namespaces.BuilderTypes}.IBuilderType";
        public const string IAsyncBuilderType = $"{Namespaces.AsyncBuilderTypes}.IAsyncBuilderType";
        public const string IValidationError = $"{Namespaces.ErrorCraft}.IValidationError";
        public const string IValidationErrors = $"{Namespaces.ErrorCraft}.IValidationErrors";
        
        public static string GetValidatorInterface(bool isAsync)
        {
            if (isAsync)
            {
                return IAsyncValidator;
            }
            
            return IValidator;
        }
    }

    public static class Methods
    {
        public const string IsValid = "IsValid";
        public const string DefineRules = "DefineRules";
        public const string Ensure = "Ensure";
        public const string EnsureEach = "EnsureEach";
        public const string ValidateWith = "ValidateWith";
        public const string WithOnFailure = "WithOnFailure";
        public const string If = "If";
        public const string Either = "Either";
        public const string Validate = "Validate";
        public const string ValidateAsync = "ValidateAsync";

        public static string GetValidateMethod(bool isAsync)
        {
            if (isAsync)
            {
                return ValidateAsync;
            }
            
            return Validate;
        }
    }

    public static class Enums
    {
        public const string OnFailureMode = "OnFailureMode";
        public const string ErrorSeverity = $"{Namespaces.ErrorCraft}.ErrorSeverity";
    }

    public static class Targets
    {
        public const string Must = "Must";
        public const string MustAsync = "MustAsync";

        public static string GetMustTarget(bool isAsync)
        {
            if (isAsync)
            {
                return MustAsync;
            }
            
            return Must;
        }
    }
}