using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using AnakinRaW.CommonUtilities.FileSystem;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

internal sealed class PetroglyphRelativeDataEntryPathResolver(IServiceProvider serviceProvider) : IDataEntryPathResolver
{
    private readonly IFileSystem _fileSystem = serviceProvider.GetRequiredService<IFileSystem>();

    public string? ResolvePath(string? path, string basePath)
    {
        var fullBase = PathNormalizer.Normalize(_fileSystem.Path.GetFullPath(basePath), PathNormalizeOptions.EnsureTrailingSeparator);

        var pathSpan = path.AsSpan();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            pathSpan = PrepareForPossibleDriveRelativePath(pathSpan, fullBase.AsSpan());

        // Needs to be after relative drive preparation (e.g, if path was just "C:")
        if (pathSpan.Length == 0)
            return null;

        if (_fileSystem.Path.HasTrailingDirectorySeparator(pathSpan!))
            return null;

        var relativePath = _fileSystem.Path.GetRelativePathEx(fullBase, pathSpan.ToString());

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


    private ReadOnlySpan<char> PrepareForPossibleDriveRelativePath(ReadOnlySpan<char> path, ReadOnlySpan<char> rootPath)
    {
        if (path.Length == 0)
            return ReadOnlySpan<char>.Empty;
        if (_fileSystem.Path.IsDriveRelative(path, out var driveLetter))
        {
            var rootPathDrive = GetVolumeNameFromFullyQualifiedPath(_fileSystem, rootPath);

            if (rootPathDrive.HasValue && char.ToUpperInvariant(rootPathDrive.Value) == char.ToUpperInvariant(driveLetter.Value))
                path = path.Slice(2);
        }
        return path;
    }

    // By design this method does not correctly handle stuff like Device paths (e.g, //./C:/)
    private static char? GetVolumeNameFromFullyQualifiedPath(IFileSystem fileSystem, ReadOnlySpan<char> fullyQualifiedPath)
    {
        var root = fileSystem.Path.GetPathRoot(fullyQualifiedPath);

        if (root.Length < 3)
            return null;

        // If the second char is a Volume separator ':' it cannot be a device or UNC path. 
        if (root[1] != ':')
            return null;

        // GetPathRoot already ensure we don't have something wierd like ?:/ or ö:/ which are not legal roots in Windows.
        return root[0];
    }
}