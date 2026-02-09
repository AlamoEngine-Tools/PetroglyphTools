using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation.Entry;

public class BinaryMegDataEntryValidatorTest : BinaryMegDataEntryValidatorTestBase
{
    protected override uint MaxMegEntrySize => MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxEntrySize;

    protected override IMegDataEntryValidator CreateValidator()
    {
        return new BinaryMegDataEntryValidator();
    }
    
    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public virtual void Validate_InvalidData(MegDataEntryBuilderInfo? builderInfo, MegDataEntryValidationStatus expectedStatus)
    {
        var result = CreateValidator().Validate(builderInfo!);
        Assert.Equal(expectedStatus, result.Status);
        Assert.False(result.IsValid);
    }

    public static IEnumerable<object?[]> InvalidTestData()
    {
        var data = new SharedDataBuilder();
        yield return [data.CreateInfo(new string('A', MegFileConstants.MegMaxEntryPathLength + 1)), MegDataEntryValidationStatus.InvalidPath];
    }

    [Theory]
    [InlineData("test")]
    [InlineData("test\\TEST")]
    [InlineData("MY\\test")]
    [InlineData("    ")]
    [InlineData("NAME WITH SPACE")]
    [InlineData("NonASCII_Name_öÄ")]
    [InlineData("/with_spec1al_chars_!\"§$%&/()=")]
    public void Validate_BinaryCompliance(string path)
    {
        var data = new SharedDataBuilder();
        var info = data.CreateInfo(path);
        var result = CreateValidator().Validate(info);
        Assert.Equal(MegDataEntryValidationStatus.Valid, result.Status);
        Assert.True(result.IsValid);
    }
}
