using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PG.StarWarsGame.Files.MEG.Services.FileSystem;

/// <summary>
/// Provides extensions for file handling.
/// </summary>
internal static class FileExtensions
{
    /// <summary>
    /// Creates a temporary, hidden file which gets deleted once the returned stream is disposed.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="directory">The directory where the temporary file shall be created.
    /// If the directory is <see langword="null"/> the current user's temporary directory will be used.</param>
    /// <returns>A temporary file which gets deleted on disposal.</returns>
    public static FileSystemStream CreateRandomHiddenTemporaryFile(this IFile _, string? directory = null)
    {
        var fs = _.FileSystem;

        directory = directory is null ? fs.Path.GetTempPath() : fs.Path.GetFullPath(directory);

        if (!fs.Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Could not find the target directory '{directory}'");

        FileSystemStream stream = null!;
        ExecuteFileActionWithRetry(3, 500, () =>
        {
            var randomName = fs.Path.GetRandomFileName();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                randomName  = "." + randomName;

            var tempFilePath = fs.Path.GetFullPath(fs.Path.Combine(directory, randomName));
            stream = fs.FileStream.New(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.DeleteOnClose);
            fs.File.SetAttributes(tempFilePath, FileAttributes.Temporary | FileAttributes.Hidden);
        });

        return stream;
    }

    internal static bool ExecuteFileActionWithRetry(
        int retryCount,
        int retryDelay,
        Action fileAction, 
        bool throwOnFailure = true, 
        Func<Exception, int, bool>? errorAction = null)
    {
        if (fileAction == null) 
            throw new ArgumentNullException(nameof(fileAction));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));

        var num = retryCount + 1;
        for (var index = 0; index < num; ++index)
        {
            try
            {
                fileAction();
                return true;
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                if (!throwOnFailure || index + 1 < num)
                {
                    if (errorAction != null)
                    {
                        if (!errorAction(ex, index))
                        {
                            if (index + 1 >= num)
                                continue;
                        }
                        else
                            continue;
                    }

                    Task.Delay(retryDelay).Wait();
                }
                else
                    throw;
            }
        }
        return false;
    }
}