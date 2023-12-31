using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class DefaultFileInformationValidatorTest : ValidatorTestSuite<MegBuilderFileInformationValidationData>
{
    protected override IValidator<MegBuilderFileInformationValidationData> CreateValidator()
    {
        return MegBuilderBase.DefaultFileInformationValidator.Instance;
    }

    protected override IEnumerable<ValidatorTestData<MegBuilderFileInformationValidationData>> GetValidCases()
    {
        yield return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(new MegFileInformation("path", MegFileVersion.V1),
                new List<MegFileDataEntryBuilderInfo>
                {
                    new(new MegDataEntryOriginInfo("path"))
                }),
            "Test_V1");

        yield return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(new MegFileInformation("path", MegFileVersion.V2),
                new List<MegFileDataEntryBuilderInfo>
                {
                    new(new MegDataEntryOriginInfo("path"))
                }),
            "Test_V2");

        yield return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(new MegFileInformation("path", MegFileVersion.V3),
                new List<MegFileDataEntryBuilderInfo>
                {
                    new(new MegDataEntryOriginInfo("path"))
                }),
            "Test_V3");
        
        yield return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(new MegFileInformation("path", MegFileVersion.V3, CreateRandomEncryptionData()),
                new List<MegFileDataEntryBuilderInfo>
                {
                    new(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)
                }),
            "Test_V3_WithEncryptionData_AndEncryptedEntry");
    }

    protected override IEnumerable<ValidatorTestData<MegBuilderFileInformationValidationData>> GetInvalidCases()
    {
        yield return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(new MegFileInformation("path", MegFileVersion.V1),
                new List<MegFileDataEntryBuilderInfo>
                {
                    new(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)
                }),
            "Test_V1_WithEncryptionData");

        yield return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(new MegFileInformation("path", MegFileVersion.V2),
                new List<MegFileDataEntryBuilderInfo>
                {
                    new(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)
                }),
            "Test_V2_WithEncryptionData");

        yield return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(new MegFileInformation("path", MegFileVersion.V3),
                new List<MegFileDataEntryBuilderInfo>
                {
                    new(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)
                }),
            "Test_V3_NoEncryptionData_WithEncryptedEntry");


        //yield return CreateTestData(
        //    "Test_V3_WithEncryptionData_WithoutEncryptedEntry",
        //    new MegFileInformation("path", MegFileVersion.V3, CreateRandomEncryptionData()),
        //    new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path")));
    }

    private static ValidatorTestData<MegBuilderFileInformationValidationData> CreateTestData(
        string testName, 
        MegFileInformation fileInfo,
        params MegFileDataEntryBuilderInfo[] entries)
    {
        return new ValidatorTestData<MegBuilderFileInformationValidationData>(
            new MegBuilderFileInformationValidationData(fileInfo, entries.ToList()), testName);
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