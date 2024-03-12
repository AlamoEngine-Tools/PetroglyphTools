using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.FileSystem;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

internal sealed class PetroglyphRelativeDataEntryPathResolver(IServiceProvider serviceProvider) : IDataEntryPathResolver
{
    private readonly IFileSystem _fileSystem = serviceProvider.GetRequiredService<IFileSystem>();

    public string? ResolvePath(string path, string basePath)
    {
        var fullBase = PathNormalizer.Normalize(_fileSystem.Path.GetFullPath(basePath), PathNormalizeOptions.EnsureTrailingSeparator);

        path = PrepareForPossibleDriveRelativePath(path, fullBase.AsSpan());

        // Needs to be after relative drive preparation (e.g, if path was just "C:")
        if (string.IsNullOrEmpty(path))
            return null;

        if (_fileSystem.Path.HasTrailingDirectorySeparator(path))
            return null;

        var relativePath = _fileSystem.Path.GetRelativePathEx(fullBase, path);

        var fullRelativePathToBase = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(fullBase, relativePath));

        if (fullRelativePathToBase.StartsWith(fullBase))
        {
            // path is effectively empty on the current file system
            if (fullRelativePathToBase.Length == fullBase.Length)
                return null;

            return fullRelativePathToBase.Substring(fullBase.Length);
        }

        return null;
    }

    private string PrepareForPossibleDriveRelativePath(string path, ReadOnlySpan<char> rootPath)
    {
        if (_fileSystem.Path.IsDriveRelative(path, out var driveLetter))
        {
            // Both roots are the same, now cut away the drive relative part and just take the relative path.
            if (DriveRootsAreEqual(driveLetter!.Value, rootPath))
            {
                // drive relative paths, always have 2 chars
                path = path.Substring(2);
            }
        }
        return path;

        bool DriveRootsAreEqual(char driveLetter, ReadOnlySpan<char> fullPath)
        {
            // This method by design is not feature complete.
            // Paths such as ("\\?\Server\Share", "\\?\C:\" or \\Server\Share) will produce false results
            // We don't expect these paths for our library as their complexity is just not worth the effort. 
            if (!_fileSystem.Path.IsPathFullyQualified(fullPath))
                return false;


            var fullPathDrive = fullPath.Slice(0, 1);

            return fullPathDrive.CompareTo(driveLetter, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}