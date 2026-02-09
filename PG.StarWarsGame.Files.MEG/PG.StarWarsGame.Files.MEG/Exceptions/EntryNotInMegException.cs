// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// The exception that is thrown when a data entry is not found inside a MEG archive.
/// </summary>
public sealed class EntryNotInMegException : Exception
{
    private readonly string _entry;
    private readonly string _megFile;

    /// <inheritdoc/>
    public override string Message => field ??= $"The entry \"{_entry}\" is not contained in the MEG archive \"{_megFile}\"";

    /// <summary>
    /// Initializes a new instance of the <see cref="EntryNotInMegException"/> class with a location reference which does not exist.
    /// </summary>
    /// <param name="locationReference">The non-existing data entry location.</param>
    internal EntryNotInMegException(MegDataEntryLocationReference locationReference)
    {
        _entry = locationReference.DataEntry.Path;
        _megFile = locationReference.MegFile.FilePath;
    }
}