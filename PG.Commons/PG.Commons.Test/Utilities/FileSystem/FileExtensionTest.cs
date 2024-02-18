using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities.FileSystem;
using Testably.Abstractions.Testing;

namespace PG.Commons.Test.Utilities.FileSystem;

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

    #region ExecuteFileActionWithRetry

    [TestMethod]
    public void Test_ExecuteFileActionWithRetry_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => FileExtensions.ExecuteFileActionWithRetry(-1, 0, () => { }));
        Assert.ThrowsException<ArgumentNullException>(() => FileExtensions.ExecuteFileActionWithRetry(0, 0, null!));
        Assert.ThrowsException<IOException>(() => FileExtensions.ExecuteFileActionWithRetry(0, 0, () => throw new IOException()));
        Assert.ThrowsException<UnauthorizedAccessException>(() => FileExtensions.ExecuteFileActionWithRetry(0, 0, () => throw new UnauthorizedAccessException()));
        Assert.ThrowsException<Exception>(() => FileExtensions.ExecuteFileActionWithRetry(0, 0, () => throw new Exception()));
        Assert.ThrowsException<Exception>(() => FileExtensions.ExecuteFileActionWithRetry(0, 0, () => throw new Exception(), false));
    }

    [TestMethod]
    public void Test_ExecuteFileActionWithRetry_ErrorAction()
    {
        var errorAction = 0;
        Assert.IsFalse(FileExtensions.ExecuteFileActionWithRetry(2, 0, () => throw new IOException(), false,
            (_, _) =>
            {
                errorAction++;
                return false;
            }));

        Assert.AreEqual(3, errorAction);

        Assert.IsFalse(FileExtensions.ExecuteFileActionWithRetry(2, 0, () => throw new IOException(), false,
            (_, _) =>
            {
                errorAction++;
                return true;
            }));

        Assert.AreEqual(6, errorAction);
    }

    [TestMethod]
    public void Test_ExecuteFileActionWithRetry()
    {
        var actionRunCount = 0;
        Assert.IsTrue(FileExtensions.ExecuteFileActionWithRetry(2, 0, () =>
        {
            actionRunCount++;
        }));
        Assert.AreEqual(1, actionRunCount);
    }

    [TestMethod]
    public void Test_ExecuteFileActionWithRetry_Retry()
    {
        var actionRunCount = 0;
        var fail = true;
        Assert.IsTrue(FileExtensions.ExecuteFileActionWithRetry(2, 0, () =>
        {
            actionRunCount++;
            if (fail)
            {
                fail = false;
                throw new IOException();
            }
        }));
        Assert.AreEqual(2, actionRunCount);
    }

    #endregion
}