using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

public class PetroglyphMegFileInformationValidatorTest
{
    private readonly TestPetroglyphMegFileInformationValidator _validator;

    public PetroglyphMegFileInformationValidatorTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _validator = new TestPetroglyphMegFileInformationValidator(sc.BuildServiceProvider());
    }


    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegBuilderFileInformationValidationData builderInfo)
    {
        Assert.True(_validator.Validate(builderInfo).IsValid);
    }

    [Theory]
    [MemberData(nameof(InvalidTestData), MemberType = typeof(DefaultMegFileInformationValidatorTest))]
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
            CreateData(new MegFileInformation(new string('a', 261), MegFileVersion.V1),
                [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))])
        ];
    }

    private static MegBuilderFileInformationValidationData CreateData(MegFileInformation megFileInformation, params MegFileDataEntryBuilderInfo[] entries)
    {
        return new MegBuilderFileInformationValidationData(megFileInformation, entries);
    }

    class TestPetroglyphMegFileInformationValidator(IServiceProvider serviceProvider)
        : PetroglyphMegFileInformationValidator(serviceProvider);
}