using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Testing;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public class MegFileServiceIntegrationTest : CommonMegTestBase
{
    private const string ContentMegFileName = "test.meg";

    private readonly IMegService _megService;

    public MegFileServiceIntegrationTest()
    {
        _megService = ServiceProvider.GetRequiredService<IMegService>();
    }

    #region Create Meg Archive

    [Fact]
    public void CreateMegArchive_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _megService.CreateMegArchive(null!, MegFileVersion.V1, null, new List<MegDataEntryBuilderInfo>()));
        Assert.Throws<ArgumentNullException>(() =>
        {
            using var fs = FileSystem.File.OpenWrite("path");
            _megService.CreateMegArchive(fs, MegFileVersion.V3, null, null!);
        });
    }

    [Fact]
    public void CreateMegArchive_DoesNotCreateDirectories_Throws()
    {
        const string megFileName = "test/a.meg";

        Assert.False(FileSystem.Directory.Exists("test"));

        Assert.Throws<DirectoryNotFoundException>(() =>
        {
            using var fs = FileSystem.File.OpenWrite(megFileName);
            _megService.CreateMegArchive(fs, MegFileVersion.V1, null, []);
        });
    }

    [Fact]
    public void CreateMegArchive_EntryNotFoundInMeg_Throws()
    {
        const string megFileName = "test.meg";
        const string dummyMegFile = "dummy.meg";
        const string newFileName = "new.meg";

        FileSystem.Initialize()
            .WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1))
            .WithFile(dummyMegFile).Which(m => m.HasBytesContent([0, 0, 0, 0, 0, 0, 0, 0]));

        var meg = _megService.Load(megFileName);

        var dummyMeg = new MegFile(new MegArchive([]), new MegFileInformation(dummyMegFile, MegFileVersion.V1),
            ServiceProvider);

        var builderInfo = new List<MegDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(new MegDataEntryLocationReference(dummyMeg, meg.Archive[0])))
        };

        Assert.Throws<EntryNotInMegException>(() =>
        {
            using var fs = FileSystem.File.OpenWrite(newFileName);
            _megService.CreateMegArchive(fs, meg.FileInformation.FileVersion, null, builderInfo);
        });
    }

    [Fact]
    public void CreateMegArchive_FilePositionMismatch_ThrowsInvalidOperationException()
    {
        const string megFileName = "new.meg";
        const string entryFileName = "file.txt";

        FileSystem.File.WriteAllBytes(entryFileName, [1, 2, 3]);
        var builderInfo = MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New(entryFileName), entryFileName);

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var fs = FileSystem.File.OpenWrite(megFileName);
            // Advance the stream so its position no longer matches the expected entry offset.
            fs.WriteByte(0);
            _megService.CreateMegArchive(fs, MegFileVersion.V1, null, [builderInfo]);
        });

        Assert.Contains("Actual file position", ex.Message);
    }

    [Fact]
    public void CreateMegArchive_DataEntrySizeMismatch_ThrowsInvalidOperationException()
    {
        const string megFileName = "new.meg";
        
        // A file whose reported length (4) disagrees with the data its stream actually yields (3 bytes)
        var fileInfo = new MegTestConstants.FakeFileInfo("file.txt", length: 4) { ReadBytes = [1, 2, 3] };
        var builderInfo = MegDataEntryBuilderInfo.FromFile(fileInfo, "file");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var fs = FileSystem.File.OpenWrite(megFileName);
            _megService.CreateMegArchive(fs, MegFileVersion.V1, null, [builderInfo]);
        });

        Assert.Contains("Actual data entry size", ex.Message);
    }

    [Fact]
    public void CreateMegArchive_MegWithEntriesOfSameNameButWithDifferentData()
    {
        const string megFileName = "test.meg";

        var expectedBytes = new byte[]
        {
            2, 0, 0, 0, 2, 0, 0, 0, // Header
            4, 0, 102, 105, 108, 101, // "file"
            4, 0, 102, 105, 108, 101, // "file"
            16, 54, 159, 140, 0, 0, 0, 0, 3, 0, 0, 0, 60, 0, 0, 0, 0, 0, 0, 0,
            16, 54, 159, 140, 1, 0, 0, 0, 3, 0, 0, 0, 63, 0, 0, 0, 1, 0, 0, 0,
            49, 50, 51, // 123
            52, 53, 54 // 456
        };


        FileSystem.Initialize().WithFile("1.txt").Which(m => m.HasStringContent("123"));
        FileSystem.Initialize().WithFile("2.txt").Which(m => m.HasStringContent("456"));

        var builderInfo = new List<MegDataEntryBuilderInfo>
        {
            MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("1.txt"), "file"),
            MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("2.txt"), "file")
        };

        using (var fs = FileSystem.File.OpenWrite(megFileName))
        {
            _megService.CreateMegArchive(fs, MegFileVersion.V1, null, builderInfo);
        }

        var bytes = FileSystem.File.ReadAllBytes(megFileName);

        Assert.Equal(expectedBytes, bytes);
    }

    #endregion

    #region Generic Read

    [Fact]
    public void Load_InvalidBinary()
    {
        const string megFileName = "test.meg";
        const string fileData = "some random data";

        FileSystem.Initialize().WithFile(megFileName).Which(m => m.HasStringContent(fileData));

        Assert.Throws<BinaryCorruptedException>(() => _megService.Load(megFileName));
    }

    [Fact]
    public void Load_ThrowFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => _megService.Load("notFound.meg"));
    }

    [Fact]
    public void Load_NullArgs()
    {
        Assert.Throws<ArgumentNullException>(() => _megService.Load((string)null!));
        Assert.Throws<ArgumentNullException>(() => _megService.Load((FileSystemStream)null!));
    }

    #endregion

    #region Read / Write V1

    [Fact]
    public void MegV1_WithEntries()
    {
        const string megFileName = "test.meg";

        FileSystem.Initialize().WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 2,
            EntryNames = new List<string>
            {
                "DATA\\XML\\CAMPAIGNFILES.XML",
                "DATA\\XML\\GAMEOBJECTFILES.XML"
            },
            NewMegFilePath = "new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = true
        };
        TestMegFiles(megFileName, expectedData);
    }

    [Fact]
    public void MegV1_Empty()
    {
        const string megFileName = "test.meg";
        const string megResource = "Files.v1_empty.meg";

        FileSystem.Initialize().WithFile(megFileName)
            .Which(m => m.HasBytesContent(TestingHelpers.GetEmbeddedResourceAsByteArray(GetType(), megResource)));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 0,
            EntryNames = new List<string>(),
            NewMegFilePath = "new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = true
        };
        TestMegFiles(megFileName, expectedData);
    }

    [Fact]
    public void MegV1_EntriesHaveNonAsciiNames()
    {
        const string megFileName = "test.meg";
        const string megResource = "Files.v1_2_files_with_extended_ascii_name.meg";

        FileSystem.Initialize().WithFile(megFileName)
            .Which(m => m.HasBytesContent(TestingHelpers.GetEmbeddedResourceAsByteArray(GetType(), megResource)));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 2,
            EntryNames = new List<string>
            {
                "TEST?.TXT",
                "TEST?.TXT"
            },
            NewMegFilePath = "new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = false
        };

        TestMegFiles(megFileName, expectedData);
    }

    #endregion

    #region GetFileVersion

    [Theory]
    [InlineData("Files.v1_1_file_data.meg", MegFileVersion.V1)]
    [InlineData("Files.v2_2_files_data.meg", MegFileVersion.V2)]
    [InlineData("Files.v3n_2_files_data.meg", MegFileVersion.V3)]
    public void GetMegFileVersion_AllOverloads_ReturnSameVersion(string megResource, MegFileVersion expectedVersion)
    {
        var bytes = TestingHelpers.GetEmbeddedResourceAsByteArray(GetType(), megResource);
        FileSystem.File.WriteAllBytes(ContentMegFileName, bytes);

        Assert.Equal(expectedVersion, _megService.GetMegFileVersion(ContentMegFileName, out var encryptedFromFile));
        Assert.False(encryptedFromFile);

        using (var stream = new MemoryStream(bytes))
        {
            Assert.Equal(expectedVersion, _megService.GetMegFileVersion(stream, out var encryptedFromStream));
            Assert.False(encryptedFromStream);
        }

        Assert.Equal(expectedVersion, _megService.GetMegFileVersion(bytes, out var encryptedFromArray));
        Assert.False(encryptedFromArray);

        Assert.Equal(expectedVersion, _megService.GetMegFileVersion(bytes.AsSpan(), out var encryptedFromSpan));
        Assert.False(encryptedFromSpan);
    }

    [Fact]
    public void GetMegFileVersion_Throws_FileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => _megService.GetMegFileVersion("notFound.meg", out _));
    }

    [Fact]
    public void GetMegFileVersion_NullArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _megService.GetMegFileVersion((string)null!, out _));
        Assert.Throws<ArgumentNullException>(() => _megService.GetMegFileVersion((Stream)null!, out _));
        Assert.Throws<ArgumentNullException>(() => _megService.GetMegFileVersion((byte[])null!, out _));
    }

    [Fact]
    public void LoadArchive_NullArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _megService.LoadArchive((Stream)null!));
        Assert.Throws<ArgumentNullException>(() => _megService.LoadArchive((byte[])null!));
    }

    #endregion

    #region Load Archive

    // The different ways a MEG archive can be loaded through IMegService. They all yield an equivalent
    // IMegDataSource; the file-based overloads additionally produce a lazily file-backed IMegFile.
    public enum LoadOverload
    {
        FilePath,
        FileStream,
        LoadArchiveFileStream,
        LoadArchiveStream,
        LoadArchiveByteArray,
        LoadArchiveSpan
    }

    [Theory]
    [InlineData(LoadOverload.FilePath)]
    [InlineData(LoadOverload.FileStream)]
    [InlineData(LoadOverload.LoadArchiveFileStream)]
    [InlineData(LoadOverload.LoadArchiveStream)]
    [InlineData(LoadOverload.LoadArchiveByteArray)]
    [InlineData(LoadOverload.LoadArchiveSpan)]
    public void Load_ParsesAndReadsEntries(LoadOverload overload)
    {
        var source = LoadContentMeg(overload);

        Assert.Equal(2, source.Archive.Count);
        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", source.Archive[0].Path);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", source.Archive[1].Path);
        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAll(source.GetData(source.Archive[0])));
        Assert.Equal(MegTestConstants.GameObjectFilesContent, ReadAll(source.GetData(source.Archive[1])));
    }

    [Theory]
    [InlineData(LoadOverload.FilePath)]
    [InlineData(LoadOverload.FileStream)]
    [InlineData(LoadOverload.LoadArchiveFileStream)]
    public void Load_FromFile_ReturnsFileBackedMeg(LoadOverload overload)
    {
        Assert.IsAssignableFrom<IMegFile>(LoadContentMeg(overload));
    }

    [Theory]
    [InlineData(LoadOverload.LoadArchiveStream)]
    [InlineData(LoadOverload.LoadArchiveByteArray)]
    [InlineData(LoadOverload.LoadArchiveSpan)]
    public void LoadArchive_FromMemory_IsNotFileBacked(LoadOverload overload)
    {
        Assert.False(LoadContentMeg(overload) is IMegFile);
    }

    [Fact]
    public void LoadArchive_NonFileStream_SourceStreamMayBeDisposed()
    {
        IMegDataSource source;
        using (var stream = new MemoryStream(MegTestConstants.ContentMegFileV1))
        { 
            source = _megService.LoadArchive(stream);
        }
        
        // The source stream is already disposed; the data is still served from the in-memory copy.
        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAll(source.GetData(source.Archive[0])));
    }

    [Fact]
    public void LoadArchive_ByteArray_InputMayBeMutated()
    {
        var data = (byte[])MegTestConstants.ContentMegFileV1.Clone();
        var source = _megService.LoadArchive(data);

        // Wiping the caller's buffer must not affect the loaded MEG (the buffer was copied defensively).
        Array.Clear(data, 0, data.Length);

        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAll(source.GetData(source.Archive[0])));
    }

    [Fact]
    public void LoadArchive_NonSeekableStream_Works()
    {
        using var nonSeekable = new NonSeekableReadStream(MegTestConstants.ContentMegFileV1);

        var source = _megService.LoadArchive(nonSeekable);

        Assert.Equal(2, source.Archive.Count);
        Assert.Equal(MegTestConstants.GameObjectFilesContent, ReadAll(source.GetData(source.Archive[1])));
    }

    [Fact]
    public void LoadArchive_InMemoryMegs_HaveDistinctNames()
    {
        var first = _megService.LoadArchive(MegTestConstants.ContentMegFileV1);
        var second = _megService.LoadArchive(MegTestConstants.ContentMegFileV1);

        // Two in-memory MEGs loaded from identical bytes should have different identifiers
        var firstDescription = new MegDataEntryLocationReference(first, first.Archive[0]).ToString();
        var secondDescription = new MegDataEntryLocationReference(second, second.Archive[0]).ToString();

        Assert.NotEqual(firstDescription, secondDescription);
    }

    // Opening Stream as input leaves a consumer the option to pass a System.IO.FileStream instance.
    // In this case, we still want to ensure the loaded MEG is using the file and not an in-memory MEG.
    [Fact]
    public void LoadArchive_RealSystemIOFileStream()
    {
        using var services = CreateRealFileSystemServiceProvider();
        var megService = services.GetRequiredService<IMegService>();

        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            File.WriteAllBytes(tempFile, MegTestConstants.ContentMegFileV1);

            IMegDataSource source;
            using (var fileStream = File.OpenRead(tempFile))
                source = megService.LoadArchive(fileStream);

            Assert.IsAssignableFrom<IMegFile>(source);
            Assert.Equal(2, source.Archive.Count);
            Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAll(source.GetData(source.Archive[0])));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private IMegDataSource LoadContentMeg(LoadOverload overload)
    {
        switch (overload)
        {
            case LoadOverload.FilePath:
                WriteContentMegToFile();
                return _megService.Load(ContentMegFileName);
            case LoadOverload.FileStream:
            {
                WriteContentMegToFile();
                using var fs = FileSystem.FileStream.New(ContentMegFileName, FileMode.Open, FileAccess.Read);
                return _megService.Load(fs);
            }
            case LoadOverload.LoadArchiveFileStream:
            {
                WriteContentMegToFile();
                using var fs = FileSystem.FileStream.New(ContentMegFileName, FileMode.Open, FileAccess.Read);
                return _megService.LoadArchive(fs);
            }
            case LoadOverload.LoadArchiveStream:
            {
                using var stream = new MemoryStream(MegTestConstants.ContentMegFileV1);
                return _megService.LoadArchive(stream);
            }
            case LoadOverload.LoadArchiveByteArray:
                return _megService.LoadArchive(MegTestConstants.ContentMegFileV1);
            case LoadOverload.LoadArchiveSpan:
                return _megService.LoadArchive(MegTestConstants.ContentMegFileV1.AsSpan());
            default:
                throw new ArgumentOutOfRangeException(nameof(overload), overload, null);
        }
    }

    private void WriteContentMegToFile()
    {
        FileSystem.File.WriteAllBytes(ContentMegFileName, MegTestConstants.ContentMegFileV1);
    }

    private static byte[] ReadAll(Stream stream)
    {
        using (stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    private static ServiceProvider CreateRealFileSystemServiceProvider()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IFileSystem>(new Testably.Abstractions.RealFileSystem());
        serviceCollection.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(serviceCollection);
        serviceCollection.SupportMEG();
        return serviceCollection.BuildServiceProvider();
    }

    #endregion

    private void TestMegFiles(string megFilePath, ExpectedMegTestData expectedData)
    {
        var megVersion = _megService.GetMegFileVersion(megFilePath, out var encrypted);
        Assert.Equal(expectedData.IsMegFileVersion, megVersion);
        Assert.Equal(expectedData.IsMegEncrypted, encrypted);

        var meg = _megService.Load(megFilePath);
        TestMegModelContent(meg, expectedData, false);

        for (var i = 0; i < meg.Archive.Count; i++)
        {
            var entry = meg.Archive[i];
            var expected = expectedData.EntryNames[i];
            Assert.Equal(expected, entry.Path);
        }

        using var param = new MegFileInformation(
            expectedData.NewMegFilePath,
            expectedData.NewMegFileVersion,
            expectedData.EncryptionData);

        var builderInformation = meg.Archive.Select(e =>
            new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, e))));

        using (var fs = FileSystem.File.OpenWrite(expectedData.NewMegFilePath))
        {
            _megService.CreateMegArchive(fs, expectedData.NewMegFileVersion, expectedData.EncryptionData,
                builderInformation);
        }

        Assert.True(FileSystem.File.Exists(expectedData.NewMegFilePath));

        var createdVersion = _megService.GetMegFileVersion(expectedData.NewMegFilePath, out var newEncrypted);
        Assert.Equal(expectedData.NewMegFileVersion, createdVersion);
        Assert.Equal(expectedData.EncryptionData is null, !newEncrypted);


        var actualBytes = FileSystem.File.ReadAllBytes(expectedData.NewMegFilePath);
        var expectedBytes = FileSystem.File.ReadAllBytes(megFilePath);
        if (expectedData.NewMegIsBinaryEqual)
            Assert.Equal(expectedBytes, actualBytes);
        else
            Assert.NotEqual(expectedBytes, actualBytes);

        var newMeg = _megService.Load(megFilePath);
        TestMegModelContent(newMeg, expectedData, true);
    }

    private static void TestMegModelContent(IMegFile meg, ExpectedMegTestData expectedData, bool isNewMeg)
    {
        Assert.NotNull(meg);
        Assert.Equal(expectedData.MegFileCount, meg.Content.Count);
        Assert.Equal(expectedData.IsMegFileVersion, meg.FileInformation.FileVersion);
        Assert.Equal(expectedData.EntryNames.Count, meg.Archive.Count);

        if (isNewMeg)
            Assert.Equal(expectedData.EncryptionData is null, !meg.FileInformation.HasEncryption);
        else
            Assert.Equal(expectedData.IsMegEncrypted, meg.FileInformation.HasEncryption);

        for (var i = 0; i < meg.Archive.Count; i++)
        {
            var entry = meg.Archive[i];
            var expected = expectedData.EntryNames[i];
            Assert.Equal(expected, entry.Path);
        }
    }

    private record ExpectedMegTestData
    {
        public MegFileVersion IsMegFileVersion { get; init; }
        public bool IsMegEncrypted { get; init; }
        public int MegFileCount { get; init; }
        public IList<string> EntryNames { get; init; } = null!;
        public string NewMegFilePath { get; init; } = null!;
        public bool NewMegIsBinaryEqual { get; init; }
        public MegFileVersion NewMegFileVersion { get; init; }
        public MegEncryptionData? EncryptionData { get; }
    }
}