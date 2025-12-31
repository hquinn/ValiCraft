using AwesomeAssertions;
using ErrorCraft;

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
    }

    [Fact]
    public void ValidationError_CanBeCreated_WithOptionalProperties()
    {
        // Act
        var error = new ValidationError<int>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = 42,
            Severity = ErrorSeverity.Warning
        };

        // Assert
        error.Severity.Should().Be(ErrorSeverity.Warning);
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
        error.Should().BeAssignableTo<IValidationError>();
    }

    [Fact]
    public void ValidationError_Metadata_IsNullByDefault()
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
        error.Metadata.Should().BeNull();
    }

    [Fact]
    public void ValidationError_Metadata_CanBeSet()
    {
        // Arrange
        var metadata = new Dictionary<string, object?>
        {
            { "MinLength", 5 },
            { "MaxLength", 10 },
            { "CustomKey", "CustomValue" }
        };

        // Act
        var error = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test",
            Metadata = metadata
        };

        // Assert
        error.Metadata.Should().NotBeNull();
        error.Metadata!["MinLength"].Should().Be(5);
        error.Metadata["MaxLength"].Should().Be(10);
        error.Metadata["CustomKey"].Should().Be("CustomValue");
    }

    [Fact]
    public void ValidationError_IValidationError_Metadata_ReturnsValue()
    {
        // Arrange
        var metadata = new Dictionary<string, object?> { { "Key", "Value" } };
        var error = new ValidationError<string>
        {
            Code = "TEST_ERROR",
            Message = "Test message",
            TargetName = "TestProperty",
            TargetPath = "Root.TestProperty",
            AttemptedValue = "test",
            Metadata = metadata
        };

        // Act
        IValidationError errorInterface = error;

        // Assert
        errorInterface.Metadata.Should().NotBeNull();
        errorInterface.Metadata!["Key"].Should().Be("Value");
    }
}
