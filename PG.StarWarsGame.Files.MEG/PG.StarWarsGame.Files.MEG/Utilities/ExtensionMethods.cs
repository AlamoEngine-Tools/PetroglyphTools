﻿using System;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Utilities;

/// <summary>
/// Provides useful extension methods for this library.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Creates a new <see cref="MegFileHolderParam"/> instance for the given <see cref="IMegFile"/>.
    /// </summary>
    /// <param name="megFile">The meg file.</param>
    /// <returns>´The meg file's parameters.</returns>
    public static MegFileHolderParam CreateParam(this IMegFile megFile)
    {
        // We don't want this to be an instance method, because that could be understood the returned MegFileHolderParam instance is a singleton.
        // This however is not possible, cause MegFileHolderParam is disposable for security/confidentiallty reasons. 
        // MegFileHolderParam is considered to be short-lived and thus should not co-exist with the holder it represents.

        if (megFile == null) 
            throw new ArgumentNullException(nameof(megFile));

        return new MegFileHolderParam
        {
            FilePath = megFile.FilePath,
            FileVersion = megFile.FileVersion,
            Key = megFile.Key,
            IV = megFile.IV
        };
    }


    /// <summary>
    /// Creates a new <see cref="MegFileDataEntryBuilderInfo"/> from a <see cref="MegDataEntryOriginInfo"/>.
    /// </summary>
    /// <param name="origin">The origin info.</param>
    /// <returns>The created builder info.</returns>
    public static MegFileDataEntryBuilderInfo ToBuilderInfo(this MegDataEntryOriginInfo origin)
    {
        return new MegFileDataEntryBuilderInfo(origin);
    }
}