using System.IO;
using System.IO.Abstractions;

namespace PG.Commons.Utilities.FileSystem;

/// <summary>
/// 
/// </summary>
public static class PathExtensions
{
    private static char DirectorySeparatorChar => Path.DirectorySeparatorChar;

    /// <summary>
    /// True if the character is any recognized directory separator character.
    /// </summary>
    private static bool IsAnyDirectorySeparator(char c)
    {
        return c is '\\' or '/';
    }

    /// <summary>
    /// Ensures a trailing directory separator character
    /// </summary>
    public static string EnsureTrailingSeparator(this IPath _, string s)
    {
        if (s.Length == 0 || IsAnyDirectorySeparator(s[s.Length - 1]))
            return s;

        // Use the existing slashes in the path, if they're consistent
        var hasSlash = s.IndexOf('/') >= 0;
        var hasBackslash = s.IndexOf('\\') >= 0;

        if (hasSlash && !hasBackslash)
            return s + '/';

        if (!hasSlash && hasBackslash)
            return s + '\\';

        // If there are no slashes or they are inconsistent, use the current platform's slash.
        return s + DirectorySeparatorChar;
    }
}