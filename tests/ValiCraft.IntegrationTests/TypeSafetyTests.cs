using AwesomeAssertions;
using ValiCraft.IntegrationTests.Models;
using ValiCraft.IntegrationTests.Validators;

namespace ValiCraft.IntegrationTests;

/// <summary>
/// Integration tests for struct validation.
/// </summary>
public class StructValidationTests
{
    [Fact]
    public void Struct_ValidData_ReturnsNoErrors()
    {
        var validator = new CoordinateValidator();
        var model = new Coordinate { Latitude = 45.0, Longitude = -93.0 };

        var errors = validator.ValidateToList(model);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Struct_InvalidData_ReturnsErrors()
    {
        var validator = new CoordinateValidator();
        var model = new Coordinate { Latitude = 100.0, Longitude = -200.0 };

        var errors = validator.ValidateToList(model);

        errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Struct_MethodTarget_ValidData_ReturnsNoErrors()
    {
        var validator = new RectangleValidator();
        var model = new Rectangle { Width = 5.0, Height = 3.0 };

        var errors = validator.ValidateToList(model);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Struct_MethodTarget_InvalidData_ReturnsErrors()
    {
        var validator = new RectangleValidator();
        var model = new Rectangle { Width = 0.0, Height = 0.0 };

        var errors = validator.ValidateToList(model);

        errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}

/// <summary>
/// Integration tests for method target validation.
/// </summary>
public class MethodTargetValidationTests
{
    [Fact]
    public void MethodTarget_ValidData_ReturnsNoErrors()
    {
        var validator = new CustomerValidator();
        var model = new Customer { FirstName = "John", LastName = "Doe", Balance = 100m };

        var errors = validator.ValidateToList(model);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void MethodTarget_InvalidMethodResult_ReturnsErrors()
    {
        var validator = new CustomerValidator();
        var model = new Customer { FirstName = null, LastName = null, Balance = 0m };

        var errors = validator.ValidateToList(model);

        errors.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void MethodTarget_WithParameters_ValidData_ReturnsNoErrors()
    {
        var validator = new CustomerValidator();
        var model = new Customer { FirstName = "Jane", LastName = "Smith", Balance = 1000m };

        var errors = validator.ValidateToList(model);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void MethodTarget_WithParameters_InvalidResult_ReturnsErrors()
    {
        var validator = new CustomerValidator();
        var model = new Customer { FirstName = "Jane", LastName = "Smith", Balance = -50m };

        var errors = validator.ValidateToList(model);

        errors.Should().NotBeEmpty();
    }
}

/// <summary>
/// Integration tests for type safety with non-nullable string properties.
/// These tests verify that the nullability fixes work correctly.
/// </summary>
public class TypeSafetyTests
{
    [Fact]
    public void NonNullableModel_ValidData_ReturnsNoErrors()
    {
        // Arrange
        var validator = new NonNullableModelValidator();
        var model = new NonNullableModel
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void NonNullableModel_InvalidData_ReturnsErrors()
    {
        // Arrange
        var validator = new NonNullableModelValidator();
        var model = new NonNullableModel
        {
            Name = "J", // Too short (min 2)
            Email = "not-an-email",
            Age = -5 // Negative (must be > 0)
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void NonNullableModel_EmptyName_ReturnsError()
    {
        // Arrange
        var validator = new NonNullableModelValidator();
        var model = new NonNullableModel
        {
            Name = "", // Empty, should fail IsNotNullOrWhiteSpace
            Email = "test@example.com",
            Age = 25
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.TargetName == "Name");
    }

    [Fact]
    public void NonNullableModel_WhitespaceName_ReturnsError()
    {
        // Arrange
        var validator = new NonNullableModelValidator();
        var model = new NonNullableModel
        {
            Name = "   ", // Whitespace only
            Email = "test@example.com",
            Age = 25
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().NotBeEmpty();
    }
}

/// <summary>
/// Integration tests for nested validation.
/// </summary>
public class NestedValidationTests
{
    [Fact]
    public void ParentModel_ValidNestedChild_ReturnsNoErrors()
    {
        // Arrange
        var validator = new ParentModelValidator();
        var model = new ParentModel
        {
            Name = "Parent",
            Child = new ChildModel
            {
                Description = "Valid child",
                Value = 10
            }
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ParentModel_InvalidNestedChild_ReturnsNestedErrors()
    {
        // Arrange
        var validator = new ParentModelValidator();
        var model = new ParentModel
        {
            Name = "Parent",
            Child = new ChildModel
            {
                Description = "", // Invalid
                Value = -1 // Invalid (must be > 0)
            }
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void ParentModel_InvalidParentAndChild_ReturnsAllErrors()
    {
        // Arrange
        var validator = new ParentModelValidator();
        var model = new ParentModel
        {
            Name = "", // Invalid
            Child = new ChildModel
            {
                Description = "", // Invalid
                Value = 0 // Invalid
            }
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().HaveCountGreaterThanOrEqualTo(3);
    }
}

/// <summary>
/// Integration tests for collection validation.
/// </summary>
public class CollectionValidationTests
{
    [Fact]
    public void CollectionModel_ValidItems_ReturnsNoErrors()
    {
        // Arrange
        var validator = new CollectionModelValidator();
        var model = new CollectionModel
        {
            Tags = ["tag1", "tag2", "tag3"],
            Children =
            [
                new() { Description = "Child 1", Value = 1 },
                new() { Description = "Child 2", Value = 2 }
            ]
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void CollectionModel_EmptyTags_ReturnsErrors()
    {
        // Arrange
        var validator = new CollectionModelValidator();
        var model = new CollectionModel
        {
            Tags = [], // Empty, should fail HasMinCount(1)
            Children = []
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().NotBeEmpty();
    }

    [Fact]
    public void CollectionModel_InvalidChildren_ReturnsErrors()
    {
        // Arrange
        var validator = new CollectionModelValidator();
        var model = new CollectionModel
        {
            Tags = ["valid"],
            Children =
            [
                new() { Description = "Valid", Value = 1 },
                new() { Description = "", Value = -1 } // Invalid
            ]
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().NotBeEmpty();
    }
}

/// <summary>
/// Integration tests for nullable string property support.
/// These tests verify that the library works with string? properties.
/// </summary>
public class NullableStringTests
{
    [Fact]
    public void NullableModel_ValidData_ReturnsNoErrors()
    {
        // Arrange
        var validator = new NullableModelValidator();
        var model = new NullableModel
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void NullableModel_NullName_ReturnsError()
    {
        // Arrange
        var validator = new NullableModelValidator();
        var model = new NullableModel
        {
            Name = null, // Null, should fail IsNotNullOrWhiteSpace
            Email = "test@example.com",
            Age = 25
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.TargetName == "Name");
    }

    [Fact]
    public void NullableModel_EmptyName_ReturnsError()
    {
        // Arrange
        var validator = new NullableModelValidator();
        var model = new NullableModel
        {
            Name = "", // Empty
            Email = "test@example.com",
            Age = 25
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().NotBeEmpty();
    }

    [Fact]
    public void NullableModel_InvalidEmail_ReturnsError()
    {
        // Arrange
        var validator = new NullableModelValidator();
        var model = new NullableModel
        {
            Name = "Valid Name",
            Email = "not-an-email",
            Age = 25
        };

        // Act
        var errors = validator.ValidateToList(model);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.TargetName == "Email");
    }
}
