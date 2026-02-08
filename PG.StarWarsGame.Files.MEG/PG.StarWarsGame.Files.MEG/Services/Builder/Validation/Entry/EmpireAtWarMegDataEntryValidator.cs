// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.Extensions;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents a validator for MEG data entries used when building MEG files for Petroglyph Star Wars games.
/// </summary>
public sealed class EmpireAtWarMegDataEntryValidator : BinaryMegDataEntryValidator
{
    // Slashes are not allowed, cause the engine normalized them into back-slashes.
    // Whitespaces (space, tab, new line) *technically* are allowed but there are scenarios where
    // file names are separated by spaces in XML code.
    // Since there is no space escaping implemented in the engine, file lookup would break at this point.
    // Thus, this validator is a little more sensitive.
    private static readonly char[] ForbiddenChars = ['/', ' ', '\0', '\t', '\r', '\n'];

    /// <summary>
    /// Gets the maximum allowed size for a MEG data entry when validating entries
    /// for Empire at War and Forces of Corruption MEG files.
    /// </summary>
    /// <value>
    /// The maximum allowed size for a MEG data entry when validating entries for Empire at War and Forces of Corruption MEG files.
    /// Maximum entry size is limited to 2GB (2^32 - 1 bytes).
    /// </value>
    protected override uint MaxMegEntrySize { get; } = MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.EawFoc).MaxEntrySize;

    /// <remarks>
    /// This method performs several checks to ensure the validity of the MEG data entry:
    /// <list type="bullet">
    /// <item>Ensures the data entry is not encrypted.</item>
    /// <item>Validates that the file path is not empty, does not exceed the maximum allowed length, and does not contain invalid characters.</item>
    /// <item>Checks that the file path is properly formatted, uppercased, and correctly encoded.</item>
    /// </list>
    /// </remarks>
    /// <inheritdoc/>
    protected override MegDataEntryValidationResult ValidateCore(MegDataEntryBuilderInfo dataEntry)
    {
        if (dataEntry.Encrypted)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.Invalid, 
                "MEG data entry cannot be encrypted.");

        var entryPath = dataEntry.EntryPath;
        if (entryPath.Length is 0)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath,
                "MEG data entry path cannot be empty.");

        if (entryPath.Length > MegFileConstants.EawMaxEntryPathLength)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath, 
                $"MEG data entry path cannot be larger than {MegFileConstants.EawMaxEntryPathLength}.");

        if (IsRootedOrStartsWithCurrent(entryPath))
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath,
                "MEG data entry path cannot with current directory.");

        if (entryPath.IndexOfAny(ForbiddenChars) != -1)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath,
                "MEG data entry path contains invalid characters.");

        // If the path contains ':' the first one must also be the first character, iff we have a slashes present
        // This rule does not really make any sense (e.g, path ":\MY\TEST.TXT" or "TEST:TEST")
        if (entryPath.IndexOf(':') > 0 && entryPath.IndexOf('\\') != -1)
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath,
                "MEG data entry path contains invalid characters.");

        Span<char> pathBuffer = stackalloc char[MegFileConstants.EawMaxEntryPathLength];

        var upperLength = entryPath.ToUpperInvariant(pathBuffer);
        var upper = pathBuffer.Slice(0, upperLength);

        if (upperLength != entryPath.Length || !entryPath.Equals(upper, StringComparison.Ordinal))
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath,
                "MEG data entry path is not uppercased.");

        var megEncoding = MegFileConstants.MegDataEntryPathEncoding;
        var asciiLength = megEncoding.EncodeString(entryPath, pathBuffer, megEncoding.GetByteCountPG(entryPath.Length));
        var asAscii = pathBuffer.Slice(0, asciiLength);

        if (asciiLength != entryPath.Length || !entryPath.Equals(asAscii, StringComparison.Ordinal))
            return new MegDataEntryValidationResult(MegDataEntryValidationStatus.InvalidPath,
                "MEG data entry path is not correctly encoded.");

        return MegDataEntryValidationResult.Valid;
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

        return path[0] is '.' && path[1] is '\\';
    }
}