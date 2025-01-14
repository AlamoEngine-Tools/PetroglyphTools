// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.Extensions;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a MEG data entry whether it is compliant to a Petroglyph Star Wars game. 
/// </summary>
public sealed class EmpireAtWarMegBuilderDataEntryValidator : PetroglyphMegBuilderDataEntryValidator
{
    /// <summary>
    /// Returns a singleton instance of the <see cref="EmpireAtWarMegBuilderDataEntryValidator"/>.
    /// </summary>
    public static readonly EmpireAtWarMegBuilderDataEntryValidator Instance = new();

    // Slashes are not allowed, cause the engine normalized them into back-slashes.
    // Whitespaces (space, tab, new line) *technically* are allowed but there are scenarios where file names are separated by spaces in XML code.
    // Since there is no space escaping implemented in the engine, file lookup would break at this point.
    // Thus, this validator is a little more sensitive.
    private static readonly char[] ForbiddenChars = ['/', ' ', '\0', '\t', '\r', '\n'];

    private EmpireAtWarMegBuilderDataEntryValidator()
    {
    }

    /// <inheritdoc />
    public override bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size)
    {
        if (encrypted)
            return false;

        if (entryPath.Length is 0 or > MegFileConstants.EawMaxEntryPathLength)
            return false;

        if (IsRootedOrStartsWithCurrent(entryPath))
            return false;

        if (entryPath.IndexOfAny(ForbiddenChars) != -1)
            return false;

        // If the path contains ':' the first one must also be the first character, iff we have a slashes present
        if (entryPath.IndexOf(':') > 0 && entryPath.IndexOf('\\') != -1)
            return false;

        Span<char> pathBuffer = stackalloc char[MegFileConstants.EawMaxEntryPathLength];
       
        var upperLength = entryPath.ToUpperInvariant(pathBuffer);
        var upper = pathBuffer.Slice(0, upperLength);

        if (upperLength != entryPath.Length || !entryPath.Equals(upper, StringComparison.Ordinal))
            return false;

        var megEncoding = MegFileConstants.MegDataEntryPathEncoding;
        var asciiLength = megEncoding.EncodeString(entryPath, pathBuffer, megEncoding.GetByteCountPG(entryPath.Length));
        var asAscii = pathBuffer.Slice(0, asciiLength);

        if (asciiLength != entryPath.Length || !entryPath.Equals(asAscii, StringComparison.Ordinal))
            return false;

        return true;
    }

    private static bool IsRootedOrStartsWithCurrent(ReadOnlySpan<char> path)
    {
        // This check is over-sensitive as @"\\" may be a valid path which can be produced by normalization, 
        // however, such a path does not make much sense. 
        if (path[0] == '\\')
            return true;
        
        // Drive rooted paths are handled elsewhere. So this is not included.

        if (path.Length < 2)
            return false;

        if (path[0] is '.' && path[1] is '\\')
            return true;

        return false;
    }
}