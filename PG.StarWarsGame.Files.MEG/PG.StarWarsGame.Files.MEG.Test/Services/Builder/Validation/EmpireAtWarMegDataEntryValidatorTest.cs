using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

[TestClass]
public class EmpireAtWarMegDataEntryValidatorTest
{
    private EmpireAtWarMegDataEntryValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _validator = new EmpireAtWarMegDataEntryValidator(sc.BuildServiceProvider());
    }

    [TestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method)]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.IsTrue(_validator.Validate(builderInfo).IsValid);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(NotNullDataEntryValidatorTest))]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(PetroglyphMegDataEntryValidatorTest))]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method)]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.IsFalse(_validator.Validate(builderInfo).IsValid);
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