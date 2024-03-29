using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using AnakinRaW.CommonUtilities.FileSystem;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides extensions for file handling.
/// </summary>
public static class FileExtensions
{
    /// <summary>
    /// Creates a temporary, hidden file which gets deleted once the returned stream is disposed.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="directory">The directory where the temporary file shall be created.
    /// If <paramref name="directory"/> is <see langword="null"/> the current user's temporary directory will be used.</param>
    /// <returns>A temporary file which gets deleted on disposal.</returns>
    public static FileSystemStream CreateRandomHiddenTemporaryFile(this IFile _, string? directory = null)
    {
        var fs = _.FileSystem;

        directory = directory is null ? fs.Path.GetTempPath() : fs.Path.GetFullPath(directory);

        if (!fs.Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Could not find the target directory '{directory}'");

        FileSystemStream stream = null!;
        FileSystemUtilities.ExecuteFileSystemActionWithRetry(3, 500, () =>
        {
            var randomName = fs.Path.GetRandomFileName();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                randomName = "." + randomName;

            var tempFilePath = fs.Path.GetFullPath(fs.Path.Combine(directory, randomName));
            stream = fs.FileStream.New(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.DeleteOnClose);
            fs.File.SetAttributes(tempFilePath, FileAttributes.Temporary | FileAttributes.Hidden);
        });
        return stream;
    }
}