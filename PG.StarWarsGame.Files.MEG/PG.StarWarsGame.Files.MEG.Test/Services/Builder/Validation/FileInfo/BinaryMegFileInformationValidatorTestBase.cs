using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.StarWarsGame.Files.MEG.Test.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation.FileInfo;

public abstract class BinaryMegFileInformationValidatorTestBase : CommonMegTestBase
{
    protected abstract uint MaxMegEntrySize { get; }

    protected abstract IMegFileInformationValidator CreateValidator();

    [Fact]
    public void Validate_Null_Throws()
    {
        var validator = CreateValidator();
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null!, []));
        Assert.Throws<ArgumentNullException>(() => validator.Validate(new MegFileInformation("p", MegFileVersion.V1), null!));
    }

    [Theory]
    [MemberData(nameof(BaseValidTestData))]
    public void Validate_ValidData(MegBuilderFileInformationValidationData builderInfo)
    {
        Assert.True(CreateValidator().Validate(builderInfo.FileInformation, builderInfo.DataEntries).IsValid);
    }

    [Theory]
    [MemberData(nameof(BaseInvalidTestData))]
    public void Validate_InvalidData(MegBuilderFileInformationValidationData builderInfo)
    {
        Assert.False(CreateValidator().Validate(builderInfo.FileInformation, builderInfo.DataEntries).IsValid);
    }

    [Fact]
    public void Validate_EntryFileNotFound_ReturnsInvalid()
    {
        var validator = CreateValidator();
        var data = new SharedDataBuilder();
        
        const string filePath = "temp_file.txt";
        FileSystem.File.WriteAllText(filePath, "content");
        var fileInfo = FileSystem.FileInfo.New(filePath);
        var entryInfo = MegDataEntryBuilderInfo.FromFile(fileInfo, "PATH");

        var info = data.CreateData(new MegFileInformation("p", MegFileVersion.V1), [entryInfo]);

        // Delete the file to trigger FileNotFoundException during RefreshSize
        FileSystem.File.Delete(filePath);

        var result = validator.Validate(info.FileInformation, info.DataEntries);
        Assert.False(result.IsValid);
        Assert.Equal("One or more MEG entries reference files that could not be found.", result.FailReason);
    }

    [Fact]
    public void Validate_EntryTooLarge_ReturnsInvalid()
    {
        var validator = CreateValidator();
        var data = new SharedDataBuilder();

        var bigFile = new MegTestConstants.FakeFileInfo("large_file.bin", MaxMegEntrySize);
        var entryInfo = MegDataEntryBuilderInfo.FromFile(bigFile, "ANY");

        var info = 
            data.CreateData(new MegFileInformation("p", MegFileVersion.V1), [entryInfo]);

        // Now exceed the max size
        bigFile.Length = (long)MaxMegEntrySize + 1;

        var result = validator.Validate(info.FileInformation, info.DataEntries);
        Assert.False(result.IsValid);
        Assert.Equal("A MEG entry exceeds the maximum allowed size.", result.FailReason);
    }

    public static IEnumerable<object[]> BaseValidTestData()
    {
        var data = new SharedDataBuilder();
        yield return
        [
            data.CreateData(new MegFileInformation("p", MegFileVersion.V1), [])
        ];
        yield return
        [
            data.CreateData(new MegFileInformation("p", MegFileVersion.V1), [data.CreateInfo("p1")])
        ];
    }

    public static IEnumerable<object[]> BaseInvalidTestData()
    {
        var data = new SharedDataBuilder();

        // Encryption mismatch: Encrypted entries but no encryption data in FileInfo
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V3),
                [data.CreateInfo("path", encrypted: true)])
        ];

        // Encryption mismatch: No encrypted entries but encryption data in FileInfo
        yield return
        [
            data.CreateData(new MegFileInformation("path", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData()),
                [data.CreateInfo("path")])
        ];
    }

    [Fact]
    public void Validate_TotalSizeTooLarge_ReturnsInvalid()
    {
        var validator = new TotalSizeLimitedValidator(ServiceProvider, 100);
        var data = new SharedDataBuilder();

        var entry1 = data.CreateInfo("p1"); // Size 1
        var entry2 = data.CreateInfo("p2"); // Size 1
        
        var info = data.CreateData(new MegFileInformation("p", MegFileVersion.V1), [entry1, entry2]);
        
        // Limit is 100, 62 is fine.
        Assert.True(validator.Validate(info.FileInformation, info.DataEntries).IsValid);

        // Now use a very large file to exceed 100
        var bigFile = new MegTestConstants.FakeFileInfo("large_file.bin", 1000);
        var entry3 = MegDataEntryBuilderInfo.FromFile(bigFile, "p3");
        var info2 = data.CreateData(new MegFileInformation("p", MegFileVersion.V1), [entry3]);

        var result = validator.Validate(info2.FileInformation, info2.DataEntries);
        Assert.False(result.IsValid);
        Assert.Equal("The total size of the MEG entries exceeds the maximum allowed size.", result.FailReason);
    }

    private class TotalSizeLimitedValidator(IServiceProvider serviceProvider, uint maxFileSize)
        : BinaryMegFileInformationValidator(serviceProvider)
    {
        protected override uint MaxMegFileSize { get; } = maxFileSize;
    }

    protected class SharedDataBuilder : CommonMegTestBase
    {
        private readonly IMegFile _meg;
        private readonly MegDataEntry _entry;

        public SharedDataBuilder()
        {
            FileSystem.File.Create("file.meg").Dispose();
            _entry = MegDataEntryTest.CreateEntry("DUMMY", default, 0, 1);
            _meg = new MegFile(new MegArchive(new List<MegDataEntry> { _entry }), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        }

        public MegDataEntryBuilderInfo CreateInfo(string overridePath, bool encrypted = false)
        {
            return MegDataEntryBuilderInfo.FromEntry(_meg, _entry, overridePath, encrypted);
        }

        public MegBuilderFileInformationValidationData CreateData(
            MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries)
        {
            return new MegBuilderFileInformationValidationData(fileInformation, dataEntries);
        }
    }
}

public record MegBuilderFileInformationValidationData(
    MegFileInformation FileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> DataEntries);
