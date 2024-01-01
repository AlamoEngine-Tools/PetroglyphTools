using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class DefaultFileInformationValidatorTest
{
    [TestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method)]
    public void TestValid(MegBuilderFileInformationValidationData builderInfo)
    {
        var validator = DefaultFileInformationValidator.Instance;
        Assert.IsTrue(validator.Validate(builderInfo).IsValid);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method)]
    public void TestInvalid(MegBuilderFileInformationValidationData builderInfo)
    {
        var validator = DefaultFileInformationValidator.Instance;
        Assert.IsFalse(validator.Validate(builderInfo).IsValid);
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
            CreateData(new MegFileInformation("path", MegFileVersion.V3, CreateRandomEncryptionData()), 
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
            CreateData(new MegFileInformation("path", MegFileVersion.V3, CreateRandomEncryptionData()),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))])
        ];
    }

    private static MegBuilderFileInformationValidationData CreateData(MegFileInformation megFileInformation, params MegFileDataEntryBuilderInfo[] entries)
    {
        return new MegBuilderFileInformationValidationData(megFileInformation, entries);
    }

    private static MegEncryptionData CreateRandomEncryptionData()
    {
        var rnd = RandomNumberGenerator.Create();
        var key = new byte[16];
        var iv = new byte[16];
        rnd.GetBytes(key);
        rnd.GetBytes(iv);
        return new MegEncryptionData(key, iv);
    }
}