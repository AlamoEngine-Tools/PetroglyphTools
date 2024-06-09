using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;


public class EmpireAtWarMegDataEntryValidatorTest
{
    private readonly EmpireAtWarMegDataEntryValidator _validator;

    public EmpireAtWarMegDataEntryValidatorTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _validator = new EmpireAtWarMegDataEntryValidator(sc.BuildServiceProvider());
    }

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.True(_validator.Validate(builderInfo).IsValid);
    }

    [Theory]
    [MemberData(nameof(NotNullDataEntryValidatorTest.InvalidTestData), MemberType = typeof(NotNullDataEntryValidatorTest))]
    [MemberData(nameof(PetroglyphMegDataEntryValidatorTest.InvalidTestData), MemberType = typeof(PetroglyphMegDataEntryValidatorTest))]
    [MemberData(nameof(InvalidTestDataEaw))]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.False(_validator.Validate(builderInfo).IsValid);
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

        // We do not allow directory operators
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".\\test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("my\\..\\test.txt"))];

        // We do not allow encrypted entries
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)];
    }
}