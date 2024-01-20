using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class MegBuilderBaseTest
{
    private readonly Mock<IBuilderInfoValidator> _entryValidator = new();
    private readonly Mock<IMegFileInformationValidator> _infoValidator = new();
    private readonly Mock<IMegDataEntryPathNormalizer> _normalizer = new();
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IMegFileService> _megFileService = new();

    #region Ctor

    [TestMethod]
    public void Test_Ctor_AbstractBase()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        var builderMock = new Mock<MegBuilderBase>(sc.BuildServiceProvider()) { CallBase = true };

        var builder = builderMock.Object;

        Assert.IsNotNull(builder.DataEntryValidator);
        Assert.IsNotNull(builder.MegFileInformationValidator);

        Assert.IsNull(builder.DataEntryPathNormalizer);
        Assert.IsFalse(builder.NormalizesEntryPaths);

        Assert.IsFalse(builder.AutomaticallyAddFileSizes);
    }

    [TestMethod]
    [DataRow(true, true, true)]
    [DataRow(false, false, false)]
    public void Test_Ctor_ConcreteInstance(bool overwrite, bool addFileSize, bool useNormalizer)
    {
        var builder = CreateBuilder(overwrite, addFileSize, useNormalizer);
        if (useNormalizer)
        {
            Assert.IsTrue(builder.NormalizesEntryPaths);
            Assert.IsNotNull(builder.DataEntryPathNormalizer);
        }
        else
        {
            Assert.IsNull(builder.DataEntryPathNormalizer);
            Assert.IsFalse(builder.NormalizesEntryPaths);
        }
        Assert.IsNotNull(builder.DataEntryValidator);
        Assert.IsNotNull(builder.MegFileInformationValidator);

        Assert.AreEqual(overwrite, builder.OverwritesDuplicateEntries);
        Assert.AreEqual(addFileSize, builder.AutomaticallyAddFileSizes);
    }

    #endregion

    #region Clear/Remove/Dispose

    [TestMethod]
    public void Test_AddFile_FileDoesNotExists()
    {
        var builder = CreateBuilder(false, false, false);

        var result = builder.AddFile("file.txt", "path/file.txt");

        Assert.IsFalse(result.Added);
        Assert.AreEqual(AddDataEntryToBuilderState.FileOrEntryNotFound, result.Status);
    }

    [TestMethod]
    public void Test_GetDataEntries()
    {
        var builder = CreateBuilder(false, false, false);

        var entries = builder.DataEntries;
        Assert.AreEqual(0, entries.Count);

        _fileSystem.AddEmptyFile("file.txt");

        builder.AddFile("file.txt", "file.txt");

        Assert.AreEqual(1, builder.DataEntries.Count);
        Assert.AreEqual(0, entries.Count);

        if (entries is List<MegFileDataEntryBuilderInfo> builderList)
        {
            builderList.Add(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path1")));
            builderList.Add(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path2")));
            builderList.Add(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path3")));
            Assert.AreEqual(1, builder.DataEntries.Count);
        }
    }

    [TestMethod]
    public void Test_Clear()
    {
        var builder = CreateBuilder(false, false, false);

        var entries = builder.DataEntries;
        Assert.AreEqual(0, entries.Count);

        _fileSystem.AddEmptyFile("file.txt");

        builder.AddFile("file.txt", "file.txt");

        Assert.AreEqual(1, builder.DataEntries.Count);

        builder.Clear();

        Assert.AreEqual(0, builder.DataEntries.Count);
    }

    [TestMethod]
    public void Test_Remove()
    {
        var builder = CreateBuilder(false, false, false);

        var entries = builder.DataEntries;
        Assert.AreEqual(0, entries.Count);

        _fileSystem.AddEmptyFile("file.txt");

        var result = builder.AddFile("file.txt", "file.txt");

        Assert.AreEqual(1, builder.DataEntries.Count);
        Assert.IsFalse(builder.Remove(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("notFound.txt"))));
        Assert.AreEqual(1, builder.DataEntries.Count);
        Assert.IsTrue(builder.Remove(result.AddedBuilderInfo!));
        Assert.AreEqual(0, builder.DataEntries.Count);
    }

    [TestMethod]
    public void Test_Dispose_ThrowsOnAddingOrBuildingMethods()
    {
        var builder = CreateBuilder(false, false, false);

        var entries = builder.DataEntries;
        Assert.AreEqual(0, entries.Count);

        _fileSystem.AddEmptyFile("file.txt");

        builder.AddFile("file.txt", "file.txt");

        Assert.AreEqual(1, builder.DataEntries.Count);

        builder.Dispose();

        Assert.AreEqual(0, builder.DataEntries.Count);

        Assert.ThrowsException<ObjectDisposedException>(() => builder.AddFile("file.txt", "file.txt"));
        Assert.ThrowsException<ObjectDisposedException>(() =>
            builder.AddEntry(new MegDataEntryLocationReference(new Mock<IMegFile>().Object, MegDataEntryTest.CreateEntry("file.txt"))));
        Assert.ThrowsException<ObjectDisposedException>(() => builder.Build(new MegFileInformation("a.meg", MegFileVersion.V1), false));

        ExceptionUtilities.AssertDoesNotThrowException(() => builder.DataEntries);
        ExceptionUtilities.AssertDoesNotThrowException(builder.Clear);
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Remove(new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("notFound.txt"))));

        ExceptionUtilities.AssertDoesNotThrowException(builder.Dispose);
    }

    #endregion

    #region ValidateFileInformation

    [TestMethod]
    public void Test_ValidateFileInformation()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        var builder = CreateBuilder(false, false, false);
        Assert.ThrowsException<ArgumentNullException>(() => builder.ValidateFileInformation(null!));

        var fileInfo = new MegFileInformation("path", MegFileVersion.V2);

        _infoValidator.Setup(v => v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()))
            .Callback((MegBuilderFileInformationValidationData data) =>
            {
                Assert.AreSame(fileInfo, data.FileInformation);
                Assert.AreEqual(1, data.DataEntries.Count);
            })
            .Returns(new ValidationResult());

        builder.AddFile(fileToAdd, inputEntryPath);

        builder.ValidateFileInformation(fileInfo);

        _infoValidator.Verify(v => 
                v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()), Times.Once);
    }

    #endregion

    #region AddFile

    [TestMethod]
    public void Test_AddFile_Throws()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        var builder = CreateBuilder(false, false, false);

        Assert.ThrowsException<ArgumentNullException>(() => builder.AddFile(fileToAdd, null!));
        Assert.ThrowsException<ArgumentNullException>(() => builder.AddFile(null!, inputEntryPath));
        Assert.ThrowsException<ArgumentException>(() => builder.AddFile("", inputEntryPath));
        Assert.ThrowsException<ArgumentException>(() => builder.AddFile(fileToAdd, ""));
    }

    [TestMethod]
    public void Test_AddFile()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        var builder = CreateBuilder(false, false, false);

        var result = builder.AddFile(fileToAdd, inputEntryPath, true);

        Assert.IsTrue(result.Added, $"Actual Result: {result.Status}");
        Assert.AreEqual(inputEntryPath, result.AddedBuilderInfo.FilePath);
        Assert.IsNull(result.OverwrittenBuilderInfo);

        Assert.AreEqual(1, builder.DataEntries.Count);

        var entry = builder.DataEntries.First();
        Assert.AreEqual(inputEntryPath, entry.FilePath);
        Assert.IsTrue(entry.Encrypted);
        Assert.IsNull(entry.Size);
        Assert.AreEqual(_fileSystem.Path.GetFullPath(fileToAdd), entry.OriginInfo.FilePath);

        _entryValidator.Verify(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()), Times.Once);
    }

    delegate void NormalizerCallBack(ref string path, out string? message);

    [TestMethod]
    public void Test_AddFile_Normalizer()
    {
        const string fileToAdd = "file.txt";
        const bool normalizerResult = true;
        const string normalizedPath = "NORMALIZED";

        var inputEntryPath = "path/file.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        string? normalizerMessage = null;

        var builder = CreateBuilder(false, false, true);

        _normalizer.Setup(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage))
            .Callback(new NormalizerCallBack((ref string path, out string? message) =>
            {
                message = null;

                Assert.AreEqual(inputEntryPath, path);
                path = normalizedPath;
            })).Returns(normalizerResult);

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.IsTrue(result.Added);
        Assert.AreEqual(normalizedPath, result.AddedBuilderInfo.FilePath);

        _normalizer.Verify(n => n.TryNormalizePath(ref It.Ref<string>.IsAny, out normalizerMessage), Times.Once);
    }

    [TestMethod]
    public void Test_AddFile_Normalizer_Fails()
    {
        const string fileToAdd = "file.txt";
        const bool normalizerResult = false;
        const string normalizedPath = "NORMALIZED";

        var inputEntryPath = "path/file.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        string? normalizerMessage = null;

        var builder = CreateBuilder(false, false, true);

        _normalizer.Setup(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage))
            .Callback(new NormalizerCallBack((ref string path, out string? message) =>
            {
                message = null;
                Assert.AreEqual(inputEntryPath, path);
                path = normalizedPath;
            })).Returns(normalizerResult);

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.AreEqual(AddDataEntryToBuilderState.FailedNormalization,result.Status);
        Assert.IsNull(result.AddedBuilderInfo);

        _normalizer.Verify(n => n.TryNormalizePath(ref It.Ref<string>.IsAny, out normalizerMessage), Times.Once);
    }

    [TestMethod]
    public void Test_AddFile_AssureEncoding()
    {
        const string fileToAdd = "file.txt";
        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        var inputEntryPath = "path/fileWithNonAsciiÖ.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        string? normalizerMessage = null;

        var builder = CreateBuilder(false, false, true);

        _normalizer.Setup(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage))
            .Callback(new NormalizerCallBack((ref string path, out string? message) =>
            {
                message = null;
                // Assure that normalization is happening before encoding
                Assert.AreEqual(inputEntryPath, path);
            })).Returns(true);

        var result = builder.AddFile(fileToAdd, inputEntryPath);
        Assert.IsTrue(result.Added);
        Assert.AreEqual(expectedEncodedEntry, result.AddedBuilderInfo.FilePath);

        _normalizer.Verify(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage), Times.Once);
    }

    [TestMethod]
    public void Test_AddFile_DoNotOverride()
    {
        const string fileToAdd = "file1.txt";
        const string otherFileToAdd = "file2.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.AddEmptyFile(fileToAdd);
        _fileSystem.AddEmptyFile(otherFileToAdd);

        var builder = CreateBuilder(false, false, false);

        builder.AddFile(fileToAdd, inputEntryPath);

        var resultSecondAdd = builder.AddFile(otherFileToAdd, inputEntryPath);

        Assert.AreEqual(AddDataEntryToBuilderState.DuplicateEntry, resultSecondAdd.Status);
        Assert.AreEqual(1, builder.DataEntries.Count);
        Assert.IsNull(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.IsFalse(resultSecondAdd.WasOverwrite);
        Assert.AreEqual(_fileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FilePath);
    }

    [TestMethod]
    public void Test_AddFile_DoOverride()
    {
        const string file = "file1.txt";
        const string inputEntryPath = "path/fileWithNonAsciiÖ.txt";
       
        const string otherFile = "file2.txt";
        const string otherEntryPath = "path/fileWithNonAsciiÄ.txt";

        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        _fileSystem.AddEmptyFile(file);
        _fileSystem.AddEmptyFile(otherFile);

        var builder = CreateBuilder(true, false, false);

        builder.AddFile(file, inputEntryPath);

        var resultSecondAdd = builder.AddFile(otherFile, otherEntryPath);

        Assert.IsTrue(resultSecondAdd.Added);
        Assert.AreEqual(1, builder.DataEntries.Count);

        Assert.IsNotNull(resultSecondAdd.AddedBuilderInfo);
        Assert.IsNotNull(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.IsTrue(resultSecondAdd.WasOverwrite);
        Assert.AreEqual(_fileSystem.Path.GetFullPath(file), resultSecondAdd.OverwrittenBuilderInfo.OriginInfo.FilePath);
        Assert.AreEqual(_fileSystem.Path.GetFullPath(otherFile), resultSecondAdd.AddedBuilderInfo.OriginInfo.FilePath);

        // Assert that duplicate check was based on encoded (thus also normalized) file path, cause the original inputs have different values.
        Assert.AreEqual(expectedEncodedEntry, resultSecondAdd.AddedBuilderInfo.FilePath);
    }

    [TestMethod]
    public void Test_AddFile_ValidatorFails()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/fileWithNonAsciiÖ.txt";
        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        var builder = CreateBuilder(false, false, false);

        _entryValidator.Setup(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()))
            .Callback((MegFileDataEntryBuilderInfo builderInfo) =>
            {
                // Assert that the validator already has the encoded (and thus normalized) path.
                Assert.AreEqual(expectedEncodedEntry, builderInfo.FilePath);
            })
            .Returns(new ValidationResult(new List<ValidationFailure> { new("someError", "some error") }));

        var result = builder.AddFile(fileToAdd, inputEntryPath);

        Assert.AreEqual(AddDataEntryToBuilderState.InvalidEntry, result.Status);
        Assert.AreEqual(0, builder.DataEntries.Count);

        _entryValidator.Verify(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()), Times.Once);
    }

    [TestMethod]
    public void Test_AddFile_AddFileSize()
    {
        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        _fileSystem.AddFile(fileToAdd, new MockFileData([1, 2, 3, 4, 5]));

        var builder = CreateBuilder(false, true, false);

        var result = builder.AddFile(fileToAdd, inputEntryPath, true);

        Assert.IsTrue(result.Added, $"Actual Result: {result.Status}");
        Assert.AreEqual(5u, result.AddedBuilderInfo.Size);

        Assert.AreEqual(1, builder.DataEntries.Count);

        _entryValidator.Verify(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()), Times.Once);
    }

    [TestMethod]
    public void Test_AddFile_AddFileSize_FileTooLarge_Throws()
    {
        var fs = new Mock<IFileSystem>();

        var sc = new ServiceCollection();
        sc.AddSingleton(_ => fs.Object);
        sc.AddSingleton(_ => _megFileService.Object);

        // Default Validator always passes
        _entryValidator.Setup(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()))
            .Returns(new ValidationResult());

        const string fileToAdd = "file.txt";
        const string inputEntryPath = "path/file.txt";

        var fi = new Mock<IFileInfo>();
        fi.SetupGet(x => x.Exists).Returns(true);
        fi.SetupGet(x => x.Length).Returns(uint.MaxValue + 1L);
        var fif = new Mock<IFileInfoFactory>();
        fif.Setup(x => x.New(fileToAdd)).Returns(fi.Object);
        fs.Setup(x => x.FileInfo).Returns(fif.Object);

        var builder = new TestingMegBuilder(false, true, _normalizer.Object, _entryValidator.Object,
            _infoValidator.Object, sc.BuildServiceProvider());

        var result = builder.AddFile(fileToAdd, inputEntryPath, true);

        Assert.AreEqual(AddDataEntryToBuilderState.EntryFileTooLarge, result.Status);
        Assert.AreEqual(0, builder.DataEntries.Count);
    }

    #endregion

    #region AddEntry

    [TestMethod]
    public void Test_AddEntry_Throws()
    {
        var builder = CreateBuilder(false, false, false);

        Assert.ThrowsException<ArgumentNullException>(() => builder.AddEntry(null!, "path"));
        Assert.ThrowsException<ArgumentException>(() =>
            builder.AddEntry(new MegDataEntryLocationReference(new Mock<IMegFile>().Object, MegDataEntryTest.CreateEntry("path")), ""));
    }

    [TestMethod]
    public void Test_AddEntry_EntryNotFound()
    {
        var builder = CreateBuilder(false, false, false);

        var entry = MegDataEntryTest.CreateEntry("file.txt");

        var archive = new MegArchive(new List<MegDataEntry>());
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry));

        Assert.AreEqual(AddDataEntryToBuilderState.FileOrEntryNotFound, result.Status);
        Assert.AreEqual(0, builder.DataEntries.Count);
    }

    [TestMethod]
    public void Test_AddEntry()
    {
        var builder = CreateBuilder(false, false, false);

        var entry = MegDataEntryTest.CreateEntry("file.txt");

        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry));

        Assert.IsTrue(result.Added);
        Assert.AreEqual(1, builder.DataEntries.Count);

        var actualEntry = builder.DataEntries.First();
        Assert.AreEqual("file.txt", actualEntry.FilePath);
        Assert.AreSame(entry, actualEntry.OriginInfo.MegFileLocation!.DataEntry);
    }

    [TestMethod]
    public void Test_AddEntry_OverrideProperties()
    {
        var builder = CreateBuilder(false, false, false);

        var entry = MegDataEntryTest.CreateEntry("file.txt", default, default, false, null);

        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), "new.txt", true);

        Assert.IsTrue(result.Added);
        Assert.AreEqual(1, builder.DataEntries.Count);

        var actualEntry = builder.DataEntries.First();
        Assert.AreEqual("new.txt", actualEntry.FilePath);
        Assert.IsTrue(actualEntry.Encrypted);
        Assert.AreSame(entry, actualEntry.OriginInfo.MegFileLocation!.DataEntry);
    }

    [TestMethod]
    [DataRow("file.txt", null)]
    [DataRow("file.txt", "new.txt")]
    public void Test_AddEntry_Normalizer(string orgPath, string? overridePath)
    {
        const bool normalizerResult = true;
        const string normalizedPath = "NORMALIZED";

        var inputEntryPath = overridePath ?? orgPath;

        string? normalizerMessage = null;

        var entry = MegDataEntryTest.CreateEntry(orgPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);


        var builder = CreateBuilder(false, false, true);

        _normalizer.Setup(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage))
            .Callback(new NormalizerCallBack((ref string path, out string? message) =>
            {
                message = null;

                Assert.AreEqual(inputEntryPath, path);
                path = normalizedPath;
            })).Returns(normalizerResult);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.IsTrue(result.Added);
        Assert.AreEqual(normalizedPath, result.AddedBuilderInfo.FilePath);

        _normalizer.Verify(n => n.TryNormalizePath(ref It.Ref<string>.IsAny, out normalizerMessage), Times.Once);
    }

    [TestMethod]
    [DataRow("file.txt", null)]
    [DataRow("file.txt", "new.txt")]
    public void Test_AddEntry_Normalizer_Fails(string orgPath, string? overridePath)
    {
        const bool normalizerResult = false;
        const string normalizedPath = "NORMALIZED";

        var inputEntryPath = overridePath ?? orgPath;

        string? normalizerMessage = null;

        var entry = MegDataEntryTest.CreateEntry(orgPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(false, false, true);

        _normalizer.Setup(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage))
            .Callback(new NormalizerCallBack((ref string path, out string? message) =>
            {
                message = null;
                Assert.AreEqual(inputEntryPath, path);
                path = normalizedPath;
            })).Returns(normalizerResult);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.AreEqual(AddDataEntryToBuilderState.FailedNormalization, result.Status);
        Assert.IsNull(result.AddedBuilderInfo);

        _normalizer.Verify(n => n.TryNormalizePath(ref It.Ref<string>.IsAny, out normalizerMessage), Times.Once);
    }

    [TestMethod]
    public void Test_AddEntry_AssureEncoding()
    {
        const string entryPath = "file.txt";
        const string overridePath = "newÄ.txt";
        const string expectedEntryPath = "new?.txt";

        var inputEntryPath = overridePath;

        string? normalizerMessage = null;

        var entry = MegDataEntryTest.CreateEntry(entryPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(false, false, true);

        _normalizer.Setup(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage))
            .Callback(new NormalizerCallBack((ref string path, out string? message) =>
            {
                message = null;
                // Assure that normalization is happening before encoding
                Assert.AreEqual(inputEntryPath, path);
            })).Returns(true);

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);
        Assert.IsTrue(result.Added);
        Assert.AreEqual(expectedEntryPath, result.AddedBuilderInfo.FilePath);

        _normalizer.Verify(n => n.TryNormalizePath(ref inputEntryPath, out normalizerMessage), Times.Once);
    }

    [TestMethod]
    [DataRow("file.txt", null)]
    [DataRow("file.txt", "new.txt")]
    public void Test_AddEntry_DoNotOverride(string entryPath, string? overridePath)
    {
        const string fileToAdd = "file1.txt";

        _fileSystem.AddEmptyFile(fileToAdd);

        var entry = MegDataEntryTest.CreateEntry(entryPath, default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(false, false, false);

        // Use AddFile here to assert that AddFile and AddEntry work when combined.
        builder.AddFile(fileToAdd, overridePath ?? entryPath);

        var resultSecondAdd = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.AreEqual(AddDataEntryToBuilderState.DuplicateEntry, resultSecondAdd.Status);
        Assert.AreEqual(1, builder.DataEntries.Count);
        Assert.IsNull(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.IsFalse(resultSecondAdd.WasOverwrite);
        Assert.AreEqual(_fileSystem.Path.GetFullPath(fileToAdd), builder.DataEntries.First().OriginInfo.FilePath);
    }

    [TestMethod]
    public void Test_AddEntry_DoOverride()
    {
        const string file = "file1.txt";
        const string inputEntryPath = "path/fileWithNonAsciiÖ.txt";

        const string otherEntryPath = "path/fileWithNonAsciiÄ.txt";

        const string expectedEncodedEntry = "path/fileWithNonAscii?.txt";

        _fileSystem.AddEmptyFile(file);

        var entry = MegDataEntryTest.CreateEntry("file.txt", default, default, false, null);
        var archive = new MegArchive(new List<MegDataEntry>
        {
            entry
        });
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive);

        var builder = CreateBuilder(true, false, false);

        // Use AddFile here to assert that AddFile and AddEntry work when combined.
        builder.AddFile(file, inputEntryPath);

        var resultSecondAdd = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), otherEntryPath);

        Assert.IsTrue(resultSecondAdd.Added);
        Assert.AreEqual(1, builder.DataEntries.Count);

        Assert.IsNotNull(resultSecondAdd.AddedBuilderInfo);
        Assert.IsNotNull(resultSecondAdd.OverwrittenBuilderInfo);
        Assert.IsTrue(resultSecondAdd.WasOverwrite);
        Assert.AreEqual(_fileSystem.Path.GetFullPath(file), resultSecondAdd.OverwrittenBuilderInfo.OriginInfo.FilePath);
        Assert.AreSame(entry, resultSecondAdd.AddedBuilderInfo.OriginInfo.MegFileLocation!.DataEntry);

        // Assert that duplicate check was based on encoded (thus also normalized) file path, cause the original inputs have different values.
        Assert.AreEqual(expectedEncodedEntry, resultSecondAdd.AddedBuilderInfo.FilePath);
    }

    [TestMethod]
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

        var builder = CreateBuilder(false, false, false);

        _entryValidator.Setup(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()))
            .Callback((MegFileDataEntryBuilderInfo builderInfo) =>
            {
                // Assert that the validator already has the encoded (and thus normalized) path.
                Assert.AreEqual(expectedEncodedEntry, builderInfo.FilePath);
            })
            .Returns(new ValidationResult(new List<ValidationFailure> { new("someError", "some error") }));

        var result = builder.AddEntry(new MegDataEntryLocationReference(meg.Object, entry), overridePath);

        Assert.AreEqual(AddDataEntryToBuilderState.InvalidEntry, result.Status);
        Assert.AreEqual(0, builder.DataEntries.Count);

        _entryValidator.Verify(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()), Times.Once);
    }

    #endregion

    #region Build

    delegate void MegFileServiceCreateCallBack(
        FileSystemStream fileStream, 
        MegFileVersion megVersion, 
        MegEncryptionData encryptionData, 
        IEnumerable<MegFileDataEntryBuilderInfo> entries);

    [TestMethod]
    public void Test_Build()
    {
        var fileInfo = new MegFileInformation("path/a.meg", MegFileVersion.V1);

        var builder = CreateBuilder(false, false, false);

        var expectedData = new byte[] { 1, 2, 3 };

        var tempFilePath = string.Empty;

        _megFileService.Setup(s => s.CreateMegArchive(It.IsAny<FileSystemStream>(), fileInfo.FileVersion,
                fileInfo.EncryptionData,
                It.IsAny<IEnumerable<MegFileDataEntryBuilderInfo>>()))
            .Callback(new MegFileServiceCreateCallBack((stream, _, _, _) =>
            {
                tempFilePath = stream.Name;
                using var ms = new MemoryStream(expectedData);
                ms.CopyTo(stream);
            }));

        builder.Build(fileInfo, false);

        CollectionAssert.AreEqual(expectedData, _fileSystem.File.ReadAllBytes(fileInfo.FilePath));
        Assert.AreNotEqual(string.Empty, tempFilePath);
        Assert.AreNotEqual(tempFilePath, fileInfo.FilePath);
        Assert.IsFalse(_fileSystem.FileExists(tempFilePath));
    }

    [TestMethod]
    public void Test_Build_WritingFails_Throws()
    {
        var fileInfo = new MegFileInformation("path/a.meg", MegFileVersion.V1);

        var builder = CreateBuilder(false, false, false);

        var expectedData = new byte[] { 1, 2, 3 };

        var tempFilePath = string.Empty;

        _megFileService.Setup(s => s.CreateMegArchive(It.IsAny<FileSystemStream>(), fileInfo.FileVersion,
                fileInfo.EncryptionData,
                It.IsAny<IEnumerable<MegFileDataEntryBuilderInfo>>()))
            .Callback(new MegFileServiceCreateCallBack((stream, _, _, _) =>
            {
                tempFilePath = stream.Name;
                using var ms = new MemoryStream(expectedData);
                ms.CopyTo(stream);
            }))
            .Throws<IOException>();

        Assert.ThrowsException<IOException>(() => builder.Build(fileInfo, false));

        Assert.IsFalse(_fileSystem.FileExists(tempFilePath));
    }

    [TestMethod]
    public void Test_Build_DoNotOverwrite_Throws()
    {
        var fileInfo = new MegFileInformation("a.meg", MegFileVersion.V1);

        _fileSystem.AddEmptyFile("a.meg");

        var builder = CreateBuilder(false, false, false);
        
        Assert.ThrowsException<IOException>(() => builder.Build(fileInfo, false));
    }

    //[TestMethod] TODO
    public void Test_CreateMegArchive_RealFileSystem_OverrideCurrentMeg()
    {
        var fileInfo = new MegFileInformation("a.meg", MegFileVersion.V1);

        var expectedData = new byte[] { 1, 2, 3 };

        // This Test does not use the MockFileSystem but the actual FileSystem
        var fs = new FileSystem();

        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(fs);
        sc.AddSingleton(_ => _megFileService.Object);

        // Default Validator always passes
        _infoValidator.Setup(v => v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()))
            .Returns(new ValidationResult());

        _megFileService.Setup(s => s.CreateMegArchive(It.IsAny<FileSystemStream>(), fileInfo.FileVersion,
                fileInfo.EncryptionData,
                It.IsAny<IEnumerable<MegFileDataEntryBuilderInfo>>()))
            .Callback(new MegFileServiceCreateCallBack((stream, _, _, _) =>
            {
                using var orgFs = fs.File.OpenRead("a.meg");
                orgFs.CopyTo(stream);
            }));

        var builder = new TestingMegBuilder(false, true, _normalizer.Object, _entryValidator.Object,
            _infoValidator.Object, sc.BuildServiceProvider());

        try
        {
            fs.File.WriteAllBytes(fileInfo.FilePath, [1, 2, 3]);
            builder.Build(fileInfo, true);

            var actualBytes = fs.File.ReadAllBytes(fileInfo.FilePath);

            CollectionAssert.AreEqual(expectedData, actualBytes);
        }
        finally
        {
            try
            {
                fs.File.Delete(fileInfo.FilePath);
            }
            catch
            {
                // Ignore
            }
        }
    }

    [TestMethod]
    public void Test_Build_InvalidInfo_Throws()
    {
        var fileInfo = new MegFileInformation("a.meg", MegFileVersion.V1);

        var builder = CreateBuilder(false, false, false);

        _infoValidator.Setup(v => v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()))
            .Returns(new ValidationResult(new List<ValidationFailure> { new("some-error", "some error") }));

        Assert.ThrowsException<NotSupportedException>(() => builder.Build(fileInfo, false));

        _infoValidator.Verify(v => 
            v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()), Times.Once);
    }

    [TestMethod]
    [DataRow("./")]
    [DataRow("./..")]
    [DataRow("path/")]
    [DataRow("/")]
    public void Test_Build_InvalidInfoPath_Throws(string path)
    {
        var fileInfo = new MegFileInformation(path, MegFileVersion.V1);

        var builder = CreateBuilder(false, false, false);

        Assert.ThrowsException<ArgumentException>(() => builder.Build(fileInfo, false));
    }

    #endregion

    private MegBuilderBase CreateBuilder(bool overwrite, bool addFileSize, bool useNormalizer)
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton(_ => _megFileService.Object);

        // Default Validator always passes
        _entryValidator.Setup(v => v.Validate(It.IsAny<MegFileDataEntryBuilderInfo>()))
            .Returns(new ValidationResult());

        // Default Validator always passes
        _infoValidator.Setup(v => v.Validate(It.IsAny<MegBuilderFileInformationValidationData>()))
            .Returns(new ValidationResult());

        return new TestingMegBuilder(
            overwrite,
            addFileSize,
            useNormalizer ? _normalizer.Object : null,
            _entryValidator.Object,
            _infoValidator.Object,
            sc.BuildServiceProvider());
    }

    private class TestingMegBuilder(
        bool overwrite,
        bool addFileSize,
        IMegDataEntryPathNormalizer? normalizer,
        IBuilderInfoValidator entryValidator,
        IMegFileInformationValidator megFileInformationValidator,
        IServiceProvider services)
        : MegBuilderBase(services)
    {
        public override bool OverwritesDuplicateEntries { get; } = overwrite;

        public override bool AutomaticallyAddFileSizes { get; } = addFileSize;

        public override IMegDataEntryPathNormalizer? DataEntryPathNormalizer { get; } = normalizer;

        public override IBuilderInfoValidator DataEntryValidator { get; } = entryValidator;

        public override IMegFileInformationValidator MegFileInformationValidator { get; } = megFileInformationValidator;
    }
}