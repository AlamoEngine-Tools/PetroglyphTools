using PG.Commons.Utilities.Validation;
using Xunit;

namespace PG.Commons.Test.Utilities.Validation;

public class NullableAbstractValidatorTest
{

    [Fact]
    public void Test_SupportNull_ReferenceType()
    {
        var validator = new NullableValidator<object?>(true);
        Assert.True(validator.Validate((object?)null).IsValid);

        Assert.True(validator.Validate(new object()).IsValid);
    }

    [Fact]
    public void Test_DoesNotSupportNull_ReferenceType()
    {
        var validator = new NullableValidator<object?>(false);
        Assert.False(validator.Validate((object?)null).IsValid);

        Assert.True(validator.Validate(new object()).IsValid);
    }

    [Fact]
    public void Test_SupportNull_ValueType()
    {
        var validator = new NullableValidator<int?>(true);
        Assert.True(validator.Validate((int?)null).IsValid);

        Assert.True(validator.Validate(1).IsValid);
    }

    [Fact]
    public void Test_DoesNotSupportNull_ValueType()
    {
        var validator = new NullableValidator<int?>(false);
        Assert.False(validator.Validate((int?)null).IsValid);

        Assert.True(validator.Validate(1).IsValid);
    }


    private class NullableValidator<T>(bool supportNull) : NullableAbstractValidator<T?>
    {
        protected override bool IsValueNullable { get; } = supportNull;
    }
}