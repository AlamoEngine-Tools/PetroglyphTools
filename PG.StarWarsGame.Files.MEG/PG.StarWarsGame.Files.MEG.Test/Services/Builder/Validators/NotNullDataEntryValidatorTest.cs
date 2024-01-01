using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class NotNullDataEntryValidatorTest
{
    [TestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method)]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        var validator = NotNullDataEntryValidator.Instance;
        Assert.IsTrue(validator.Validate(builderInfo).IsValid);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method)]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        var validator = NotNullDataEntryValidator.Instance;
        Assert.IsFalse(validator.Validate(builderInfo).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        yield return [null];
    }
}