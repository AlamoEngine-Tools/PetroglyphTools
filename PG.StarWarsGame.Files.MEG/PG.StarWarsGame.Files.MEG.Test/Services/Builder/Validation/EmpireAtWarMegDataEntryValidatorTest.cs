using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using System.Reflection;
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

    [DataTestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetValidationDataDisplayName))]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.IsTrue(_validator.Validate(builderInfo).IsValid);
    }

    [DataTestMethod]
    [DynamicData(nameof(NotNullDataEntryValidatorTest.InvalidTestData), typeof(NotNullDataEntryValidatorTest), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetValidationDataDisplayName))]
    [DynamicData(nameof(PetroglyphMegDataEntryValidatorTest.InvalidTestData), typeof(PetroglyphMegDataEntryValidatorTest), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetValidationDataDisplayName))]
    [DynamicData(nameof(InvalidTestDataEaw), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetValidationDataDisplayName))]
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

    public static IEnumerable<object[]> InvalidTestDataEaw()
    {
        // We do not allow linux path separators. 
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path/test.txt"))];

        // We do not allow non-ASCII characters
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("testNonAsciiÖ.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("nonAsciiÖ/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("testNonAscii😅.txt"))];

        // We do not allow any Windows illegal file names
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test?.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test.txt\0"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test*.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test|"))];

        // We do not allow encrypted entries
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)];
    }

    public static string GetValidationDataDisplayName(MethodInfo methodInfo, object[] data)
    {
        var builderInfo = data[0] as MegFileDataEntryBuilderInfo;
        var filePath = builderInfo?.FilePath;

        if (filePath is not null && filePath.Length > 30)
            filePath = filePath.Substring(0, 30) + "..." + filePath.Length + "]";

        return $"{methodInfo.Name} ({filePath}, {builderInfo?.Encrypted})";
    }
}