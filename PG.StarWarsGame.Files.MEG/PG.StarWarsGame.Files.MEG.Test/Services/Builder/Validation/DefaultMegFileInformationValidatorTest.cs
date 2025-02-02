using System.Collections.Generic;
using System.Reflection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

public class DefaultMegFileInformationValidatorTest
{
    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegBuilderFileInformationValidationData builderInfo)
    {
        var validator = DefaultMegFileInformationValidator.Instance;
        Assert.True(validator.Validate(builderInfo).IsValid);
    }

    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void TestInvalid(MegBuilderFileInformationValidationData builderInfo)
    {
        var validator = DefaultMegFileInformationValidator.Instance;
        Assert.False(validator.Validate(builderInfo).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V1),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))])
        ];
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V2),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))])
        ];
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V3),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))])
        ];
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData()),
            [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)])
        ];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V1),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)])
        ];
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V2),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)])
        ];
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V3),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)])
        ];
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData()),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))])
        ];
    }

    private static MegBuilderFileInformationValidationData CreateData(MegFileInformation megFileInformation, params MegFileDataEntryBuilderInfo[] entries)
    {
        return new MegBuilderFileInformationValidationData(megFileInformation, entries);
    }

    public static string GetValidationDataDisplayName(MethodInfo methodInfo, object[] data)
    {
        var info = data[0] as MegBuilderFileInformationValidationData;
        var fileInfo = info?.FileInformation;
        var filePath = fileInfo?.FilePath;

        if (filePath is not null && filePath.Length > 30)
            filePath = filePath.Substring(0, 30) + "..." + filePath.Length + "]";

        return $"{methodInfo.Name} (Path={filePath}, Version={fileInfo?.FileVersion}, Encrypted={fileInfo?.HasEncryption}, Entries={info?.DataEntries.Count})";
    }
}