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
        Assert.True(_validator.Validate(builderInfo));
    }

    [Theory]
    [MemberData(nameof(NotNullDataEntryValidatorTest.InvalidTestData), MemberType = typeof(NotNullDataEntryValidatorTest))]
    [MemberData(nameof(PetroglyphMegDataEntryValidatorTest.InvalidTestData), MemberType = typeof(PetroglyphMegDataEntryValidatorTest))]
    [MemberData(nameof(InvalidTestDataEaw))]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.False(_validator.Validate(builderInfo));
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".PATH"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH\\TEST.TXT"))];
    }

    public static IEnumerable<object[]> InvalidTestDataEaw()
    {
        // We do not allow lower case chars 
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.txt"))];

        // We do not allow linux path separators. 
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH/TEST.TXT"))];

        // We do not allow non-ASCII characters
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TESTNONASCIIÖ.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("NONASCIIÖ/TEST.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TESTNONASCII😅.TXT"))];

        // We do not allow any Windows illegal file names
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST?.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.TXT\0"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST*.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST|"))];

        // We do not allow directory operators
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".\\TEST.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("MY\\..\\TEST.TXT"))];

        // We do not allow encrypted entries
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH"), overrideEncrypted: true)];
    }
}