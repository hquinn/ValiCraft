using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class ValidEnumValueTests
{
    private enum TestEnum
    {
        First = 1,
        Second = 2,
        Third = 3
    }

    private enum ByteEnum : byte
    {
        Low = 10,
        Medium = 20,
        High = 30
    }

    [Fact]
    public void IsValid_WhenValueIsValidEnumValue_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnumValue<TestEnum, int>(1);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsAnotherValidEnumValue_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnumValue<TestEnum, int>(2);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsInvalidEnumValue_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumValue<TestEnum, int>(99);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsZero_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumValue<TestEnum, int>(0);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithByteEnum_WhenValueIsValid_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnumValue<ByteEnum, byte>(10);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithByteEnum_WhenValueIsInvalid_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumValue<ByteEnum, byte>(15);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithAllValidValues_ReturnsTrue()
    {
        ValiCraft.Rules.ValidEnumValue<TestEnum, int>(1).Should().BeTrue();
        ValiCraft.Rules.ValidEnumValue<TestEnum, int>(2).Should().BeTrue();
        ValiCraft.Rules.ValidEnumValue<TestEnum, int>(3).Should().BeTrue();
    }
}
