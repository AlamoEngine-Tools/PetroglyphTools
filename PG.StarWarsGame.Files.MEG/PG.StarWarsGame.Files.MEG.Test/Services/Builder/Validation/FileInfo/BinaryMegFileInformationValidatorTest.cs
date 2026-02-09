using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation.FileInfo;

public class BinaryMegFileInformationValidatorTest : BinaryMegFileInformationValidatorTestBase
{
    protected override uint MaxMegEntrySize => MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxEntrySize;

    protected override IMegFileInformationValidator CreateValidator()
    {
        return new BinaryMegFileInformationValidator(ServiceProvider);
    }

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegBuilderFileInformationValidationData builderInfo)
    {
        Assert.True(CreateValidator().Validate(builderInfo.FileInformation, builderInfo.DataEntries).IsValid);
    }

    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void TestInvalid(MegBuilderFileInformationValidationData builderInfo)
    {
        Assert.False(CreateValidator().Validate(builderInfo.FileInformation, builderInfo.DataEntries).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        var data = new SharedDataBuilder();
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V1),
                [data.CreateInfo("path")])
        ];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        var data = new SharedDataBuilder();
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V1),
                [data.CreateInfo("path", encrypted: true)])
        ];

        // Currently not supported. Tests will fail, as soon we do. Move to ValidTestData then.
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V2),
                [data.CreateInfo("path")])
        ];
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V3),
                [data.CreateInfo("path")])
        ];
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData()),
                [data.CreateInfo("path", encrypted: true)])
        ];
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData()),
                [data.CreateInfo("path")])
        ];
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData()),
            [
                data.CreateInfo("path", encrypted: false),
                data.CreateInfo("path", encrypted: true)
            ])
        ];
    }
}