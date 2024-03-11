using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.FileSystem;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.FileSystem;

[TestClass]
public class FileExtensionTest
{
    private readonly MockFileSystem _fileSystem = new();

    [TestMethod]
    public void Test_CreateRandomHiddenTemporaryFile()
    {
        _fileSystem.Initialize();

        var tempStream = _fileSystem.File.CreateRandomHiddenTemporaryFile();
        var filePath = tempStream.Name;

        // Check file exists
        Assert.IsTrue(_fileSystem.File.Exists(filePath));
        Assert.IsTrue(_fileSystem.File.GetAttributes(filePath).HasFlag(FileAttributes.Hidden));

        // Check file read/write
        tempStream.Write([1,2,3], 0, 3);
        tempStream.Seek(0, SeekOrigin.Begin);

        var resultBuffer = new byte[3];
        _ = tempStream.Read(resultBuffer, 0, 3);
        CollectionAssert.AreEqual(new byte[]{1,2,3}, resultBuffer);

        // Check deleted
        tempStream.Dispose();
        Assert.IsFalse(_fileSystem.File.Exists(filePath));
    }

    [TestMethod]
    public void Test_CreateRandomHiddenTemporaryFile_FromDirectory()
    {
        _fileSystem.Initialize().WithSubdirectory(_fileSystem.Path.GetFullPath("test"));

        var tempStream = _fileSystem.File.CreateRandomHiddenTemporaryFile(_fileSystem.Path.GetFullPath("test"));
        var filePath = tempStream.Name;

        Assert.IsTrue(filePath.StartsWith(_fileSystem.Path.GetFullPath("test")));

        // Check file exists
        Assert.IsTrue(_fileSystem.File.Exists(filePath));
        Assert.IsTrue(_fileSystem.File.GetAttributes(filePath).HasFlag(FileAttributes.Hidden));

        // Check file read/write
        tempStream.Write([1, 2, 3], 0, 3);
        tempStream.Seek(0, SeekOrigin.Begin);

        var resultBuffer = new byte[3];
        _ = tempStream.Read(resultBuffer, 0, 3);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, resultBuffer);

        // Check deleted
        tempStream.Dispose();
        Assert.IsFalse(_fileSystem.File.Exists(filePath));
    }

    [TestMethod]
    public void Test_CreateRandomHiddenTemporaryFile_DirectoryNotExist()
    {
        Assert.ThrowsException<DirectoryNotFoundException>(() => _fileSystem.File.CreateRandomHiddenTemporaryFile());
        Assert.ThrowsException<DirectoryNotFoundException>(() => _fileSystem.File.CreateRandomHiddenTemporaryFile(@"C:\Test\"));
    }
}