using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

public class EmpireAtWarMegBuilderDataEntryValidatorTest
{
    private readonly EmpireAtWarMegBuilderDataEntryValidator _validator;

    public EmpireAtWarMegBuilderDataEntryValidatorTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _validator = new EmpireAtWarMegBuilderDataEntryValidator(sc.BuildServiceProvider());
    }

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.True(_validator.Validate(builderInfo));
        if (builderInfo is not null)
            Assert.True(_validator.Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size));
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData(null)]
    public void Validate_InvalidPaths(string? path)
    {
        Assert.False(_validator.Validate(path.AsSpan(), false, null));
    }

    [Theory]
    [MemberData(nameof(NotNullDataEntryValidatorTest.InvalidTestData), MemberType = typeof(NotNullDataEntryValidatorTest))]
    [MemberData(nameof(InvalidTestDataEaw))]
    public void Validate_InvalidInfos(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.False(_validator.Validate(builderInfo));
        if (builderInfo is not null)
            Assert.False(_validator.Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size));
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH"), fileSize: 0)];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH"), fileSize: 1)];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".PATH"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH\\TEST.TXT"))];

        // While not recommended, relative names are not forbidden by the engine.
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("MY\\..\\TEST.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("..\\MY\\TEST.TXT"))];

        // We only allow a path to have double colons if the first one is the first character of the path. 
        // (This rule is imo completely broken, but that's what it is in the engine)
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(":\\MY\\TEST.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(":\\:\\TEST.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(":\\"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST:TEST"))];
        // This is a drive rooted path, but it's allowed
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:TEST.TEXT"))];

        // Also odd, but allowed
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("."))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".."))];

        // Invalid file names are also allowed (though it will fail when trying to write to file)
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST?.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.TXT\\"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("MY\\TEST.TXT\\"))];
    }

    public static IEnumerable<object[]> InvalidTestDataEaw()
    {
        yield return [null!];

        // We do not allow lower case
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("test\\TEST"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("MY\\test"))];

        // Check for trailing directory separator
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST//"))];

        // We do not allow lower case chars 
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.txt"))];

        // We do not allow linux/alternate directory separators. 
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH/TEST.TXT"))];

        // We do not allow non-ASCII characters
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TESTNONASCIIÖ.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("NONASCIIÖ/TEST.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TESTNONASCII😅.TXT"))];

        // Special treatment for some characters. This library shall not accept them.
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.TXT\0"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.TXT\n"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.TXT\r"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("TEST.TXT\t"))];

        // This would produce an empty file name (CRC: 0). We do not allow this too.
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("\\"))];


        // We do not allow to start with the current directory operator
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".\\TEST.TXT"))];

        // We only allow a path to have double colons if the first one is the first character of the path
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("FILE:\\TEST.TXT"))];

        // We do not allow absolute, rooted or URI paths
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:\\TEST.TXT"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("\\TEST.TEST"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(@"\\SERVER2\SHARE\TEST\FOO.TXT"))];

        // We do not allow encrypted entries
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("PATH"), overrideEncrypted: true)];

        // We do not allow paths with are longer than PG max allowed characters, which is 259.
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(new string('A', 260)))];

        // Because XML parsing is sometimes done on space as delimiter, we cannot use them
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("NAMEWITH SPACE"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("MY\\PATH\\WITH SPACE\\TEXT.TXT"))];
    }
}