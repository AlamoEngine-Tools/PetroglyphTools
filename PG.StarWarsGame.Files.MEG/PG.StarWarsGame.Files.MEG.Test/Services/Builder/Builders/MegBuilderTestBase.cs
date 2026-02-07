using System;
using System.Collections.Generic;
using System.Linq;
using AnakinRaW.CommonUtilities.Extensions;
using AnakinRaW.CommonUtilities.Testing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.StarWarsGame.Files.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class MegBuilderTestBase<TBuilder> : FileBuilderTestBase<TBuilder, IReadOnlyCollection<MegDataEntryBuilderInfo>, MegFileInformation>
    where TBuilder : MegBuilderBase
{
    protected virtual bool CanProduceInvalidEntryPaths => false;

    protected abstract Type ExpectedFileInfoValidatorType { get; }
    protected abstract Type ExpectedDataEntryValidatorType { get; }
    protected abstract Type? ExpectedDataEntryPathNormalizerType { get; }
    protected abstract bool? ExpectedOverwritesDuplicates { get; }
    protected abstract bool? ExpectedAutomaticallyAddFileSizes { get; }

    protected override string DefaultFileName => "test.meg";

    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportMEG();
    }

    protected sealed override MegFileInformation CreateFileInfo(bool valid, string path)
    {
        if (!FileInfoIsAlwaysValid && !valid)
            return CreateInvalidFileInfo(path);
        return new MegFileInformation(path, MegFileVersion.V1);
    }

    protected virtual MegFileInformation CreateInvalidFileInfo(string path)
    {
        throw new NotSupportedException();
    }

    protected virtual string GetFailingEntryPath()
    {
        throw new NotSupportedException();
    }

    [Fact]
    public void MegBuilderTestSuite_Ctor()
    {
        var builder = CreateBuilder();

        Assert.Equal(ExpectedFileInfoValidatorType, builder.MegFileInformationValidator.GetType());
        Assert.Equal(ExpectedDataEntryValidatorType, builder.DataEntryValidator.GetType());
        Assert.Equal(ExpectedDataEntryPathNormalizerType, builder.DataEntryPathNormalizer?.GetType());
        Assert.Equal(ExpectedOverwritesDuplicates, builder.OverwritesDuplicateEntries);
        Assert.Equal(ExpectedAutomaticallyAddFileSizes, builder.AutomaticallyAddFileSizes);
        Assert.Equal(uint.MaxValue, builder.MaxEntrySize);
    }

    [Fact]
    public void AddFile_FileDoesNotExists_EntryNotAdded()
    {
        var builder = CreateBuilder();

        var result = builder.AddFile("file.txt", "path/file.txt");

        Assert.False(result.Added);
        Assert.Equal(MegDataEntryAddStatus.FileOrEntryNotFound, result.Status);
    }

    [Fact]
    public void GetDataEntries_IsReadOnly()
    {
        var builder = CreateBuilder();

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        FileSystem.Initialize().WithFile("file.txt");
        builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);
        Assert.Empty(entries);

        if (entries is List<MegDataEntryBuilderInfo> builderList)
        {
            builderList.Add(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo("path1")));
            builderList.Add(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo("path2")));
            builderList.Add(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo("path3")));
            Assert.Single(builder.DataEntries);
        }
    }

    [Fact]
    public void Clear()
    {
        var builder = CreateBuilder();

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        FileSystem.Initialize().WithFile("file.txt");
        builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);

        builder.Clear();

        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void Remove()
    {
        var builder = CreateBuilder();

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        FileSystem.Initialize().WithFile("file.txt");
        var result = builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);
        Assert.False(builder.Remove(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo("notFound.txt"))));
        Assert.Single(builder.DataEntries);
        Assert.True(builder.Remove(result.AddedBuilderInfo!));
        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void Dispose_ThrowsOnAddingOrBuildingMethods()
    {
        var builder = CreateBuilder();

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        FileSystem.Initialize().WithFile("file.txt");
        builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);

        builder.Dispose();

        Assert.Empty(builder.DataEntries);

        Assert.Throws<ObjectDisposedException>(() => builder.AddFile("file.txt", "file.txt"));
        Assert.Throws<ObjectDisposedException>(() =>
            builder.AddEntry(new MegDataEntryLocationReference(CreateEmptyTestMeg(), MegDataEntryTest.CreateEntry("file.txt"))));
        Assert.Throws<ObjectDisposedException>(() =>
            builder.Build(new MegFileInformation("a.meg", MegFileVersion.V1), false));

        Assert.DoesNotThrow(() => builder.DataEntries);
        Assert.DoesNotThrow(builder.Clear);
        Assert.DoesNotThrow(() => builder.Remove(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo("notFound.txt"))));

        Assert.DoesNotThrow(builder.Dispose);
    }

    #region AddFile
    
    [Fact]
    public void AddFile_Throws()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        FileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.AddFile(fileToAdd, null!));
        Assert.Throws<ArgumentNullException>(() => builder.AddFile(null!, inputEntryPath));
        Assert.Throws<ArgumentException>(() => builder.AddFile("", inputEntryPath));
        Assert.Throws<ArgumentException>(() => builder.AddFile(fileToAdd, ""));
    }

    [Fact]
    public void AddFile_WithFileSize()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        FileSystem.Initialize().WithFile(fileToAdd).Which(x => x.HasBytesContent([1, 2, 3]));

        var builder = CreateBuilder();

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.True(result.Added, $"Actual Result: {result.Status}");

        var expectedEntryPath = builder.DataEntryPathNormalizer?.Normalize(inputEntryPath) ?? inputEntryPath;

        Assert.Equal(expectedEntryPath, result.AddedBuilderInfo.EntryPath);
        Assert.Null(result.OverwrittenBuilderInfo);

        Assert.Single(builder.DataEntries);

        var entry = builder.DataEntries.First();
        Assert.Equal(expectedEntryPath, entry.EntryPath);
        Assert.False(entry.Encrypted);
        if (ExpectedAutomaticallyAddFileSizes == true)
            Assert.Equal(3u, entry.Size);
        else
            Assert.Null(entry.Size);

        Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), entry.OriginInfo.FileInfo);
    }

    [Fact]
    public void AddFile_AssureEncoding_WithNormalization()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/fileWithNonAsciiÖ.txt";

        FileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder();

        var expectedEntryPath =
            MegFileConstants.MegDataEntryPathEncoding.EncodeString(
                builder.DataEntryPathNormalizer?.Normalize(inputEntryPath) ?? inputEntryPath);

        var result = builder.AddFile(fileToAdd, inputEntryPath);
        Assert.True(result.Added);
        Assert.Equal(expectedEntryPath, result.AddedBuilderInfo.EntryPath);
    }

    [Fact]
    public void AddFile_LongStringHandling()
    {
        const string fileToAdd = "file.txt";

        var firstPart = new string('a', 150);
        var secondPart = new string('b', 150);

        FileSystem.Initialize().WithFile(fileToAdd);

        // Make sure the normalizer triggers
        var builder = CreateBuilder();
        
        var entryPath = firstPart + secondPart;
        var result = builder.AddFile(fileToAdd, firstPart + secondPart);

        if (!builder.DataEntryValidator.Validate(entryPath.AsSpan(), false, null))
        {
            Assert.False(result.Added);
            return;
        }

        var expectedEntryPath =
            MegFileConstants.MegDataEntryPathEncoding.EncodeString(
                builder.DataEntryPathNormalizer?.Normalize(entryPath) ?? entryPath);
        Assert.True(result.Added);
        Assert.Equal(expectedEntryPath, result.AddedBuilderInfo.EntryPath);
    }

    [Fact]
    public void AddFile_Override()
    {
        const string fileToAdd = "file1.txt";
        const string otherFileToAdd = "file2.txt";
        const string inputEntryPath = "fileWithNonAsciiÖ";
        const string otherEntryPath = "fileWithNonAsciiÄ";

        FileSystem.Initialize().WithFile(fileToAdd);
        FileSystem.Initialize().WithFile(otherFileToAdd);

        var builder = CreateBuilder();

        builder.AddFile(fileToAdd, inputEntryPath);

        var resultSecondAdd = builder.AddFile(otherFileToAdd, otherEntryPath);

        if (ExpectedOverwritesDuplicates == false)
        {
            Assert.False(resultSecondAdd.Added);
            Assert.Equal(MegDataEntryAddStatus.DuplicateEntry, resultSecondAdd.Status);
            Assert.Single(builder.DataEntries);
            Assert.Null(resultSecondAdd.OverwrittenBuilderInfo);
            Assert.False(resultSecondAdd.WasOverwrite);
            Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FileInfo);
        }
        else
        {
            Assert.True(resultSecondAdd.Added);
            Assert.Single(builder.DataEntries);
            Assert.NotNull(resultSecondAdd.AddedBuilderInfo);
            Assert.NotNull(resultSecondAdd.OverwrittenBuilderInfo);
            Assert.True(resultSecondAdd.WasOverwrite);
            Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), resultSecondAdd.OverwrittenBuilderInfo.OriginInfo.FileInfo);
            Assert.Equal(FileSystem.Path.GetFullPath(otherFileToAdd), resultSecondAdd.AddedBuilderInfo.OriginInfo.FileInfo);

            //Assert that duplicate check was based on encoded(thus also normalized) file path, cause the original inputs have different values.

            var expectedEncodedEntry = MegFileConstants.MegDataEntryPathEncoding.EncodeString(MegFileConstants.MegDataEntryPathEncoding.EncodeString(
                builder.DataEntryPathNormalizer?.Normalize(inputEntryPath) ?? inputEntryPath));
            Assert.Equal(expectedEncodedEntry, resultSecondAdd.AddedBuilderInfo.EntryPath);
        }
    }

    [Fact]
    public void AddFile_ValidatorFails()
    {
        if (!CanProduceInvalidEntryPaths)
            return;

        const string fileToAdd = "file.txt";

        FileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder();

        var result = builder.AddFile(fileToAdd, GetFailingEntryPath());

        Assert.Equal(MegDataEntryAddStatus.InvalidEntry, result.Status);
        Assert.Empty(builder.DataEntries);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AddFile_AddFileSize_FileTooLarge_Throws(bool addFileSize)
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "file.txt";

        // One byte too large.
        FileSystem.Initialize().WithFile(fileToAdd).Which(x => x.HasBytesContent([1, 2, 3, 4]));

        var builder = new MaxFileSizeMegBuilder(3, addFileSize, ServiceProvider);

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        if (addFileSize)
        {
            Assert.Equal(MegDataEntryAddStatus.EntryFileTooLarge, result.Status);
            Assert.Empty(builder.DataEntries);
        }
        else
        {
            Assert.Equal(MegDataEntryAddStatus.Added, result.Status);
            Assert.Single(builder.DataEntries);
        }
    }

    #endregion

    #region AddEntry

    [Fact]
    public void AddEntry_Throws()
    {
        var builder = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.AddEntry(null!, "path"));
        Assert.Throws<ArgumentException>(() => builder.AddEntry(new MegDataEntryLocationReference(CreateEmptyTestMeg(), MegDataEntryTest.CreateEntry("path")), ""));
    }

    [Fact]
    public void AddEntry_EntryNotFound()
    {
        var builder = CreateBuilder();

        var entry = MegDataEntryTest.CreateEntry("file.txt");

        var meg = CreateEmptyTestMeg();

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg, entry));

        Assert.Equal(MegDataEntryAddStatus.FileOrEntryNotFound, result.Status);
        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void AddEntry()
    {
        var builder = CreateBuilder();

        var meg = CreateTestMeg();
        var entry = meg.Archive[0];

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg, entry));

        Assert.True(result.Added);
        Assert.Single(builder.DataEntries);

        var actualEntry = builder.DataEntries.First();

        var expectedEntryPath = MegFileConstants.MegDataEntryPathEncoding.EncodeString(MegFileConstants.MegDataEntryPathEncoding.EncodeString(
            builder.DataEntryPathNormalizer?.Normalize(entry.Path) ?? entry.Path));

        Assert.Equal(expectedEntryPath, actualEntry.EntryPath);
        Assert.Same(entry, actualEntry.OriginInfo.MegFileLocation!.DataEntry);
    }

    [Fact]
    public void AddEntry_OverrideProperties()
    {
        var builder = CreateBuilder();

        var meg = CreateTestMeg();
        var entry = meg.Archive[0];

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg, entry), "new.txt");

        Assert.True(result.Added);
        Assert.Single(builder.DataEntries);

        var expectedEntryPath = builder.DataEntryPathNormalizer?.Normalize("new.txt") ?? "new.txt";

        var actualEntry = builder.DataEntries.First();
        Assert.Equal(expectedEntryPath, actualEntry.EntryPath);
        Assert.Same(entry, actualEntry.OriginInfo.MegFileLocation!.DataEntry);
    }

    [Fact]
    public void AddEntry_Override()
    {
        const string fileToAdd = "file.txt";
        FileSystem.Initialize().WithFile(fileToAdd).Which(x => x.HasBytesContent([1, 2, 3]));

        var meg = CreateTestMeg();
        var entry = meg.Archive[0];

        var builder = CreateBuilder();

        // Use AddFile here to assert that AddFile and AddEntry work when combined.
        var addedFile = builder.AddFile(fileToAdd, fileToAdd);
        Assert.True(addedFile.Added);

        var resultSecondAdd = builder.AddEntry(new MegDataEntryLocationReference(meg, entry), addedFile.AddedBuilderInfo!.EntryPath);

        if (ExpectedOverwritesDuplicates == false)
        {
            Assert.Equal(MegDataEntryAddStatus.DuplicateEntry, resultSecondAdd.Status);
            Assert.Single(builder.DataEntries);
            Assert.Null(resultSecondAdd.OverwrittenBuilderInfo);
            Assert.False(resultSecondAdd.WasOverwrite);
            Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FileInfo);
        }
        else
        {
            Assert.Equal(MegDataEntryAddStatus.Added, resultSecondAdd.Status);
            Assert.True(resultSecondAdd.WasOverwrite);
            Assert.Single(builder.DataEntries);
            Assert.Same(addedFile.AddedBuilderInfo, resultSecondAdd.OverwrittenBuilderInfo);
            Assert.True(builder.DataEntries.First().OriginInfo.IsEntryReference);
            Assert.Same(meg, builder.DataEntries.First().OriginInfo.MegFileLocation!.MegFile);
        }
    }

    [Fact]
    public void AddEntry_ValidatorFails()
    {
        if (!CanProduceInvalidEntryPaths)
            return;

        var meg = CreateTestMeg();
        var entry = meg.Archive[0];

        var builder = CreateBuilder();

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg, entry), GetFailingEntryPath());

        Assert.Equal(MegDataEntryAddStatus.InvalidEntry, result.Status);
        Assert.Empty(builder.DataEntries);
    }

    #endregion

    private IMegFile CreateEmptyTestMeg()
    {
        using var _ = FileSystem.File.Create("a.meg");
        return new MegFile(new MegArchive([]), CreateFileInfo(true, "a.meg"), ServiceProvider);
    }

    private IMegFile CreateTestMeg()
    {
        FileSystem.File.WriteAllBytes("test.meg", MegTestConstants.ContentMegFileV1);
        return ServiceProvider.GetRequiredService<IMegFileService>().Load("test.meg");
    }

    private class MaxFileSizeMegBuilder(uint maxFileSize, bool addFileSize, IServiceProvider services) : MegBuilderBase(services)
    {
        internal override uint MaxEntrySize => maxFileSize;

        public override bool AutomaticallyAddFileSizes => addFileSize;
    }
}