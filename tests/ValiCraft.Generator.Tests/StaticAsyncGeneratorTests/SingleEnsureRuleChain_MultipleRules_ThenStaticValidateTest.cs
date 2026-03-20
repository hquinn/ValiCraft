using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.StaticAsyncGeneratorTests;

public class SingleEnsureRuleChain_MultipleRules_ThenStaticValidateTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public decimal OrderTotal { get; set; }
                                                                 public Customer? Customer { get; set; }
                                                             }

                                                             public class Customer
                                                             {
                                                                 public string? Name { get; set; }
                                                             }
                                                             """;

    [StringSyntax("CSharp")] private const string InputCustomerValidator = """
                                                                        using Test.Requests;
                                                                        using ValiCraft;
                                                                        using ValiCraft.Attributes;
                                                                        using ValiCraft.BuilderTypes;
                                                                        using ErrorCraft;

                                                                        namespace Test.Validators;

                                                                        [GenerateValidator]
                                                                        public partial class CustomerValidator : StaticValidator<Customer>
                                                                        {
                                                                            protected override void DefineRules(IValidationRuleBuilder<Customer> builder)
                                                                            {
                                                                                builder.Ensure(x => x.Name)
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
                                                                                orderBuilder.Ensure(x => x.Customer)
                                                                                   .IsNotNull()
                                                                                   .Is(x => x.Name != null)
                                                                                   .Validate<CustomerValidator>();
                                                                            }
                                                                        }
                                                                        """;

    [StringSyntax("CSharp")] private const string ExpectedCustomerValidator = """
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
                                                                            /// Generated static validator for <see cref="global::Test.Requests.Customer"/>.
                                                                            /// </summary>
                                                                            public partial class CustomerValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.Customer>
                                                                            {
                                                                                /// <inheritdoc />
                                                                                public static global::ErrorCraft.ValidationErrors? Validate(global::Test.Requests.Customer request)
                                                                                {
                                                                                    var errors = RunValidation(request, null);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ErrorCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "CustomerErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <inheritdoc />
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static global::ErrorCraft.ValidationErrors? Validate(global::Test.Requests.Customer request, string? inheritedTargetPath)
                                                                                {
                                                                                    var errors = RunValidation(request, inheritedTargetPath);

                                                                                    if (errors is null) return null;

                                                                                    return new global::ErrorCraft.ValidationErrors
                                                                                    {
                                                                                        Code = "CustomerErrors",
                                                                                        Message = "One or more validation errors occurred.",
                                                                                        Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                        Errors = errors
                                                                                    };
                                                                                }

                                                                                /// <summary>
                                                                                /// Runs the validation logic and returns the raw error list. This method is intended for internal use by nested validators.
                                                                                /// </summary>
                                                                                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                                public static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidation(global::Test.Requests.Customer request, string? inheritedTargetPath)
                                                                                {
                                                                                    global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;

                                                                                    if (!global::ValiCraft.Rules.NotNullOrWhiteSpace(request.Name))
                                                                                    {
                                                                                        errors ??= new(1);
                                                                                        errors.Add(new global::ErrorCraft.ValidationError<string?>
                                                                                        {
                                                                                            Code = nameof(global::ValiCraft.Rules.NotNullOrWhiteSpace),
                                                                                            Message = $"Name must not be null or contain only whitespace.",
                                                                                            Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                            TargetName = "Name",
                                                                                            TargetPath = $"{inheritedTargetPath}Name",
                                                                                            AttemptedValue = request.Name,
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

                                                                                    if (!global::ValiCraft.Rules.NotNull(request.Customer))
                                                                                    {
                                                                                        errors ??= new(3);
                                                                                        errors.Add(new global::ErrorCraft.ValidationError<global::Test.Requests.Customer?>
                                                                                        {
                                                                                            Code = nameof(global::ValiCraft.Rules.NotNull),
                                                                                            Message = $"Customer is required.",
                                                                                            Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                            TargetName = "Customer",
                                                                                            TargetPath = $"{inheritedTargetPath}Customer",
                                                                                            AttemptedValue = request.Customer,
                                                                                        });
                                                                                    }
                                                                                    if (!(request.Customer.Name != null))
                                                                                    {
                                                                                        errors ??= new(2);
                                                                                        errors.Add(new global::ErrorCraft.ValidationError<global::Test.Requests.Customer?>
                                                                                        {
                                                                                            Code = "Is",
                                                                                            Message = $"'Customer' doesn't satisfy the condition",
                                                                                            Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                            TargetName = "Customer",
                                                                                            TargetPath = $"{inheritedTargetPath}Customer",
                                                                                            AttemptedValue = request.Customer,
                                                                                        });
                                                                                    }
                                                                                    var errors1 = global::Test.Validators.CustomerValidator.RunValidation(request.Customer, $"{inheritedTargetPath}Customer.");
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

                                                                                    return errors;
                                                                                }
                                                                            }
                                                                        }
                                                                        """;

    [Fact]
    public void ShouldGenerateStaticAsyncValidator()
    {
        AssertGenerator(inputs: [InputRequests, InputCustomerValidator, InputOrderValidator],
            outputs: [ExpectedCustomerValidator, ExpectedOrderValidator],
            diagnostics: [],
            assertInitialCompilation: true);
    }
}
