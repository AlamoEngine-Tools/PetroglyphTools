using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities.Validation;

namespace PG.Commons.Test.Utilities.Validation;

[TestClass]
public class NullableAbstractValidatorTest
{

    [TestMethod]
    public void Test_SupportNull_ReferenceType()
    {
        var validator = new NullableValidator<object?>(true);
        Assert.IsTrue(validator.Validate((object?)null).IsValid);

        Assert.IsTrue(validator.Validate(new object()).IsValid);
    }

    [TestMethod]
    public void Test_DoesNotSupportNull_ReferenceType()
    {
        var validator = new NullableValidator<object?>(false);
        Assert.IsFalse(validator.Validate((object?)null).IsValid);

        Assert.IsTrue(validator.Validate(new object()).IsValid);
    }

    [TestMethod]
    public void Test_SupportNull_ValueType()
    {
        var validator = new NullableValidator<int?>(true);
        Assert.IsTrue(validator.Validate((int?)null).IsValid);

        Assert.IsTrue(validator.Validate(1).IsValid);
    }

    [TestMethod]
    public void Test_DoesNotSupportNull_ValueType()
    {
        var validator = new NullableValidator<int?>(false);
        Assert.IsFalse(validator.Validate((int?)null).IsValid);

        Assert.IsTrue(validator.Validate(1).IsValid);
    }


    private class NullableValidator<T>(bool supportNull) : NullableAbstractValidator<T?>
    {
        protected override bool IsValueNullable { get; } = supportNull;
    }
}