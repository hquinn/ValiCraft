using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.StaticAsyncGeneratorTests;

public class SingleEnsureEachRuleChain_MultipleRules_ThenStaticValidateTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
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

                                                                        namespace Test.Validators;

                                                                        [GenerateValidator]
                                                                        public partial class OrderValidator : StaticAsyncValidator<Order>
                                                                        {
                                                                            protected override void DefineRules(IAsyncValidationRuleBuilder<Order> orderBuilder)
                                                                            {
                                                                                orderBuilder.EnsureEach(x => x.LineItems)
                                                                                   .IsNotNull()
                                                                                   .Is(x => x.Quantity > 0)
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

                                                                        namespace Test.Validators
                                                                        {
                                                                            /// <summary>
                                                                            /// Generated static validator for <see cref="global::Test.Requests.LineItem"/>.
                                                                            /// </summary>
                                                                            public partial class LineItemValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.LineItem>
                                                                            {
                                                                                /// <inheritdoc />
                                                                                public static global::ValiCraft.ValidationErrors? Validate(global::Test.Requests.LineItem request)
                                                                                {
                                                                                    var errors = RunValidationLogic(request, null);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ValiCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "LineItemErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ValiCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <inheritdoc />
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static global::System.Collections.Generic.List<global::ValiCraft.ValidationError>? RunValidation(global::Test.Requests.LineItem request, string? inheritedTargetPath)
                                                                                {
                                                                                    return RunValidationLogic(request, inheritedTargetPath);
                                                                                }

                                                                                private static global::System.Collections.Generic.List<global::ValiCraft.ValidationError>? RunValidationLogic(global::Test.Requests.LineItem request, string? inheritedTargetPath)
                                                                                {
                                                                                    global::System.Collections.Generic.List<global::ValiCraft.ValidationError>? errors = null;

                                                                                    if (!global::ValiCraft.Rules.NotNullOrWhiteSpace(request.ProductName))
                                                                                    {
                                                                                        errors ??= new(1);
                                                                                        errors.Add(new global::ValiCraft.ValidationError
                                                                                        {
                                                                                            Code = nameof(global::ValiCraft.Rules.NotNullOrWhiteSpace),
                                                                                            Message = $"Product Name must not be null or contain only whitespace.",
                                                                                            Severity = global::ValiCraft.ErrorSeverity.Error,
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

                                                                        namespace Test.Validators
                                                                        {
                                                                            /// <summary>
                                                                            /// Generated static async validator for <see cref="global::Test.Requests.Order"/>.
                                                                            /// </summary>
                                                                            public partial class OrderValidator : global::ValiCraft.IStaticAsyncValidator<global::Test.Requests.Order>
                                                                            {
                                                                                /// <inheritdoc />
                                                                                public static async global::System.Threading.Tasks.Task<global::ValiCraft.ValidationErrors?> ValidateAsync(global::Test.Requests.Order request, global::System.Threading.CancellationToken cancellationToken = default)
                                                                                {
                                                                                    var errors = await RunValidationLogicAsync(request, null, cancellationToken);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ValiCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "OrderErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ValiCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <inheritdoc />
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static async global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::ValiCraft.ValidationError>?> RunValidationAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken)
                                                                                {
                                                                                    return await RunValidationLogicAsync(request, inheritedTargetPath, cancellationToken);
                                                                                }

                                                                                private static async global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::ValiCraft.ValidationError>?> RunValidationLogicAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken)
                                                                                {
                                                                                    global::System.Collections.Generic.List<global::ValiCraft.ValidationError>? errors = null;

                                                                                    var index3 = 0;
                                                                                    foreach (var element in request.LineItems)
                                                                                    {
                                                                                        if (!global::ValiCraft.Rules.NotNull(element))
                                                                                        {
                                                                                            errors ??= new(3);
                                                                                            errors.Add(new global::ValiCraft.ValidationError
                                                                                            {
                                                                                                Code = nameof(global::ValiCraft.Rules.NotNull),
                                                                                                Message = $"Line Items is required.",
                                                                                                Severity = global::ValiCraft.ErrorSeverity.Error,
                                                                                                TargetName = "Line Items",
                                                                                                TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "LineItems[{index3}]." : null)}",
                                                                                                AttemptedValue = element,
                                                                                            });
                                                                                        }
                                                                                        if (!(element.Quantity > 0))
                                                                                        {
                                                                                            errors ??= new(2);
                                                                                            errors.Add(new global::ValiCraft.ValidationError
                                                                                            {
                                                                                                Code = "Is",
                                                                                                Message = $"'Line Items' doesn't satisfy the condition",
                                                                                                Severity = global::ValiCraft.ErrorSeverity.Error,
                                                                                                TargetName = "Line Items",
                                                                                                TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "LineItems[{index3}]." : null)}",
                                                                                                AttemptedValue = element,
                                                                                            });
                                                                                        }
                                                                                        var errors1 = global::Test.Validators.LineItemValidator.RunValidation(element, $"{inheritedTargetPath}LineItems[{index3}].");
                                                                                        if (errors1 is not null)
                                                                                        {
                                                                                            if (errors is null)
                                                                                            {
                                                                                                errors = errors1;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                errors.AddRange(errors1);
                                                                                            }
                                                                                        }
                                                                                        index3++;
                                                                                    }

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
