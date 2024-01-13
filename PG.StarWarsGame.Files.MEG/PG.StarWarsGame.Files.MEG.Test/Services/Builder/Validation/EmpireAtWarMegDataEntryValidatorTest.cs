using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

[TestClass]
public class EmpireAtWarMegDataEntryValidatorTest
{
    [TestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method)]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        var validator = EmpireAtWarMegDataEntryValidator.Instance;
        Assert.IsTrue(validator.Validate(builderInfo).IsValid);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(NotNullDataEntryValidatorTest))]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(PetroglyphMegDataEntryValidatorTest))]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method)]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        var validator = EmpireAtWarMegDataEntryValidator.Instance;
        Assert.IsFalse(validator.Validate(builderInfo).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".path"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path\\text.txt"))];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        // We do not allow linux path separators. 
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path/test.txt"))];

        // We do not allow non-ASCII characters
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("testNonAsciiÖ.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("testNonAscii😅.txt"))];

        // We do not allow any Windows illegal file names
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("  "))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test?.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test.txt\0"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test*.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test|"))];

        // We do not allow encrypted entries
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)];
    }
}