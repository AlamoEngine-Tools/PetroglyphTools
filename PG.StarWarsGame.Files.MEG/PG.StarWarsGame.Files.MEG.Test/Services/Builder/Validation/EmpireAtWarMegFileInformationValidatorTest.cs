// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;


public class EmpireAtWarMegFileInformationValidatorTest
{
    private readonly EmpireAtWarMegFileInformationValidator _validator;

    public EmpireAtWarMegFileInformationValidatorTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _validator = new EmpireAtWarMegFileInformationValidator(sc.BuildServiceProvider());
    }


    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegBuilderFileInformationValidationData builderInfo)
    {
        Assert.True(_validator.Validate(builderInfo).IsValid);
    }

    [Theory]
    [MemberData(nameof(InvalidTestData), MemberType = typeof(DefaultMegFileInformationValidatorTest))]
    [MemberData(nameof(InvalidTestData), MemberType = typeof(PetroglyphMegFileInformationValidatorTest))]
    [MemberData(nameof(InvalidTestData))]
    public void TestInvalid(MegBuilderFileInformationValidationData builderInfo)
    {
        Assert.False(_validator.Validate(builderInfo).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return
        [
            CreateData(new MegFileInformation("path", MegFileVersion.V1),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))])
        ];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        yield return
        [
            CreateData(new MegFileInformation("pathÄ", MegFileVersion.V1),
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