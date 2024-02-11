using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PG.Commons.Utilities.FileSystem;

// Based on https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/FileSystem/PathUtilities.cs
// and https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Common/PathUtil/PathUtility.cs
// and https://github.com/dotnet/runtime
/// <summary>
/// Provides extensions for path manipulation.
/// </summary>
public static class PathExtensions
{
    private const string ThisDirectory = ".";
    private const string ParentRelativeDirectory = "..";
    private const char VolumeSeparatorChar = ':';

    private static readonly char DirectorySeparatorChar = Path.DirectorySeparatorChar;
    private static readonly char AltDirectorySeparatorChar = Path.AltDirectorySeparatorChar;
    private static readonly string DirectorySeparatorStr = new(DirectorySeparatorChar, 1);

    private static readonly char[] PathChars =
    [
        VolumeSeparatorChar, DirectorySeparatorChar, AltDirectorySeparatorChar
    ];

    private static readonly bool IsUnixLikePlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    private static readonly Lazy<bool> IsFileSystemCaseInsensitive = new(CheckIfFileSystemIsCaseInsensitive);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyDirectorySeparatorWindows(char c)
    {
        return c is '\\' or '/';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyDirectorySeparatorLinux(char c)
    {
        return c is '/';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyDirectorySeparator(char c, DirectorySeparatorKind separatorKind)
    {
        switch (separatorKind)
        {
            case DirectorySeparatorKind.System:
                return IsAnyDirectorySeparator(c);
            case DirectorySeparatorKind.Linux:
                return IsAnyDirectorySeparatorLinux(c);
            case DirectorySeparatorKind.Windows:
                return IsAnyDirectorySeparatorWindows(c);
            default:
                throw new ArgumentOutOfRangeException(nameof(separatorKind), separatorKind, null);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyDirectorySeparator(char c)
    {
        return c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;
    }

    /// <summary>
    /// Ensures a trailing root separator character
    /// </summary>
    /// <param name="_"></param>
    /// <param name="input">The path to append a path separator to, if required.</param>
    public static string EnsureTrailingSeparator(this IPath _, string input)
    {
        return EnsureTrailingSeparatorInternal(input);
    }

    private static string EnsureTrailingSeparatorInternal(string input)
    {
        if (input.Length == 0 || IsAnyDirectorySeparator(input[input.Length - 1]))
            return input;

        // Use the existing slashes in the path, if they're consistent
        var hasPrimarySlash = input.IndexOf(DirectorySeparatorChar) >= 0;
        var hasAlternateSlash = input.IndexOf(AltDirectorySeparatorChar) >= 0;

        if (hasPrimarySlash && !hasAlternateSlash)
            return input + DirectorySeparatorChar;

        if (!hasPrimarySlash && hasAlternateSlash)
            return input + AltDirectorySeparatorChar;

        // If there are no slashes, or they are inconsistent, use the current platform's primary slash.
        return input + DirectorySeparatorChar;
    }


    /// <summary>
    /// Checks whether a given path ends with a path separator
    /// </summary>
    /// <param name="_"></param>
    /// <param name="path">The path to check.</param>
    /// <returns><see langowrd="true"/> if <paramref name="path"/> end with a path separator; otherwise, <see langowrd="false"/>.</returns>
    public static bool HasTrailingPathSeparator(this IPath _, string path)
    {
        if (path == null) 
            throw new ArgumentNullException(nameof(path));
        return HasTrailingPathSeparatorInternal(path.AsSpan());
    }

    /// <summary>
    /// Checks whether a given character span ends with a path separator
    /// </summary>
    /// <param name="_"></param>
    /// <param name="value">The character span to check.</param>
    /// <returns><see langowrd="true"/> if <paramref name="value"/> end with a path separator; otherwise, <see langowrd="false"/>.</returns>
    public static bool HasTrailingPathSeparator(this IPath _, ReadOnlySpan<char> value)
    {
        return HasTrailingPathSeparatorInternal(value);
    }

    private static bool HasTrailingPathSeparatorInternal(ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            return false;
        var last = value[value.Length - 1];
        return IsAnyDirectorySeparator(last);
    }


    /// <summary>
    /// Returns a relative path from a path to given root
    /// or <paramref name="path"/> if <paramref name="path"/> is not rooted.
    /// </summary>
    /// <param name="fsPath">The file system's path instance.</param>
    /// <param name="root">The root path the result should be relative to. This path is always considered to be a directory.</param>
    /// <param name="path">The destination path.</param>
    /// <returns>The relative path, or path if the paths don't share the same root.</returns>
    /// <remarks>This method behaves differently to .NET Core <c>Path.GetRelativePath(string, string)</c>
    /// if <paramref name="path"/> is not rooted.
    /// </remarks>
    public static string GetRelativePathEx(this IPath fsPath, string root, string path)
    {
        var endsWithTrailingPathSeparator = HasTrailingPathSeparatorInternal(path.AsSpan());

        if (!fsPath.IsPathRooted(path))
            return path;

        // Root should always be absolute
        root = fsPath.GetFullPath(root);
        root = TrimTrailingSeparators(root.AsSpan());

        path = fsPath.GetFullPath(path);
        var trimmedPath = TrimTrailingSeparators(path.AsSpan());

        var rootParts = GetPathParts(root);
        var pathParts = GetPathParts(trimmedPath);

        if (rootParts.Length == 0 || pathParts.Length == 0)
            return path;

        var index = 0;

        // find index where full path diverges from base path
        var maxSearchIndex = Math.Min(rootParts.Length, pathParts.Length);
        for (; index < maxSearchIndex; index++)
        {
            if (!PathsEqual(rootParts[index], pathParts[index]))
                break;
        }

        // if the first part doesn't match, they don't even have the same volume
        // so there can be no relative path.
        if (index == 0)
            return path;

        var relativePath = string.Empty;

        // add backup notation for remaining base path levels beyond the index
        var remainingParts = rootParts.Length - index;
        if (remainingParts > 0)
        {
            for (var i = 0; i < remainingParts; i++) 
                relativePath = relativePath + ParentRelativeDirectory + DirectorySeparatorStr;
        }
        
        if (index < pathParts.Length)
        {
            // add the rest of the full path parts
            for (var i = index; i < pathParts.Length; i++)
                relativePath = CombinePathsUnchecked(relativePath, pathParts[i]);

            if (endsWithTrailingPathSeparator)
                relativePath = EnsureTrailingSeparatorInternal(relativePath);
        }
        else
        {
            if (!string.IsNullOrEmpty(relativePath))
                relativePath = TrimTrailingSeparators(relativePath.AsSpan());
        }


        if (relativePath == string.Empty)
            return ThisDirectory;
        return relativePath;
    }

    private static string[] GetPathParts(string path)
    {
        var pathParts = path.Split(PathChars);

        // remove references to self directories ('.')
        if (pathParts.Contains(ThisDirectory))
            pathParts = pathParts.Where(s => s != ThisDirectory).ToArray();

        return pathParts;
    }

    private static bool PathsEqual(string path1, string path2)
    {
        return PathsEqual(path1, path2, Math.Max(path1.Length, path2.Length));
    }

    /// <summary>
    /// True if the two paths are the same.  (but only up to the specified length)
    /// </summary>
    private static bool PathsEqual(string path1, string path2, int length)
    {
        if (path1.Length < length || path2.Length < length)
            return false;

        for (var i = 0; i < length; i++)
        {
            if (!PathCharEqual(path1[i], path2[i]))
                return false;
        }

        return true;
    }

    private static bool PathCharEqual(char x, char y)
    {
        if (IsAnyDirectorySeparator(x) && IsAnyDirectorySeparator(y))
            return true;

        return IsUnixLikePlatform
            ? x == y
            : char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
    }

    /// <summary>
    /// Checks whether a path is absolute to a drive e.g, "C:/" or "C:\my\path"
    /// </summary>
    /// <remarks>
    /// Only works on Windows. For Linux systems, this method will always return <see langword="false"/>.
    /// </remarks>
    /// <param name="fsPath">The file system's path instance.</param>
    /// <param name="path">The path to check.</param>
    /// <returns>Return <see langword="true"/> if <paramref name="path"/> is absolute to a drive; otherwise, <see langword="false"/>.</returns>
    public static bool IsDriveAbsolute(this IPath fsPath, string path)
    {
        // Implementation based on Path.Windows.cs from the .NET repository
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;

        if (!fsPath.IsPathRooted(path))
            return false;

        if (path.Length < 3)
            return false;

        if (IsValidDriveChar(path[0]) && path[1] == VolumeSeparatorChar)
            return IsAnyDirectorySeparator(path[2]);

        return false;
    }

    /// <summary>
    /// Checks whether a path is rooted, but not absolute to a drive e.g, "C:" or "C:my/path"
    /// </summary>
    /// <remarks>
    /// Only works on Windows. For Linux systems, this method will always return <see langword="false"/>.
    /// </remarks>
    /// <param name="fsPath">The file system's path instance.</param>
    /// <param name="path">The path to check.</param>
    /// <returns>Return <see langword="true"/> if <paramref name="path"/> is relative, but not absolute to a drive; otherwise, <see langword="false"/>.</returns>
    public static bool IsDriveRelative(this IPath fsPath, string path)
    {
        // Implementation based on Path.Windows.cs from the .NET repository
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;

        if (!fsPath.IsPathRooted(path))
            return false;

        if (path.Length < 2)
            return false;

        if (IsValidDriveChar(path[0]) && path[1] == VolumeSeparatorChar)
            return path.Length < 3 || !IsAnyDirectorySeparator(path[2]);

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsValidDriveChar(char value)
    {
        return (uint)((value | 0x20) - 'a') <= 'z' - 'a';
    }


    /// <summary>
    /// Normalizes a given path according to given normalization rules.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="path">The input path.</param>
    /// <param name="options">The options how to normalize.</param>
    /// <returns>The normalized path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    /// <exception cref="IOException">The normalization failed due to an internal error.</exception>
    public static string Normalize(this IPath _, string path, PathNormalizeOptions options)
    {
        ThrowHelper.ThrowIfNullOrEmpty(path);

        // Only do for DirectorySeparatorKind.System, cause for other kinds it will be done at the very end anyway.
        if (options.UnifySlashes && options.SeparatorKind == DirectorySeparatorKind.System)
            path = GetPathWithDirectorySeparator(path, DirectorySeparatorKind.System);

        if (options.TrimTrailingSeparator)
            path = TrimTrailingSeparators(path.AsSpan(), options.SeparatorKind);

        path = NormalizeCasing(path, options.UnifyCase);

        // NB: As previous steps may add new separators (such as GetFullPath) we need to re-apply slash normalization
        // if the desired DirectorySeparatorKind is not DirectorySeparatorKind.System
        if (options.UnifySlashes && options.SeparatorKind != DirectorySeparatorKind.System) 
            path = GetPathWithDirectorySeparator(path, options.SeparatorKind);

        return path;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string NormalizeCasing(string path, UnifyCasingKind casing)
    {
        if (casing  == UnifyCasingKind.None)
            return path;

        if (!IsFileSystemCaseInsensitive.Value && !casing.IsForce())
            return path;

        if (casing is UnifyCasingKind.LowerCaseForce or UnifyCasingKind.LowerCase)
            return path.ToLowerInvariant();

        if (casing is UnifyCasingKind.UpperCase or UnifyCasingKind.UpperCaseForce)
            return path.ToUpperInvariant();

        throw new ArgumentOutOfRangeException(nameof(casing));
    }

    private static string TrimTrailingSeparators(ReadOnlySpan<char> s, DirectorySeparatorKind separatorKind = DirectorySeparatorKind.System) 
    {
        var lastSeparator = s.Length;
        while (lastSeparator > 0 && IsAnyDirectorySeparator(s[lastSeparator - 1], separatorKind))
            lastSeparator -= 1;
        if (lastSeparator != s.Length)
            s = s.Slice(0, lastSeparator);
        return s.ToString();
    }

    private static string GetPathWithDirectorySeparator(string path, DirectorySeparatorKind separatorKind)
    {
        switch (separatorKind)
        {
            case DirectorySeparatorKind.System:
                return IsUnixLikePlatform ? GetPathWithForwardSlashes(path) : GetPathWithBackSlashes(path);
            case DirectorySeparatorKind.Windows:
                return GetPathWithBackSlashes(path);
            case DirectorySeparatorKind.Linux:
                return GetPathWithForwardSlashes(path);
            default:
                throw new ArgumentOutOfRangeException(nameof(separatorKind));
        }
    }


    private static string CombinePathsUnchecked(string root, string relativePath)
    {
        if (root == string.Empty)
            return relativePath;
        var c = root[root.Length - 1];
        if (!IsAnyDirectorySeparator(c) && c != VolumeSeparatorChar)
        {
            return root + DirectorySeparatorStr + relativePath;
        }

        return root + relativePath;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetPathWithBackSlashes(string path)
    {
        return path.Replace('/', '\\');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetPathWithForwardSlashes(string path)
    {
        return path.Replace('\\', '/');
    }

    private static bool CheckIfFileSystemIsCaseInsensitive()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return true;
        var listOfPathsToCheck = new[]
        {
            Path.GetTempPath(),
            Directory.GetCurrentDirectory()
        };

        var isCaseInsensitive = true;
        foreach (var path in listOfPathsToCheck)
        {
            var result = CheckCaseSensitivityRecursivelyTillDirectoryExists(path, out var ignore);
            if (!ignore)
                isCaseInsensitive &= result;
        }
        return isCaseInsensitive;
    }

    private static bool CheckCaseSensitivityRecursivelyTillDirectoryExists(string path, out bool ignoreResult)
    {
        path = Path.GetFullPath(path);
        ignoreResult = true;
        var parentDirectoryFound = true;
        while (true)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (path.Length <= 1)
            {
                ignoreResult = true;
                parentDirectoryFound = false;
                break;
            }
            if (Directory.Exists(path))
            {
                ignoreResult = false;
                break;
            }
            path = Path.GetDirectoryName(path)!;
        }

        if (parentDirectoryFound)
        {
            return Directory.Exists(path.ToLowerInvariant()) && Directory.Exists(path.ToUpperInvariant());
        }
        return false;
    }

    private static bool IsForce(this UnifyCasingKind casing)
    {
        if (casing is UnifyCasingKind.LowerCaseForce or UnifyCasingKind.UpperCaseForce)
            return true;
        return false;
    }
}