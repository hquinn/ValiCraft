using System.Diagnostics.CodeAnalysis;
using MonadCraft;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

/// <summary>
/// Tests for weak semantic type resolution - the scenario where validation rules
/// are defined in the same project as validators, and the generator must resolve
/// types without full semantic information from the compiler.
/// </summary>
public class WeakSemanticTypeResolutionTests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    #region Common Inputs

    [StringSyntax("CSharp")]
    private const string CommonRequestTypes = """
        using System;
        using System.Collections.Generic;

        namespace Test.Models;

        public class TestModel
        {
            // String properties
            public required string Name { get; set; }
            public string? NullableName { get; set; }
            
            // Numeric properties
            public int Age { get; set; }
            public int? NullableAge { get; set; }
            public decimal Price { get; set; }
            public decimal? NullablePrice { get; set; }
            public double Score { get; set; }
            public long BigNumber { get; set; }
            
            // DateTime properties
            public DateTime CreatedAt { get; set; }
            public DateTime? NullableCreatedAt { get; set; }
            
            // Collection properties
            public required List<string> Tags { get; set; }
            public required IEnumerable<int> Numbers { get; set; }
            public string[]? NullableArray { get; set; }
            
            // Guid properties
            public Guid Id { get; set; }
            public Guid? NullableId { get; set; }
            
            // Enum properties
            public Status Status { get; set; }
            
            // Nested object
            public NestedModel? Nested { get; set; }
        }

        public class NestedModel
        {
            public required string Value { get; set; }
        }

        public enum Status
        {
            Active,
            Inactive
        }
        """;

    #endregion

    #region Test: Simple non-generic rule (string? -> bool)

    [StringSyntax("CSharp")]
    private const string SimpleStringRule = """
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsNotEmpty")]
        [DefaultMessage("{TargetName} must not be empty")]
        public class NotEmpty : IValidationRule<string?>
        {
            public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
        }
        """;

    [StringSyntax("CSharp")]
    private const string SimpleStringRuleValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class SimpleStringValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Should work: string property with string? rule
                builder.Ensure(x => x.Name).IsNotEmpty();
                
                // Should work: nullable string property with string? rule
                builder.Ensure(x => x.NullableName).IsNotEmpty();
            }
        }
        """;

    [Fact]
    public void SimpleStringRule_ShouldResolveCorrectly()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, SimpleStringRule, SimpleStringRuleValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Generic rule with IComparable constraint

    [StringSyntax("CSharp")]
    private const string GenericComparableRule = """
        using System;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsGreaterThan")]
        [DefaultMessage("{TargetName} must be greater than {MinValue}")]
        [RulePlaceholder("{MinValue}", "minValue")]
        public class GreaterThan<T> : IValidationRule<T, T>
            where T : IComparable<T>
        {
            public static bool IsValid(T value, T minValue) => value.CompareTo(minValue) > 0;
        }
        """;

    [StringSyntax("CSharp")]
    private const string GenericComparableRuleValidator = """
        using System;
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class ComparableValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Should work: int with int literal
                builder.Ensure(x => x.Age).IsGreaterThan(0);
                
                // Should work: decimal with decimal literal
                builder.Ensure(x => x.Price).IsGreaterThan(0.0M);
                
                // Should work: double with double literal
                builder.Ensure(x => x.Score).IsGreaterThan(0.0);
                
                // Should work: DateTime with DateTime
                builder.Ensure(x => x.CreatedAt).IsGreaterThan(DateTime.MinValue);
            }
        }
        """;

    [Fact]
    public void GenericComparableRule_ShouldResolveForDifferentTypes()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, GenericComparableRule, GenericComparableRuleValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Rule with collection parameter (IsIn)

    [StringSyntax("CSharp")]
    private const string CollectionParameterRule = """
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsIn")]
        [DefaultMessage("{TargetName} must be one of the allowed values")]
        public class In<T> : IValidationRule<T, IEnumerable<T>>
            where T : IEquatable<T>
        {
            public static bool IsValid(T value, IEnumerable<T> allowedValues)
                => allowedValues.Contains(value);
        }
        """;

    [StringSyntax("CSharp")]
    private const string CollectionParameterRuleValidator = """
        using System.Collections.Generic;
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class CollectionParamValidator : Validator<TestModel>
        {
            private static readonly List<string> AllowedNames = ["Alice", "Bob"];
            
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Should work: string property with IEnumerable<string>
                builder.Ensure(x => x.Name).IsIn(AllowedNames);
                
                // Should work: inline array
                builder.Ensure(x => x.Name).IsIn(new[] { "Alice", "Bob" });
            }
        }
        """;

    [Fact]
    public void CollectionParameterRule_ShouldResolveCorrectly()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, CollectionParameterRule, CollectionParameterRuleValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Rule targeting collection property (IsUnique)

    [StringSyntax("CSharp")]
    private const string CollectionTargetRule = """
        using System.Collections.Generic;
        using System.Linq;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsUnique")]
        [DefaultMessage("{TargetName} must contain only unique items")]
        public class Unique<T> : IValidationRule<IEnumerable<T>?>
        {
            public static bool IsValid(IEnumerable<T>? value)
            {
                if (value == null) return true;
                var list = value.ToList();
                return list.Count == list.Distinct().Count();
            }
        }
        """;

    [StringSyntax("CSharp")]
    private const string CollectionTargetRuleValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class CollectionTargetValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Should work: List<string> matches IEnumerable<T>?
                builder.Ensure(x => x.Tags).IsUnique();
                
                // Should work: IEnumerable<int> matches IEnumerable<T>?
                builder.Ensure(x => x.Numbers).IsUnique();
            }
        }
        """;

    [Fact]
    public void CollectionTargetRule_ShouldMatchDifferentCollectionTypes()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, CollectionTargetRule, CollectionTargetRuleValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Rule with Func<T, bool> parameter (predicate)

    [StringSyntax("CSharp")]
    private const string PredicateRule = """
        using System;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("Satisfies")]
        [DefaultMessage("{TargetName} does not satisfy the condition")]
        public class Predicate<T> : IValidationRule<T, Func<T, bool>>
        {
            public static bool IsValid(T value, Func<T, bool> predicate) => predicate(value);
        }
        """;

    [StringSyntax("CSharp")]
    private const string PredicateRuleValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class PredicateValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Should work: lambda expression
                builder.Ensure(x => x.Name).Satisfies(n => n.StartsWith("A"));
                
                // Should work: lambda with block body
                builder.Ensure(x => x.Age).Satisfies(age => { return age >= 18; });
                
                // Should work: for nullable types
                builder.Ensure(x => x.NullableName).Satisfies(n => n == null || n.Length > 0);
            }
        }
        """;

    [Fact]
    public void PredicateRule_ShouldMatchLambdaExpressions()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, PredicateRule, PredicateRuleValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Multiple rules with same name but different signatures (overloads)

    [StringSyntax("CSharp")]
    private const string OverloadedRules = """
        using System.Collections.Generic;
        using System.Linq;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        // Overload 1: Simple contains check
        [GenerateRuleExtension("Contains")]
        [DefaultMessage("{TargetName} must contain '{Value}'")]
        [RulePlaceholder("{Value}", "value")]
        public class ContainsString : IValidationRule<string?, string>
        {
            public static bool IsValid(string? target, string value)
                => target?.Contains(value) ?? false;
        }

        // Overload 2: Collection contains
        [GenerateRuleExtension("Contains")]
        [DefaultMessage("{TargetName} must contain the item")]
        [RulePlaceholder("{Item}", "item")]
        public class ContainsItem<T> : IValidationRule<IEnumerable<T>?, T>
        {
            public static bool IsValid(IEnumerable<T>? target, T item)
                => target?.Contains(item) ?? false;
        }
        """;

    [StringSyntax("CSharp")]
    private const string OverloadedRulesValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class OverloadValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Should resolve to ContainsString (string?, string)
                builder.Ensure(x => x.Name).Contains("test");
                
                // Should resolve to ContainsItem<string> (IEnumerable<string>?, string)
                builder.Ensure(x => x.Tags).Contains("important");
            }
        }
        """;

    [Fact]
    public void OverloadedRules_ShouldResolveCorrectOverload()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, OverloadedRules, OverloadedRulesValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Type mismatch detection (VALC207)

    [StringSyntax("CSharp")]
    private const string StringOnlyRule = """
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsEmailAddress")]
        [DefaultMessage("{TargetName} must be a valid email")]
        public class Email : IValidationRule<string?>
        {
            public static bool IsValid(string? value)
                => value != null && value.Contains('@');
        }
        """;

    [StringSyntax("CSharp")]
    private const string TypeMismatchValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class TypeMismatchValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // ERROR: Applying string rule to int property
                builder.Ensure(x => x.Age).IsEmailAddress();
            }
        }
        """;

    [Fact]
    public void TypeMismatch_ShouldReportDiagnostic()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, StringOnlyRule, TypeMismatchValidator],
            outputs: null,
            diagnostics: ["'IsEmailAddress' expects 'string?' but property is of type 'int'. Consider using a numeric or comparison validation rule instead."]);
    }

    #endregion

    #region Test: Nullable type handling

    [StringSyntax("CSharp")]
    private const string NullableHandlingRule = """
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsPositive")]
        [DefaultMessage("{TargetName} must be positive")]
        public class Positive : IValidationRule<int>
        {
            public static bool IsValid(int value) => value > 0;
        }

        [GenerateRuleExtension("IsPositiveNullable")]
        [DefaultMessage("{TargetName} must be positive")]
        public class PositiveNullable : IValidationRule<int?>
        {
            public static bool IsValid(int? value) => value is > 0;
        }
        """;

    [StringSyntax("CSharp")]
    private const string NullableHandlingValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class NullableValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Non-nullable property with non-nullable rule
                builder.Ensure(x => x.Age).IsPositive();
                
                // Nullable property with nullable rule
                builder.Ensure(x => x.NullableAge).IsPositiveNullable();
            }
        }
        """;

    [Fact]
    public void NullableTypes_ShouldResolveCorrectly()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, NullableHandlingRule, NullableHandlingValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Nested generic types (e.g., IEnumerable<IEnumerable<T>>)

    [StringSyntax("CSharp")]
    private const string NestedGenericRule = """
        using System.Collections.Generic;
        using System.Linq;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("HasMinCount")]
        [DefaultMessage("{TargetName} must have at least {MinCount} items")]
        [RulePlaceholder("{MinCount}", "minCount")]
        public class MinCount<T> : IValidationRule<IEnumerable<T>?, int>
        {
            public static bool IsValid(IEnumerable<T>? value, int minCount)
                => value?.Count() >= minCount;
        }
        """;

    [StringSyntax("CSharp")]
    private const string NestedGenericValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class NestedGenericValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // List<string> should match IEnumerable<T>? where T = string
                builder.Ensure(x => x.Tags).HasMinCount(1);
                
                // IEnumerable<int> should match IEnumerable<T>? where T = int  
                builder.Ensure(x => x.Numbers).HasMinCount(1);
            }
        }
        """;

    [Fact]
    public void NestedGenericTypes_ShouldResolveCorrectly()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, NestedGenericRule, NestedGenericValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Variable/field reference as argument

    [StringSyntax("CSharp")]
    private const string VariableArgumentRule = """
        using System;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsBetween")]
        [DefaultMessage("{TargetName} must be between {Min} and {Max}")]
        [RulePlaceholder("{Min}", "min")]
        [RulePlaceholder("{Max}", "max")]
        public class Between<T> : IValidationRule<T, T, T>
            where T : IComparable<T>
        {
            public static bool IsValid(T value, T min, T max)
                => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }
        """;

    [StringSyntax("CSharp")]
    private const string VariableArgumentValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class VariableArgValidator : Validator<TestModel>
        {
            private const int MinAge = 0;
            private const int MaxAge = 150;
            private readonly decimal _minPrice = 0.01M;
            
            private decimal MaxPrice => 9999.99M;
            
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Using const fields
                builder.Ensure(x => x.Age).IsBetween(MinAge, MaxAge);
                
                // Using readonly field and property
                builder.Ensure(x => x.Price).IsBetween(_minPrice, MaxPrice);
                
                // Using literals
                builder.Ensure(x => x.Score).IsBetween(0.0, 100.0);
            }
        }
        """;

    [Fact]
    public void VariableArguments_ShouldResolveTypesCorrectly()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, VariableArgumentRule, VariableArgumentValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion

    #region Test: Mixed weak and rich semantic modes in same validator

    [StringSyntax("CSharp")]
    private const string WeakSemanticRule = """
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        [GenerateRuleExtension("IsNotBlank")]
        [DefaultMessage("{TargetName} must not be blank")]
        public class NotBlank : IValidationRule<string?>
        {
            public static bool IsValid(string? value) => !string.IsNullOrWhiteSpace(value);
        }
        """;

    [StringSyntax("CSharp")]
    private const string RichSemanticRule = """
        using ValiCraft;
        using ValiCraft.Attributes;
        using ValiCraft.BuilderTypes;

        namespace Test.Rules;

        // Already generated extension (simulates rule from different project)
        [DefaultMessage("{TargetName} must not be null")]
        public class NotNullRule<T> : IValidationRule<T?>
        {
            public static bool IsValid(T? value) => value is not null;
        }

        [DefaultMessage("{TargetName} must not be null")]
        public static class NotNullRuleExtensions
        {
            [MapToValidationRule(typeof(NotNullRule<>), "<{0}>")]
            public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotNull<TRequest, TTargetType>(
                this IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                => throw new System.NotImplementedException();
        }
        """;

    [StringSyntax("CSharp")]
    private const string MixedModeValidator = """
        using Test.Models;
        using Test.Rules;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Validators;

        [GenerateValidator]
        public partial class MixedModeValidator : Validator<TestModel>
        {
            protected override void DefineRules(IValidationRuleBuilder<TestModel> builder)
            {
                // Rich semantic: extension already exists
                builder.Ensure(x => x.Name).IsNotNull();
                
                // Weak semantic: extension being generated in same compilation
                builder.Ensure(x => x.Name).IsNotBlank();
                
                // Chained: rich then weak
                builder.Ensure(x => x.NullableName)
                    .IsNotNull()
                    .IsNotBlank();
            }
        }
        """;

    [Fact]
    public void MixedSemanticModes_ShouldWorkTogether()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName],
            inputs: [CommonRequestTypes, WeakSemanticRule, RichSemanticRule, MixedModeValidator],
            outputs: null,
            diagnostics: []);
    }

    #endregion
}
