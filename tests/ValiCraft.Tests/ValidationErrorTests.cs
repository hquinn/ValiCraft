using AwesomeAssertions;
using MonadCraft.Errors;

namespace ValiCraft.Tests;

public class ValidationErrorTests
{
    [Fact]
    public void ValidationError_CanBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var error = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "invalid value"
        };

        // Assert
        error.Code.Should().Be("TEST_ERROR");
        error.Message.Should().Be("Test message");
        error.TargetName.Should().Be("TestProperty");
        error.TargetPath.Should().Be("Root.TestProperty");
        error.AttemptedValue.Should().Be("invalid value");
        error.Severity.Should().Be(ErrorSeverity.Info);
        error.Cause.Should().BeNull();
    }

    [Fact]
    public void ValidationError_CanBeCreated_WithOptionalProperties()
    {
        // Arrange
        var cause = new TestError();

        // Act
        var error = new ValidationError<int>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = 42,
            Severity = ErrorSeverity.Warning,
            Cause = cause
        };

        // Assert
        error.Severity.Should().Be(ErrorSeverity.Warning);
        error.Cause.Should().Be(cause);
    }

    [Fact]
    public void ValidationError_AttemptedValue_ReturnsGenericValue()
    {
        // Arrange
        var error = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test value"
        };

        // Act & Assert
        error.AttemptedValue.Should().Be("test value");
    }

    [Fact]
    public void ValidationError_IValidationError_AttemptedValue_ReturnsAsObject()
    {
        // Arrange
        var error = new ValidationError<int>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = 42
        };

        // Act
        IValidationError errorInterface = error;

        // Assert
        errorInterface.AttemptedValue.Should().Be(42);
    }

    [Fact]
    public void ValidationError_WithNullableType_CanHaveNullAttemptedValue()
    {
        // Arrange & Act
        var error = new ValidationError<string?>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = null
        };

        // Assert
        error.AttemptedValue.Should().BeNull();
    }

    [Fact]
    public void ValidationError_Equality_WorksCorrectly()
    {
        // Arrange
        var error1 = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test"
        };

        var error2 = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test"
        };

        // Act & Assert
        error1.Should().Be(error2);
    }

    [Fact]
    public void ValidationError_Inequality_WorksCorrectly()
    {
        // Arrange
        var error1 = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test"
        };

        var error2 = new ValidationError<string>
        {
            Code = "DIFFERENT_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test"
        };

        // Act & Assert
        error1.Should().NotBe(error2);
    }

    [Fact]
    public void ValidationError_ImplementsIValidationError()
    {
        // Arrange & Act
        var error = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test"
        };

        // Assert
        error.Should().BeAssignableTo<IValidationError>();
    }

    [Fact]
    public void ValidationError_ImplementsIError()
    {
        // Arrange & Act
        var error = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test"
        };

        // Assert
        error.Should().BeAssignableTo<IError>();
    }

    private class TestError : IError
    {
        public string Code => "TEST_CAUSE";
        public string Message => "Test cause";
        public IError? Cause => null;
        public ErrorSeverity Severity => ErrorSeverity.Error;
        public string TargetPath => "test";
    }
}
