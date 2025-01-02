using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.Commons;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public class MegBuilderBaseTest
{
    private TestMegBuilderInfoValidator _entryValidator = null!;
    private readonly Mock<IMegFileInformationValidator> _infoValidator = new();
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IMegFileService> _megFileService = new();

    #region Ctor

    [Fact]
    public void Test_Ctor_AbstractBase()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportMEG();
        var builderMock = new Mock<MegBuilderBase>(sc.BuildServiceProvider()) { CallBase = true };

        var builder = builderMock.Object;

        Assert.NotNull(builder.DataEntryValidator);
        Assert.NotNull(builder.MegFileInformationValidator);

        Assert.Null(builder.DataEntryPathNormalizer);
        Assert.False(builder.NormalizesEntryPaths);

        Assert.False(builder.AutomaticallyAddFileSizes);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(false, false, false)]
    public void Test_Ctor_ConcreteInstance(bool overwrite, bool addFileSize, bool useNormalizer)
    {
        Func<string, string>? normalizeFunc = useNormalizer ? s => s : null;
        var builder = CreateBuilder(overwrite, addFileSize, normalizeFunc);
        if (useNormalizer)
        {
            Assert.True(builder.NormalizesEntryPaths);
            Assert.NotNull(builder.DataEntryPathNormalizer);
        }
        else
        {
            Assert.Null(builder.DataEntryPathNormalizer);
            Assert.False(builder.NormalizesEntryPaths);
        }
        Assert.NotNull(builder.DataEntryValidator);
        Assert.NotNull(builder.MegFileInformationValidator);

        Assert.Equal(overwrite, builder.OverwritesDuplicateEntries);
        Assert.Equal(addFileSize, builder.AutomaticallyAddFileSizes);
    }

    #endregion

    #region Clear/Remove/Dispose

    [Fact]
    public void Test_AddFile_FileDoesNotExists()
    {
        var builder = CreateBuilder(false, false, null);

        var result = builder.AddFile("file.txt", "path/file.txt");

        Assert.False(result.Added);
        Assert.Equal(AddDataEntryToBuilderState.FileOrEntryNotFound, result.Status);
    }

    [Fact]
    public void Test_GetDataEntries()
    {
        var builder = CreateBuilder(false, false, null);

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        _fileSystem.Initialize().WithFile("file.txt");

        builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);
        Assert.Empty(entries);

        if (entries is List<MegFileDataEntryBuilderInfo> builderList)
        {
            builderList.Add(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path1")));
            builderList.Add(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path2")));
            builderList.Add(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path3")));
            Assert.Single(builder.DataEntries);
        }
    }

    [Fact]
    public void Test_Clear()
    {
        var builder = CreateBuilder(false, false, null);

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        _fileSystem.Initialize().WithFile("file.txt");

        builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);

        builder.Clear();

        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void Test_Remove()
    {
        var builder = CreateBuilder(false, false, null);

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        _fileSystem.Initialize();

        _fileSystem.Initialize().WithFile("file.txt");

        var result = builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);
        Assert.False(builder.Remove(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("notFound.txt"))));
        Assert.Single(builder.DataEntries);
        Assert.True(builder.Remove(result.AddedBuilderInfo!));
        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void Test_Dispose_ThrowsOnAddingOrBuildingMethods()
    {
        var builder = CreateBuilder(false, false, null);

        var entries = builder.DataEntries;
        Assert.Empty(entries);

        _fileSystem.Initialize().WithFile("file.txt");

        builder.AddFile("file.txt", "file.txt");

        Assert.Single(builder.DataEntries);

        builder.Dispose();

        Assert.Empty(builder.DataEntries);

        Assert.Throws<ObjectDisposedException>(() => builder.AddFile("file.txt", "file.txt"));
        Assert.Throws<ObjectDisposedException>(() =>
            builder.AddEntry(new MegDataEntryLocationReference(new Mock<IMegFile>().Object, MegDataEntryTest.CreateEntry("file.txt"))));
        Assert.Throws<ObjectDisposedException>(() => builder.Build(new MegFileInformation("a.meg", MegFileVersion.V1), false));

        ExceptionUtilities.AssertDoesNotThrowException(() => builder.DataEntries);
        ExceptionUtilities.AssertDoesNotThrowException(builder.Clear);
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Remove(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("notFound.txt"))));

        ExceptionUtilities.AssertDoesNotThrowException(builder.Dispose);
    }

    #endregion

    #region ValidateFileInformation

    [Fact]
    public void Test_ValidateFileInformation()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder(false, false, null);

        var fileInfo = new MegFileInformation("path", MegFileVersion.V2);

        _infoValidator.Setup(v => v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()))
            .Callback((MegBuilderFileInformationValidationData data) =>
            {
                Assert.Same(fileInfo, data.FileInformation);
                Assert.Single(data.DataEntries);
            })
            .Returns(MegFileInfoValidationResult.Valid);

        
        builder.AddFile(fileToAdd, inputEntryPath);

        builder.ValidateFileInformation(fileInfo);

        _infoValidator.Verify(v => 
                v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()), Times.Once);
    }

    #endregion

    #region AddFile

    [Fact]
    public void Test_AddFile_Throws()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder(false, false, null);

        Assert.Throws<ArgumentNullException>(() => builder.AddFile(fileToAdd, null!));
        Assert.Throws<ArgumentNullException>(() => builder.AddFile(null!, inputEntryPath));
        Assert.Throws<ArgumentException>(() => builder.AddFile("", inputEntryPath));
        Assert.Throws<ArgumentException>(() => builder.AddFile(fileToAdd, ""));
    }

    [Fact]
    public void Test_AddFile()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder(false, false, null);

        var result = builder.AddFile(fileToAdd, inputEntryPath, true);

        Assert.True(result.Added, $"Actual Result: {result.Status}");
        Assert.Equal(inputEntryPath, result.AddedBuilderInfo.FilePath);
        Assert.Null(result.OverwrittenBuilderInfo);

        Assert.Single(builder.DataEntries);

        var entry = builder.DataEntries.First();
        Assert.Equal(inputEntryPath, entry.FilePath);
        Assert.True(entry.Encrypted);
        Assert.Null(entry.Size);
        Assert.Equal(_fileSystem.Path.GetFullPath(fileToAdd), entry.OriginInfo.FilePath);
    }

    delegate void NormalizerCallBack(ref string path, out string? message);

    [Fact]
    public void Test_AddFile_Normalizer()
    {
        const string fileToAdd = "file.txt"; 
        const string normalizedPath = "NORMALIZED";

        var inputEntryPath = "path/file.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder(false, false, input =>
        {
            Assert.Equal(inputEntryPath, input);
            return normalizedPath;

        });

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.True(result.Added);
        Assert.Equal(normalizedPath, result.AddedBuilderInfo.FilePath);
    }

    [Fact]
    public void Test_AddFile_Normalizer_Fails()
    {
        const string fileToAdd = "file.txt";

        var inputEntryPath = "path/file.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder(false, false, input =>
        {
            Assert.Equal(inputEntryPath, input);
            throw new Exception();
        });

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.Equal(AddDataEntryToBuilderState.FailedNormalization, result.Status);
        Assert.Null(result.AddedBuilderInfo);
    }

    [Fact]
    public void Test_AddFile_AssureEncoding_WithNormalization()
    {
        const string fileToAdd = "file.txt";
        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        var inputEntryPath = "path/fileWithNonAsciiÖ.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        // Make sure the normalizer triggers
        var builder = CreateBuilder(false, false, input => input);

        var result = builder.AddFile(fileToAdd, inputEntryPath);
        Assert.True(result.Added);
        Assert.Equal(expectedEncodedEntry, result.AddedBuilderInfo.FilePath);
    }

    [Fact]
    public void Test_AddFile_AssureEncoding_WithoutNormalization()
    {
        const string fileToAdd = "file.txt";
        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        var inputEntryPath = "path/fileWithNonAsciiÖ.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder(false, false, null);

        var result = builder.AddFile(fileToAdd, inputEntryPath);
        Assert.True(result.Added);
        Assert.Equal(expectedEncodedEntry, result.AddedBuilderInfo.FilePath);
    }

    [Fact]
    public void Test_AddFile_LongStringHandling()
    {
        const string fileToAdd = "file.txt";

        var firstPart = new string('a', 150);
        var secondPart = new string('ö', 150);

        _fileSystem.Initialize().WithFile(fileToAdd);

        // Make sure the normalizer triggers
        var builder = CreateBuilder(false, false, input => input.Replace('a', 'b'));

        var result = builder.AddFile(fileToAdd, firstPart + secondPart);
        Assert.True(result.Added);
        Assert.Equal(new string('b', 150) + new string('?', 150), result.AddedBuilderInfo.FilePath);
    }

    [Fact]
    public void Test_AddFile_DoNotOverride()
    {
        const string fileToAdd = "file1.txt";
        const string otherFileToAdd = "file2.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);
        _fileSystem.Initialize().WithFile(otherFileToAdd);

        var builder = CreateBuilder(false, false, null);

        builder.AddFile(fileToAdd, inputEntryPath);

        var resultSecondAdd = builder.AddFile(otherFileToAdd, inputEntryPath);

        Assert.Equal(AddDataEntryToBuilderState.DuplicateEntry, resultSecondAdd.Status);
        Assert.Single(builder.DataEntries);
        Assert.Null(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.False(resultSecondAdd.WasOverwrite);
        Assert.Equal(_fileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FilePath);
    }

    [Fact]
    public void Test_AddFile_DoOverride()
    {
        const string file = "file1.txt";
        const string inputEntryPath = "path/fileWithNonAsciiÖ.txt";
       
        const string otherFile = "file2.txt";
        const string otherEntryPath = "path/fileWithNonAsciiÄ.txt";

        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        _fileSystem.Initialize().WithFile(file);
        _fileSystem.Initialize().WithFile(otherFile);

        var builder = CreateBuilder(true, false, null);

        builder.AddFile(file, inputEntryPath);

        var resultSecondAdd = builder.AddFile(otherFile, otherEntryPath);

        Assert.True(resultSecondAdd.Added);
        Assert.Single(builder.DataEntries);

        Assert.NotNull(resultSecondAdd.AddedBuilderInfo);
        Assert.NotNull(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.True(resultSecondAdd.WasOverwrite);
        Assert.Equal(_fileSystem.Path.GetFullPath(file), resultSecondAdd.OverwrittenBuilderInfo.OriginInfo.FilePath);
        Assert.Equal(_fileSystem.Path.GetFullPath(otherFile), resultSecondAdd.AddedBuilderInfo.OriginInfo.FilePath);

        // Assert that duplicate check was based on encoded (thus also normalized) file path, cause the original inputs have different values.
        Assert.Equal(expectedEncodedEntry, resultSecondAdd.AddedBuilderInfo.FilePath);
    }

    [Fact]
    public void Test_AddFile_ValidatorFails()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/fileWithNonAsciiÖ.txt";
        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var builder = CreateBuilder(false, false, null, false);

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.Equal(AddDataEntryToBuilderState.InvalidEntry, result.Status);
        Assert.Empty(builder.DataEntries);

        Assert.Equal(expectedEncodedEntry, _entryValidator.Path);
    }

    [Fact]
    public void Test_AddFile_AddFileSize()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.Initialize().WithFile(fileToAdd).Which(m => m.HasBytesContent([1, 2, 3, 4, 5]));

        var builder = CreateBuilder(false, true, null);

        var result = builder.AddFile(fileToAdd, inputEntryPath, true);

        Assert.True(result.Added, $"Actual Result: {result.Status}");
        Assert.Equal(5u, result.AddedBuilderInfo.Size);

        Assert.Single(builder.DataEntries);
    }

    [Fact]
    public void Test_AddFile_AddFileSize_FileTooLarge_Throws()
    {
        var fs = new Mock<IFileSystem>();

        var sc = new ServiceCollection();
        sc.SupportMEG();
        PetroglyphCommons.ContributeServices(sc); 
        sc.AddSingleton(_ => fs.Object);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.AddSingleton(_ => _megFileService.Object);
        sc.AddSingleton(_ => _infoValidator.Object);

        // Default Validator always passes
        _entryValidator = new TestMegBuilderInfoValidator(true);

        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        var fi = new Mock<IFileInfo>();
        fi.SetupGet(x => x.Exists).Returns(true);
        fi.SetupGet(x => x.Length).Returns(uint.MaxValue + 1L);
        var fif = new Mock<IFileInfoFactory>();
        fif.Setup(x => x.New(fileToAdd)).Returns(fi.Object);
        fs.Setup(x => x.FileInfo).Returns(fif.Object);

        var builder = new TestingMegBuilder(false, true, _entryValidator,
            _infoValidator.Object, sc.BuildServiceProvider());

        var result = builder.AddFile(fileToAdd, inputEntryPath, true);

        Assert.Equal(AddDataEntryToBuilderState.EntryFileTooLarge, result.Status);
        Assert.Empty(builder.DataEntries);
    }

    #endregion

    #region AddEntry

    [Fact]
    public void Test_AddEntry_Throws()
    {
        var builder = CreateBuilder(false, false, null);

        Assert.Throws<ArgumentNullException>(() => builder.AddEntry(null!, "path"));
        Assert.Throws<ArgumentException>(() =>
            builder.AddEntry(new MegDataEntryLocationReference(new Mock<IMegFile>().Object, MegDataEntryTest.CreateEntry("path")), ""));
    }

    [Fact]
    public void Test_AddEntry_EntryNotFound()
    {
        var builder = CreateBuilder(false, false, null);

        var entry = MegDataEntryTest.CreateEntry("file.txt");

        var archive = new MegArchive(new List<MegDataEntry>());
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry));

        Assert.Equal(AddDataEntryToBuilderState.FileOrEntryNotFound, result.Status);
        Assert.Empty(builder.DataEntries);
    }

    [Fact]
    public void Test_AddEntry()
    {
        var builder = CreateBuilder(false, false, null);

        var entry = MegDataEntryTest.CreateEntry("file.txt");

        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry));

        Assert.True(result.Added);
        Assert.Single(builder.DataEntries);

        var actualEntry = builder.DataEntries.First();
        Assert.Equal("file.txt", actualEntry.FilePath);
        Assert.Same(entry, actualEntry.OriginInfo.MegFileLocation!.DataEntry);
    }

    [Fact]
    public void Test_AddEntry_OverrideProperties()
    {
        var builder = CreateBuilder(false, false, null);

        var entry = MegDataEntryTest.CreateEntry("file.txt", default, default, false, null);

        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), "new.txt", true);

        Assert.True(result.Added);
        Assert.Single(builder.DataEntries);

        var actualEntry = builder.DataEntries.First();
        Assert.Equal("new.txt", actualEntry.FilePath);
        Assert.True(actualEntry.Encrypted);
        Assert.Same(entry, actualEntry.OriginInfo.MegFileLocation!.DataEntry);
    }

    [Theory]
    [InlineData("file.txt", null)]
    [InlineData("file.txt", "new.txt")]
    public void Test_AddEntry_Normalizer(string orgPath, string? overridePath)
    {
        const string normalizedPath = "norm";

        var inputEntryPath = overridePath ?? orgPath;

        var entry = MegDataEntryTest.CreateEntry(orgPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);


        var builder = CreateBuilder(false, false, input =>
        {
            Assert.Equal(inputEntryPath, input);
            return normalizedPath;
        });

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.True(result.Added);
        Assert.Equal(normalizedPath, result.AddedBuilderInfo.FilePath);
    }

    [Theory]
    [InlineData("file.txt", null)]
    [InlineData("file.txt", "new.txt")]
    public void Test_AddEntry_Normalizer_Fails(string orgPath, string? overridePath)
    {
        var inputEntryPath = overridePath ?? orgPath;

        var entry = MegDataEntryTest.CreateEntry(orgPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(false, false, input =>
        {
            Assert.Equal(inputEntryPath, input);
            throw new Exception();
        });

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.Equal(AddDataEntryToBuilderState.FailedNormalization, result.Status);
        Assert.Null(result.AddedBuilderInfo);
    }

    [Fact]
    public void Test_AddEntry_Normalizer_LongerThanOriginal_Throws()
    {
        const string input = "entry.txt";

        var entry = MegDataEntryTest.CreateEntry(input, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);


        var builder = CreateBuilder(false, false, i =>
        {
            Assert.Equal(input, i);
            return input + "a";
        });


        var added = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry));
        Assert.True(added.Added);
    }

    [Fact]
    public void Test_AddEntry_AssureEncoding()
    {
        const string entryPath = "file.txt";
        const string overridePath = "newÄ.txt";
        const string expectedEntryPath = "new?.txt";

        var entry = MegDataEntryTest.CreateEntry(entryPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        // Make sure the normalizer triggers
        var builder = CreateBuilder(false, false, input => input);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);
        Assert.True(result.Added);
        Assert.Equal(expectedEntryPath, result.AddedBuilderInfo.FilePath);
    }

    [Theory]
    [InlineData("file.txt", null)]
    [InlineData("file.txt", "new.txt")]
    public void Test_AddEntry_DoNotOverride(string entryPath, string? overridePath)
    {
        const string fileToAdd = "file1.txt";

        _fileSystem.Initialize().WithFile(fileToAdd);

        var entry = MegDataEntryTest.CreateEntry(entryPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(false, false, null);

        // Use AddFile here to assert that AddFile and AddEntry work when combined.
        builder.AddFile(fileToAdd, overridePath ?? entryPath);

        var resultSecondAdd = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.Equal(AddDataEntryToBuilderState.DuplicateEntry, resultSecondAdd.Status);
        Assert.Single(builder.DataEntries);
        Assert.Null(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.False(resultSecondAdd.WasOverwrite);
        Assert.Equal(_fileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FilePath);
    }

    [Fact]
    public void Test_AddEntry_DoOverride()
    {
        const string file = "file1.txt";
        const string inputEntryPath = "path/fileWithNonAsciiÖ.txt";

        const string otherEntryPath = "path/fileWithNonAsciiÄ.txt";

        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        _fileSystem.Initialize().WithFile(file);

        var entry = MegDataEntryTest.CreateEntry("file.txt", default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(true, false, null);

        // Use AddFile here to assert that AddFile and AddEntry work when combined.
        builder.AddFile(file, inputEntryPath);

        var resultSecondAdd = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), otherEntryPath);

        Assert.True(resultSecondAdd.Added);
        Assert.Single(builder.DataEntries);

        Assert.NotNull(resultSecondAdd.AddedBuilderInfo);
        Assert.NotNull(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.True(resultSecondAdd.WasOverwrite);
        Assert.Equal(_fileSystem.Path.GetFullPath(file), resultSecondAdd.OverwrittenBuilderInfo.OriginInfo.FilePath);
        Assert.Same(entry, resultSecondAdd.AddedBuilderInfo.OriginInfo.MegFileLocation!.DataEntry);

        // Assert that duplicate check was based on encoded (thus also normalized) file path, cause the original inputs have different values.
        Assert.Equal(expectedEncodedEntry, resultSecondAdd.AddedBuilderInfo.FilePath);
    }

    [Fact]
    public void Test_AddEntry_ValidatorFails()
    {
        const string overridePath = "path/fileWithNonAsciiÖ.txt";
        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        var entry = MegDataEntryTest.CreateEntry("file.txt", default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(false, false, null, false);


        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.Equal(AddDataEntryToBuilderState.InvalidEntry, result.Status);
        Assert.Empty(builder.DataEntries);

        Assert.Equal(expectedEncodedEntry, _entryValidator.Path);
    }

    #endregion

    #region Build

    delegate void MegFileServiceCreateCallBack(
        FileSystemStream fileStream, 
        MegFileVersion megVersion, 
        MegEncryptionData encryptionData, 
        IEnumerable<MegFileDataEntryBuilderInfo> entries);

    [Fact]
    public void Test_Build()
    {
        var fileInfo = new MegFileInformation("path/a.meg", MegFileVersion.V1);

        var builder = CreateBuilder(false, false, null);

        var expectedData = new byte[] { 1, 2, 3 };

        _megFileService.Setup(s => s.CreateMegArchive(It.IsAny<FileSystemStream>(), fileInfo.FileVersion,
                fileInfo.EncryptionData,
                It.IsAny<IEnumerable<MegFileDataEntryBuilderInfo>>()))
            .Callback(new MegFileServiceCreateCallBack((stream, _, _, _) =>
            {
                using var ms = new MemoryStream(expectedData);
                ms.CopyTo(stream);
            }));

        builder.Build(fileInfo, false);
        Assert.Equal(expectedData, _fileSystem.File.ReadAllBytes(fileInfo.FilePath));
    }

    #endregion

    private MegBuilderBase CreateBuilder(bool overwrite, bool addFileSize, Func<string, string>? normalizerAction, bool validationResult = true)
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(sc);
        sc.AddSingleton(_ => _megFileService.Object);

        _entryValidator = new TestMegBuilderInfoValidator(validationResult);

        if (normalizerAction is not null)
            sc.AddSingleton<TestEntryNormalizer>(sp => new TestEntryNormalizer(normalizerAction, sp));

        // Default Validator always passes
        _infoValidator.Setup(v => v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()))
            .Returns(MegFileInfoValidationResult.Valid);

        return new TestingMegBuilder(
            overwrite,
            addFileSize,
            _entryValidator,
            _infoValidator.Object,
            sc.BuildServiceProvider());
    }

    private class TestingMegBuilder(
        bool overwrite,
        bool addFileSize,
        TestMegBuilderInfoValidator entryValidator,
        IMegFileInformationValidator megFileInformationValidator,
        IServiceProvider services)
        : MegBuilderBase(services)
    {
        public override bool OverwritesDuplicateEntries { get; } = overwrite;

        public override bool AutomaticallyAddFileSizes { get; } = addFileSize;

        public override IMegDataEntryPathNormalizer? DataEntryPathNormalizer => TestNormalizer;

        public TestEntryNormalizer? TestNormalizer { get; } = services.GetService<TestEntryNormalizer>();


        public override IMegBuilderInfoValidator DataEntryValidator { get; } = entryValidator;

        public override IMegFileInformationValidator MegFileInformationValidator { get; } = megFileInformationValidator;
    }

    private class TestMegBuilderInfoValidator(bool validationResult) : IMegBuilderInfoValidator
    {
        public string Path { get; private set; }
        public bool Encrypted { get; private set; }
        public uint? Size { get; private set; }

        public bool Validate(MegFileDataEntryBuilderInfo builderInfo)
        {
            return Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size);
        }

        public bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size)
        {
            Path = entryPath.ToString();
            Encrypted = encrypted;
            Size = size;
            return validationResult;
        }
    }

    private class TestEntryNormalizer(Func<string, string> normalizeAction, IServiceProvider serviceProvider) : MegDataEntryPathNormalizerBase(serviceProvider)
    { 
        public override void Normalize(ReadOnlySpan<char> filePath, ref ValueStringBuilder stringBuilder)
        {
            var result = normalizeAction(filePath.ToString()).AsSpan();
            stringBuilder.Append(result);
        }
    }
}