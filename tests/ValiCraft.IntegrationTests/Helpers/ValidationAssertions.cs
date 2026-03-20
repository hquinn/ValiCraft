using AwesomeAssertions;

namespace ValiCraft.IntegrationTests.Helpers;

/// <summary>
/// Helper methods for asserting validation results with deep property checks.
/// </summary>
public static class ValidationAssertions
{
    /// <summary>
    /// Asserts that validation passed (no errors).
    /// </summary>
    public static void ShouldPass(this ValidationErrors? result)
    {
        result.Should().BeNull("validation should pass with no errors");
    }

    /// <summary>
    /// Asserts that validation failed and returns the errors for further inspection.
    /// </summary>
    public static IReadOnlyList<ValidationError> ShouldFail(this ValidationErrors? result)
    {
        result.Should().NotBeNull("validation should fail with errors");
        result!.Value.Errors.Should().NotBeEmpty("there should be at least one error");
        return result.Value.Errors;
    }

    /// <summary>
    /// Asserts that validation failed with exactly the given number of errors.
    /// </summary>
    public static IReadOnlyList<ValidationError> ShouldFailWith(this ValidationErrors? result, int expectedCount)
    {
        var errors = result.ShouldFail();
        errors.Should().HaveCount(expectedCount, $"expected exactly {expectedCount} validation error(s)");
        return errors;
    }

    /// <summary>
    /// Asserts a single error exists matching the expected properties and returns it.
    /// </summary>
    public static ValidationError ShouldContainError(
        this IReadOnlyList<ValidationError> errors,
        string expectedTargetPath,
        string expectedCode)
    {
        var match = errors.FirstOrDefault(e =>
            e.TargetPath == expectedTargetPath && e.Code == expectedCode);
        match.Should().NotBe(default(ValidationError),
            $"expected an error with TargetPath='{expectedTargetPath}' and Code='{expectedCode}'");
        return match;
    }

    /// <summary>
    /// Asserts a single error exists with the given target path and returns it.
    /// </summary>
    public static ValidationError ShouldContainErrorForPath(
        this IReadOnlyList<ValidationError> errors,
        string expectedTargetPath)
    {
        var match = errors.FirstOrDefault(e => e.TargetPath == expectedTargetPath);
        match.Should().NotBe(default(ValidationError),
            $"expected an error with TargetPath='{expectedTargetPath}'");
        return match;
    }

    /// <summary>
    /// Asserts that no error exists with the given target path.
    /// </summary>
    public static void ShouldNotContainErrorForPath(
        this IReadOnlyList<ValidationError> errors,
        string expectedTargetPath)
    {
        errors.Should().NotContain(e => e.TargetPath == expectedTargetPath,
            $"did not expect an error with TargetPath='{expectedTargetPath}'");
    }

    /// <summary>
    /// Asserts that the error has the expected TargetName.
    /// </summary>
    public static ValidationError WithTargetName(this ValidationError error, string expectedTargetName)
    {
        error.TargetName.Should().Be(expectedTargetName);
        return error;
    }

    /// <summary>
    /// Asserts that the error has the expected severity.
    /// </summary>
    public static ValidationError WithSeverity(this ValidationError error, ErrorSeverity expectedSeverity)
    {
        error.Severity.Should().Be(expectedSeverity);
        return error;
    }

    /// <summary>
    /// Asserts that the error has the expected message.
    /// </summary>
    public static ValidationError WithMessage(this ValidationError error, string expectedMessage)
    {
        error.Message.Should().Be(expectedMessage);
        return error;
    }

    /// <summary>
    /// Asserts that the error has the expected attempted value.
    /// </summary>
    public static ValidationError WithAttemptedValue(this ValidationError error, object? expectedValue)
    {
        error.AttemptedValue.Should().Be(expectedValue);
        return error;
    }
}
