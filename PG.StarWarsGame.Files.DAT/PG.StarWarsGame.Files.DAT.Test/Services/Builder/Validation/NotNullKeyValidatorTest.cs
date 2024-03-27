using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder.Validation;

[TestClass]
public class NotNullKeyValidatorTest
{
    private readonly NotNullKeyValidator _validator = new();

    [TestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method)]
    public void TestValid(string key)
    {
        Assert.IsTrue(_validator.Validate(key).IsValid);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method)]
    public void TestInvalid(string key)
    {
        Assert.IsFalse(_validator.Validate(key).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return ["123"];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        yield return [null];
    }
}