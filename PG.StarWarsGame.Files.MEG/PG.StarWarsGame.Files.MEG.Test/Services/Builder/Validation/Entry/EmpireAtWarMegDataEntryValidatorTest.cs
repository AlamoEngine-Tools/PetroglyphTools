using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation.Entry;

public class EmpireAtWarMegDataEntryValidatorTest : BinaryMegDataEntryValidatorTestBase
{
    protected override uint MaxMegEntrySize => MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.EawFoc).MaxEntrySize;

    protected override IMegDataEntryValidator CreateValidator()
    {
        return new EmpireAtWarMegDataEntryValidator();
    }

    [Theory]
    [MemberData(nameof(EawValidTestData))]
    public void Validate_EawSpecific(MegDataEntryBuilderInfo builderInfo)
    {
        var result = CreateValidator().Validate(builderInfo);
        Assert.Equal(MegDataEntryValidationStatus.Valid, result.Status);
        Assert.True(result.IsValid);
    }

    [Theory]
    [MemberData(nameof(InvalidTestDataEaw))]
    public void Validate_InvalidInfos_EawSpecific(MegDataEntryBuilderInfo? builderInfo, MegDataEntryValidationStatus expectedStatus)
    {
        var result = CreateValidator().Validate(builderInfo!);
        Assert.Equal(expectedStatus, result.Status);
        Assert.False(result.IsValid);
    }

    public static IEnumerable<object[]> EawValidTestData()
    {
        var data = new SharedDataBuilder();
        yield return [data.CreateInfo("PATH")];
        yield return [data.CreateInfo("PATH", encrypted: false)];
        yield return [data.CreateInfo(".PATH")];
        yield return [data.CreateInfo("PATH\\TEST.TXT")];

        // While not recommended, relative names are not forbidden by the engine.
        yield return [data.CreateInfo("MY\\..\\TEST.TXT")];
        yield return [data.CreateInfo("..\\MY\\TEST.TXT")];

        // We only allow a path to have double colons if the first one is the first character of the path. 
        yield return [data.CreateInfo(":\\MY\\TEST.TXT")];
        yield return [data.CreateInfo(":\\:\\TEST.TXT")];
        yield return [data.CreateInfo(":\\")];
        yield return [data.CreateInfo("TEST:TEST")];
        // This is a drive rooted path, but it's allowed
        yield return [data.CreateInfo("C:TEST.TEXT")];

        // Also odd, but allowed
        yield return [data.CreateInfo(".")];
        yield return [data.CreateInfo("..")];

        // Invalid file names are also allowed (though it will fail when trying to write to file)
        yield return [data.CreateInfo("TEST?.TXT")];
        yield return [data.CreateInfo("TEST.TXT\\")];
        yield return [data.CreateInfo("MY\\TEST.TXT\\")];
    }

    public static IEnumerable<object[]> InvalidTestDataEaw()
    {
        var data = new SharedDataBuilder();

        // Whitespace path is not allowed
        yield return [data.CreateInfo("    "), MegDataEntryValidationStatus.InvalidPath];

        // We do not allow lower case
        yield return [data.CreateInfo("test"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("test\\TEST"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("MY\\test"), MegDataEntryValidationStatus.InvalidPath];

        // Check for trailing directory separator
        yield return [data.CreateInfo("TEST//"), MegDataEntryValidationStatus.InvalidPath];

        // We do not allow lower case chars 
        yield return [data.CreateInfo("TEST.txt"), MegDataEntryValidationStatus.InvalidPath];

        // We do not allow linux/alternate directory separators. 
        yield return [data.CreateInfo("PATH/TEST.TXT"), MegDataEntryValidationStatus.InvalidPath];

        // We do not allow non-ASCII characters
        yield return [data.CreateInfo("TESTNONASCIIÖ.TXT"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("NONASCIIÖ/TEST.TXT"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("TESTNONASCII😅.TXT"), MegDataEntryValidationStatus.InvalidPath];
        
        // Special treatment for some characters. This library shall not accept them.
        yield return [data.CreateInfo("TEST.TXT\0"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("TEST.TXT\n"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("TEST.TXT\r"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("TEST.TXT\t"), MegDataEntryValidationStatus.InvalidPath];

        // This would produce an empty file name (CRC: 0). We do not allow this too.
        yield return [data.CreateInfo("\\"), MegDataEntryValidationStatus.InvalidPath];

        // We do not allow to start with the current directory operator
        yield return [data.CreateInfo(".\\TEST.TXT"), MegDataEntryValidationStatus.InvalidPath];

        // We only allow a path to have double colons if the first one is the first character of the path
        yield return [data.CreateInfo("FILE:\\TEST.TXT"), MegDataEntryValidationStatus.InvalidPath];

        // We do not allow absolute, rooted or URI paths
        yield return [data.CreateInfo("C:\\TEST.TXT"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("\\TEST.TEST"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo(@"\\\\SERVER2\\SHARE\\TEST\\FOO.TXT"), MegDataEntryValidationStatus.InvalidPath];

        // We do not allow encrypted entries
        yield return [data.CreateInfo("PATH", encrypted: true), MegDataEntryValidationStatus.Invalid];

        // We do not allow paths with are longer than PG max allowed characters, which is 259.
        yield return [data.CreateInfo(new string('A', 260)), MegDataEntryValidationStatus.InvalidPath];

        // Because XML parsing is sometimes done on space as delimiter, we cannot use them
        yield return [data.CreateInfo("NAMEWITH SPACE"), MegDataEntryValidationStatus.InvalidPath];
        yield return [data.CreateInfo("MY\\PATH\\WITH SPACE\\TEXT.TXT"), MegDataEntryValidationStatus.InvalidPath];
    }
}