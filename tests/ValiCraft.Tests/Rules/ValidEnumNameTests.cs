using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class ValidEnumNameTests
{
    private enum TestEnum
    {
        First,
        Second,
        Third
    }

    [Fact]
    public void IsValid_WhenValueIsValidEnumName_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnumName<TestEnum>("First");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsAnotherValidEnumName_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnumName<TestEnum>("Second");
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsInvalidEnumName_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumName<TestEnum>("Fourth");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsNull_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumName<TestEnum>(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsEmpty_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumName<TestEnum>("");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsWhitespace_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumName<TestEnum>("   ");
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsCaseMismatch_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnumName<TestEnum>("first");
        result.Should().BeFalse();
    }
}
