using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.StaticAsyncGeneratorTests;

public class SingleEnsureEachRuleChain_SingleRule_ThenStaticValidate_OnFailureModeHaltTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public decimal OrderTotal { get; set; }
                                                                 public required List<LineItem> LineItems { get; set; }
                                                             }

                                                             public class LineItem
                                                             {
                                                                 public required string ProductName { get; set; }
                                                                 public int Quantity { get; set; }
                                                             }
                                                             """;

    [StringSyntax("CSharp")] private const string InputLineItemValidator = """
                                                                        using Test.Requests;
                                                                        using ValiCraft;
                                                                        using ValiCraft.Attributes;
                                                                        using ValiCraft.BuilderTypes;
                                                                        using ErrorCraft;

                                                                        namespace Test.Validators;

                                                                        [GenerateValidator]
                                                                        public partial class LineItemValidator : StaticValidator<LineItem>
                                                                        {
                                                                            protected override void DefineRules(IValidationRuleBuilder<LineItem> builder)
                                                                            {
                                                                                builder.Ensure(x => x.ProductName)
                                                                                   .IsNotNullOrWhiteSpace();
                                                                            }
                                                                        }
                                                                        """;

    [StringSyntax("CSharp")] private const string InputOrderValidator = """
                                                                        using Test.Requests;
                                                                        using ValiCraft;
                                                                        using ValiCraft.Attributes;
                                                                        using ValiCraft.AsyncBuilderTypes;
                                                                        using ErrorCraft;

                                                                        namespace Test.Validators;

                                                                        [GenerateValidator]
                                                                        public partial class OrderValidator : StaticAsyncValidator<Order>
                                                                        {
                                                                            protected override void DefineRules(IAsyncValidationRuleBuilder<Order> orderBuilder)
                                                                            {
                                                                                orderBuilder.EnsureEach(x => x.LineItems, OnFailureMode.Halt)
                                                                                   .IsNotNull()
                                                                                   .Validate<LineItemValidator>();
                                                                            }
                                                                        }
                                                                        """;

    [StringSyntax("CSharp")] private const string ExpectedLineItemValidator = """
                                                                        // <auto-generated />
                                                                        #nullable enable

                                                                        using Test.Requests;
                                                                        using ValiCraft;
                                                                        using ValiCraft.Attributes;
                                                                        using ValiCraft.BuilderTypes;
                                                                        using ErrorCraft;

                                                                        namespace Test.Validators
                                                                        {
                                                                            /// <summary>
                                                                            /// Generated static validator for <see cref="global::Test.Requests.LineItem"/>.
                                                                            /// </summary>
                                                                            public partial class LineItemValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.LineItem>
                                                                            {
                                                                                /// <inheritdoc />
                                                                                public static global::ErrorCraft.ValidationErrors? Validate(global::Test.Requests.LineItem request)
                                                                                {
                                                                                    var errors = RunValidation(request, null);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ErrorCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "LineItemErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <inheritdoc />
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static global::ErrorCraft.ValidationErrors? Validate(global::Test.Requests.LineItem request, string? inheritedTargetPath)
                                                                                {
                                                                                    var errors = RunValidation(request, inheritedTargetPath);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ErrorCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "LineItemErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <summary>
                                                                                /// Runs the validation logic and returns the raw error list. This method is intended for internal use by nested validators.
                                                                                /// </summary>
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidation(global::Test.Requests.LineItem request, string? inheritedTargetPath)
                                                                                {
                                                                                    global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;

                                                                                    if (!global::ValiCraft.Rules.NotNullOrWhiteSpace(request.ProductName))
                                                                                    {
                                                                                        errors ??= new(1);
                                                                                        errors.Add(new global::ErrorCraft.ValidationError<string>
                                                                                        {
                                                                                            Code = nameof(global::ValiCraft.Rules.NotNullOrWhiteSpace),
                                                                                            Message = $"Product Name must not be null or contain only whitespace.",
                                                                                            Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                            TargetName = "Product Name",
                                                                                            TargetPath = $"{inheritedTargetPath}ProductName",
                                                                                            AttemptedValue = request.ProductName,
                                                                                        });
                                                                                    }

                                                                                    return errors;
                                                                                }
                                                                            }
                                                                        }
                                                                        """;

    [StringSyntax("CSharp")] private const string ExpectedOrderValidator = """
                                                                        // <auto-generated />
                                                                        #nullable enable

                                                                        using Test.Requests;
                                                                        using ValiCraft;
                                                                        using ValiCraft.Attributes;
                                                                        using ValiCraft.AsyncBuilderTypes;
                                                                        using ErrorCraft;

                                                                        namespace Test.Validators
                                                                        {
                                                                            /// <summary>
                                                                            /// Generated static async validator for <see cref="global::Test.Requests.Order"/>.
                                                                            /// </summary>
                                                                            public partial class OrderValidator : global::ValiCraft.IStaticAsyncValidator<global::Test.Requests.Order>
                                                                            {
                                                                                /// <inheritdoc />
                                                                                public static async global::System.Threading.Tasks.Task<global::ErrorCraft.ValidationErrors?> ValidateAsync(global::Test.Requests.Order request, global::System.Threading.CancellationToken cancellationToken = default)
                                                                                {
                                                                                    var errors = await RunValidationAsync(request, null, cancellationToken);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ErrorCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "OrderErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <inheritdoc />
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static async global::System.Threading.Tasks.Task<global::ErrorCraft.ValidationErrors?> ValidateAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken = default)
                                                                                {
                                                                                    var errors = await RunValidationAsync(request, inheritedTargetPath, cancellationToken);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ErrorCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "OrderErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <summary>
                                                                                /// Runs the validation logic and returns the raw error list. This method is intended for internal use by nested validators.
                                                                                /// </summary>
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static async global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>?> RunValidationAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken)
                                                                                {
                                                                                    global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;

                                                                                    var index2 = 0;
                                                                                    foreach (var element in request.LineItems)
                                                                                    {
                                                                                        if (!global::ValiCraft.Rules.NotNull(element))
                                                                                        {
                                                                                            errors ??= new(2);
                                                                                            errors.Add(new global::ErrorCraft.ValidationError<global::Test.Requests.LineItem>
                                                                                            {
                                                                                                Code = nameof(global::ValiCraft.Rules.NotNull),
                                                                                                Message = $"Line Items is required.",
                                                                                                Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                                TargetName = "Line Items",
                                                                                                TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "LineItems[{index2}]." : null)}",
                                                                                                AttemptedValue = element,
                                                                                            });
                                                                                            goto HaltValidation_2;
                                                                                        }
                                                                                        var errors1 = global::Test.Validators.LineItemValidator.RunValidation(element, $"{inheritedTargetPath}LineItems[{index2}].");
                                                                                        if (errors1 is not null)
                                                                                        {
                                                                                            if (errors is null)
                                                                                            {
                                                                                                errors = errors1;
                                                                                                    goto HaltValidation_2;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                errors.AddRange(errors1);
                                                                                                    goto HaltValidation_2;
                                                                                            }
                                                                                        }
                                                                                        index2++;
                                                                                    }

                                                                                    HaltValidation_2:

                                                                                    return errors;
                                                                                }
                                                                            }
                                                                        }
                                                                        """;

    [Fact]
    public void ShouldGenerateStaticAsyncValidator()
    {
        AssertGenerator(inputs: [InputRequests, InputLineItemValidator, InputOrderValidator],
            outputs: [ExpectedLineItemValidator, ExpectedOrderValidator],
            diagnostics: [],
            assertInitialCompilation: true);
    }
}
