using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation.Entry;

public abstract class BinaryMegDataEntryValidatorTestBase : CommonMegTestBase
{
    protected abstract uint MaxMegEntrySize { get; }
    
    protected abstract IMegDataEntryValidator CreateValidator();
    
    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void Validate_ValidData(MegDataEntryBuilderInfo builderInfo)
    {
        var result = CreateValidator().Validate(builderInfo);
        Assert.Equal(MegDataEntryValidationStatus.Valid, result.Status);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreateValidator().Validate(null!));
    }

    [Fact]
    public void Validate_OriginNotFound_EntryReference()
    {
        FileSystem.File.Create("file.meg");
        var entry = MegDataEntryTest.CreateEntry("DUMMY", default, 0, 1);
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var location = new MegDataEntryLocationReference(meg, entry);
        var origin = new MegDataEntryOriginInfo(location);
        
        var info = new MegDataEntryBuilderInfo(origin, "PATH");
        
        var result = CreateValidator().Validate(info);
        Assert.Equal(MegDataEntryValidationStatus.InvalidOriginNotFound, result.Status);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_OriginNotFound_FileNotFoundException()
    {
        const string file = "test_to_delete.txt";
        FileSystem.File.WriteAllText(file, "content");
        var info = MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New(file), "PATH");
        
        FileSystem.File.Delete(file);
        
        var result = CreateValidator().Validate(info);
        Assert.Equal(MegDataEntryValidationStatus.InvalidOriginNotFound, result.Status);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidSize_UsingFakeFileInfo()
    {
        var maxSize = MaxMegEntrySize;
        var bigFile = new MegTestConstants.FakeFileInfo("large_file.bin", maxSize);
        var info = MegDataEntryBuilderInfo.FromFile(bigFile, "ANY");
        // Now exceed the max size before validation
        bigFile.Length = (long)maxSize + 1;
        var result = CreateValidator().Validate(info);
        Assert.Equal(MegDataEntryValidationStatus.InvalidEntryTooLarge, result.Status);
        Assert.False(result.IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        var data = new SharedDataBuilder();
        foreach (var path in data.ValidPaths())
            yield return [data.CreateInfo(path)];
    }

    protected class SharedDataBuilder : CommonMegTestBase
    {
        private readonly IMegFile _meg;
        private readonly MegDataEntry _entry;

        public SharedDataBuilder()
        {
            FileSystem.File.Create("file.meg");
            _entry = MegDataEntryTest.CreateEntry("DUMMY", default, 0, 1);
            _meg = new MegFile(new MegArchive([_entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        }

        public MegDataEntryBuilderInfo CreateInfo(string overridePath, bool encrypted = false)
        {
            return MegDataEntryBuilderInfo.FromEntry(_meg, _entry, overridePath, encrypted);
        }

        public virtual IEnumerable<string> ValidPaths()
        {
            yield return "PATH";
            yield return "PATH\\TEST.TXT";
            yield return ".PATH";
        }
    }
}
