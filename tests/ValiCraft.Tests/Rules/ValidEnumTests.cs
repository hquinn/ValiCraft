using AwesomeAssertions;

namespace ValiCraft.Tests.Rules;

public class ValidEnumTests
{
    private enum TestEnum
    {
        Yes,
        No
    }

    private enum TestEnumWithValues
    {
        First = 1,
        Second = 2,
        Third = 3
    }

    [Flags]
    private enum TestFlagsEnum
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }

    [Fact]
    public void IsValid_WhenValueIsDefinedEnumMember_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnum(TestEnum.Yes);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsAnotherDefinedEnumMember_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnum(TestEnum.No);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsUndefinedEnumMember_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnum((TestEnum)2);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsNegativeUndefinedEnumMember_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnum((TestEnum)(-1));
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsDefinedEnumWithExplicitValues_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnum(TestEnumWithValues.First);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenValueIsUndefinedEnumWithExplicitValues_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnum((TestEnumWithValues)99);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenValueIsZeroAndNotDefined_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnum((TestEnumWithValues)0);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenAllDefinedMembers_ReturnsTrue()
    {
        ValiCraft.Rules.ValidEnum(TestEnumWithValues.First).Should().BeTrue();
        ValiCraft.Rules.ValidEnum(TestEnumWithValues.Second).Should().BeTrue();
        ValiCraft.Rules.ValidEnum(TestEnumWithValues.Third).Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenFlagsEnumHasDefinedValue_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnum(TestFlagsEnum.Read);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenFlagsEnumHasCombinedUndefinedValue_ReturnsFalse()
    {
        var result = ValiCraft.Rules.ValidEnum((TestFlagsEnum)3);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenDefaultEnumValue_ReturnsTrue()
    {
        var result = ValiCraft.Rules.ValidEnum(default(TestEnum));
        result.Should().BeTrue();
    }
}
