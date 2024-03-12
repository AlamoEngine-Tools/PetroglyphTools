using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
            var rootPathDrive = new WindowsPathHelper(_fileSystem).GetVolumeNameFromFullyQualifiedPath(rootPath);

            if (rootPathDrive.HasValue && char.ToUpperInvariant(rootPathDrive.Value) == char.ToUpperInvariant(driveLetter.Value))
                path = path.Substring(2);
        }
        return path;
    }


    // Shamelessly copied from .NET Runtime
    private readonly ref struct WindowsPathHelper(IFileSystem fileSystem)
    {
        private const int DevicePrefixLength = 4;


        public char? GetVolumeNameFromFullyQualifiedPath(ReadOnlySpan<char> fullyQualifiedPath)
        {

#if NETSTANDARD2_0
            var root = fileSystem.Path.GetPathRoot(fullyQualifiedPath.ToString()).AsSpan();
#else
            var root = fileSystem.Path.GetPathRoot(fullyQualifiedPath);
#endif

            if (root.Length == 0)
                return null;

            var isDevice = IsDevice(fullyQualifiedPath);

            if (IsUncPath(fullyQualifiedPath, isDevice))
                return null;

            var drivePartOffset = 0;

            if (isDevice)
                drivePartOffset = 4; // e.g, "\\?\C:\" to "C:\"

            var driveOnlyPath = root.Slice(drivePartOffset);
            return driveOnlyPath[0];
        }

        private bool IsUncPath(ReadOnlySpan<char> path, bool isDevice)
        {
            switch (isDevice)
            {
                case false when path.Slice(0, 2).Equals(@"\\".AsSpan(), StringComparison.Ordinal):
                case true when path.Length >= 8
                               && (path.Slice(0, 8).Equals(@"\\?\UNC\".AsSpan(), StringComparison.Ordinal)
                                   || path.Slice(5, 4).Equals(@"UNC\".AsSpan(), StringComparison.Ordinal)):
                    return true;
                default:
                    return false;
            }
        }

        private bool IsDevice(ReadOnlySpan<char> path)
        {
            // If the path begins with any two separators is will be recognized and normalized and prepped with
            // "\??\" for internal usage correctly. "\??\" is recognized and handled, "/??/" is not.
            return IsExtended(path)
                   ||
                   (
                       path.Length >= DevicePrefixLength
                       && IsDirectorySeparator(path[0])
                       && IsDirectorySeparator(path[1])
                       && (path[2] == '.' || path[2] == '?')
                       && IsDirectorySeparator(path[3])
                   );
        }

        private bool IsExtended(ReadOnlySpan<char> path)
        {
            // While paths like "//?/C:/" will work, they're treated the same as "\\.\" paths.
            // Skipping of normalization will *only* occur if back slashes ('\') are used.
            return path.Length >= DevicePrefixLength
                   && path[0] == '\\'
                   && (path[1] == '\\' || path[1] == '?')
                   && path[2] == '?'
                   && path[3] == '\\';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDirectorySeparator(char c)
        {
            return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
        }
    }
}