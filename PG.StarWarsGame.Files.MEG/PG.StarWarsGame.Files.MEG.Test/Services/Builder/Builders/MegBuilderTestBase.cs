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
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
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
        Assert.True(builder.MaxMegFileSize > 0);
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
        Assert.Single(entries);

        if (entries is List<MegDataEntryBuilderInfo> builderList)
        {
            builderList.Add(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("path1"))));
            builderList.Add(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("path2"))));
            builderList.Add(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("path3"))));
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
        FileSystem.Initialize().WithFile("notFound.txt");
        Assert.False(builder.Remove(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("notFound.txt")))));
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

        Assert.DoesNotThrow(() => { _ = builder.DataEntries; });
        Assert.DoesNotThrow(builder.Clear);
        Assert.DoesNotThrow(() => builder.Remove(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("file.txt")))));

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
    public void AddFile()
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
        Assert.Equal(3u, entry.Size);

        Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), entry.OriginInfo.FileInfo!.FullName);
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

        var validationResult = builder.DataEntryValidator.Validate(new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(FileSystem.FileInfo.New(fileToAdd)), entryPath));
        if (!validationResult.IsValid)
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
            Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FileInfo!.FullName);
        }
        else
        {
            Assert.True(resultSecondAdd.Added);
            Assert.Single(builder.DataEntries);
            Assert.NotNull(resultSecondAdd.AddedBuilderInfo);
            Assert.NotNull(resultSecondAdd.OverwrittenBuilderInfo);
            Assert.True(resultSecondAdd.WasOverwrite);
            Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), resultSecondAdd.OverwrittenBuilderInfo.OriginInfo.FileInfo!.FullName);
            Assert.Equal(FileSystem.Path.GetFullPath(otherFileToAdd), resultSecondAdd.AddedBuilderInfo.OriginInfo.FileInfo!.FullName);

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

    [Fact]
    public void AddFile_AddFileSize_FileTooLarge_Throws()
    {
        var path = FileSystem.Path.GetFullPath("large_file.bin");
        FileSystem.File.WriteAllBytes(path, [1]);
        var mockFileInfo = new MegTestConstants.FakeFileInfo(path, (long)uint.MaxValue + 1);
        
        // We use a custom builder to access AddBuilderInfo
        var builder = new TestOriginMegBuilder(ServiceProvider);

        var result = builder.AddWithOrigin(new MegDataEntryOriginInfo(mockFileInfo), "path", false);

        Assert.Equal(MegDataEntryAddStatus.EntryFileTooLarge, result.Status);
        Assert.Equal("The entry is too large to be added to a MEG file.", result.Message);
        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void AddFile_AddFileSize_LimitReached_Throws()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "file.txt";

        // One byte too large.
        FileSystem.Initialize().WithFile(fileToAdd).Which(x => x.HasBytesContent([1, 2, 3, 4]));

        var builder = new MaxFileSizeMegBuilder(3, ServiceProvider);

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.Equal(MegDataEntryAddStatus.EntryFileTooLarge, result.Status);
        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void AddFile_NormalizationReturnsEmpty_ReturnsFailedNormalization()
    {
        FileSystem.Initialize().WithFile("file.txt");
        var builder = new NormalizerMegBuilder(new EmptyNormalizer(), ServiceProvider);
        var result = builder.AddFile("file.txt", "somePath");
        Assert.Equal(MegDataEntryAddStatus.FailedNormalization, result.Status);
        Assert.Equal("Normalized entry path cannot be null or empty.", result.Message);
    }

    [Fact]
    public void AddFile_NormalizationThrows_ReturnsFailedNormalization()
    {
        FileSystem.Initialize().WithFile("file.txt");
        var builder = new NormalizerMegBuilder(new ThrowingNormalizer(), ServiceProvider);
        var result = builder.AddFile("file.txt", "somePath");
        Assert.Equal(MegDataEntryAddStatus.FailedNormalization, result.Status);
        Assert.Equal("Normalization failed", result.Message);
    }

    [Fact]
    public void AddFile_ValidationThrows_ReturnsInvalidEntry()
    {
        FileSystem.Initialize().WithFile("file.txt");
        var builder = new ThrowingValidatorMegBuilder(ServiceProvider);
        var result = builder.AddFile("file.txt", "somePath");
        Assert.Equal(MegDataEntryAddStatus.InvalidEntry, result.Status);
        Assert.Contains("Entry validation failed with exception", result.Message!);
        Assert.Contains("Validation failed", result.Message!);
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
            Assert.Equal(FileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FileInfo!.FullName);
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
    public void AddEntry_AddFileSize_FileTooLarge_Throws()
    {
        var meg = CreateTestMeg();
        var entry = meg.Archive[0];

        var builder = new MaxFileSizeMegBuilder(entry.Location.Size - 1, ServiceProvider);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg, entry));

        Assert.Equal(MegDataEntryAddStatus.EntryFileTooLarge, result.Status);
        Assert.Empty(builder.DataEntries);
    }

    #endregion

    #region GetMinRequiredMegFiles

    [Fact]
    public void GetMinRequiredMegFiles_EntryTooLarge_Throws()
    {
        FileSystem.File.WriteAllBytes("1.txt", [1, 2, 3]);

        // Max file size 36 is too small for MEG with a file (metadata + content)
        var builder = new MaxFileSizeMegBuilder(uint.MaxValue, ServiceProvider, 36);
        builder.AddFile("1.txt", "1.txt");

        Assert.Throws<InvalidOperationException>(() => builder.GetMinRequiredMegFiles(MegFileVersion.V1));
    }

    [Fact]
    public void GetMinRequiredMegFiles_SingleFile()
    {
        FileSystem.File.WriteAllBytes("1.txt", [1, 2, 3]);
        FileSystem.File.WriteAllBytes("2.txt", [4, 5, 6]);

        var builder = new MaxFileSizeMegBuilder(uint.MaxValue, ServiceProvider, 1000);
        builder.AddFile("1.txt", "1.txt");
        builder.AddFile("2.txt", "2.txt");

        Assert.Equal(1, builder.GetMinRequiredMegFiles(MegFileVersion.V1));
    }
    
    [Fact]
    public void GetMinRequiredMegFiles_MultipleFiles()
    {
        FileSystem.File.WriteAllBytes("1.txt", [1, 2, 3]);
        FileSystem.File.WriteAllBytes("2.txt", [4, 5, 6]);
        
        var builder = new MaxFileSizeMegBuilder(uint.MaxValue, ServiceProvider, 40);
        builder.AddFile("1.txt", "1.txt");
        builder.AddFile("2.txt", "2.txt");

        Assert.Equal(2, builder.GetMinRequiredMegFiles(MegFileVersion.V1));
    }

    #endregion

    #region BuildMany

    [Fact]
    public void BuildMany_SingleFile()
    {
        FileSystem.File.WriteAllBytes("1.txt", [1, 2, 3]);
        var builder = new MaxFileSizeMegBuilder(uint.MaxValue, ServiceProvider, 1000);
        builder.AddFile("1.txt", "1.txt");

        var fileInfo = CreateFileInfo(true, "out.meg");
        var factoryCalled = 0;
        builder.BuildMany(fileInfo, _ =>
        {
            factoryCalled++;
            return "unused.meg";
        }, true);

        Assert.Equal(0, factoryCalled);
        Assert.True(FileSystem.File.Exists("out.meg"));
    }

    [Fact]
    public void BuildMany_MultipleFiles()
    {
        FileSystem.File.WriteAllBytes("1.txt", [1, 2, 3]);
        FileSystem.File.WriteAllBytes("2.txt", [4, 5, 6]);
        
        // See GetMinRequiredMegFiles for size calculation
        var builder = new MaxFileSizeMegBuilder(uint.MaxValue, ServiceProvider, 40);
        builder.AddFile("1.txt", "1.txt");
        builder.AddFile("2.txt", "2.txt");

        var fileInfo = CreateFileInfo(true, "out.meg");
        var factoryCalled = 0;
        builder.BuildMany(fileInfo, i =>
        {
            factoryCalled++;
            return $"out_{i}.meg";
        }, true);

        Assert.Equal(2, factoryCalled);
        Assert.False(FileSystem.File.Exists("out.meg")); // Should NOT exist as it was split
        Assert.True(FileSystem.File.Exists("out_1.meg"));
        Assert.True(FileSystem.File.Exists("out_2.meg"));
    }

    [Fact]
    public void BuildMany_ThrowsOnNull()
    {
        var builder = CreateBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.BuildMany(null!, _ => "a.meg", true));
        Assert.Throws<ArgumentNullException>(() => builder.BuildMany(CreateFileInfo(true, "a.meg"), null!, true));
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

    private class MaxFileSizeMegBuilder(uint maxFileSize, IServiceProvider services, uint? maxMegSize = null) : MegBuilderBase(services)
    {
        public uint MaxEntrySize { get; } = maxFileSize;

        public override uint MaxMegFileSize => maxMegSize ?? base.MaxMegFileSize;

        public override IMegDataEntryValidator DataEntryValidator => new MaxEntrySizeValidator(MaxEntrySize);

        private class MaxEntrySizeValidator(uint maxEntrySize) : BinaryMegDataEntryValidator
        {
            protected override uint MaxMegEntrySize => maxEntrySize;
        }
    }

    private class NormalizerMegBuilder(IMegDataEntryPathNormalizer normalizer, IServiceProvider services) : MegBuilderBase(services)
    {
        public override IMegDataEntryPathNormalizer DataEntryPathNormalizer { get; } = normalizer;
    }

    private class EmptyNormalizer : IMegDataEntryPathNormalizer
    {
        public string Normalize(string entryPath) => string.Empty;
        public string Normalize(ReadOnlySpan<char> entryPath) => string.Empty;
        public bool TryNormalize(ReadOnlySpan<char> entryPath, Span<char> destination, out int charsWritten)
        {
            charsWritten = 0;
            return true;
        }
    }

    private class ThrowingNormalizer : IMegDataEntryPathNormalizer
    {
        public string Normalize(string entryPath) => throw new Exception("Normalization failed");
        public string Normalize(ReadOnlySpan<char> entryPath) => throw new Exception("Normalization failed");
        public bool TryNormalize(ReadOnlySpan<char> entryPath, Span<char> destination, out int charsWritten) => throw new Exception("Normalization failed");
    }

    private class ThrowingValidatorMegBuilder(IServiceProvider services) : MegBuilderBase(services)
    {
        public override IMegDataEntryValidator DataEntryValidator { get; } = new ThrowingValidator();

        private class ThrowingValidator : IMegDataEntryValidator
        {
            public MegDataEntryValidationResult Validate(MegDataEntryBuilderInfo dataEntry) => throw new Exception("Validation failed");
        }
    }

    private class TestOriginMegBuilder(IServiceProvider services) : MegBuilderBase(services)
    {
        public MegDataEntryAddResult AddWithOrigin(MegDataEntryOriginInfo originInfo, string entryPath, bool encrypt)
        {
            return (MegDataEntryAddResult)typeof(MegBuilderBase)
                .GetMethod("AddBuilderInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(this, [originInfo, entryPath, encrypt])!;
        }
    }
}