using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

[TestClass]
public class PetroglyphMegDataEntryValidatorTest
{
    [TestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method)]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        var validator = new TestPetroglyphMegDataEntryValidator();
        Assert.IsTrue(validator.Validate(builderInfo).IsValid);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(NotNullDataEntryValidatorTest))]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method)]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        var validator = new TestPetroglyphMegDataEntryValidator();
        Assert.IsFalse(validator.Validate(builderInfo).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".path"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path/text.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path\\text.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        // We do not allow directory names
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("dir/"))];

        // We do not allow directory operators
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("./test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("../test.txt"))];

        // We do not allow absolute, rooted or URI paths
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("file://test.txt"))];

        // We do not allow paths with are longer than 256 characters, as that's the default Windows limit.
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(new string('a', 256)))];
    }

    private class TestPetroglyphMegDataEntryValidator : PetroglyphMegDataEntryValidator;
}