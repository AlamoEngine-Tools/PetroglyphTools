// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// The exception that is thrown when a data entry is not found inside a MEG archive.
/// </summary>
public sealed class FileNotInMegException : Exception
{
    private readonly string _file;
    private readonly string _megFile;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string Message => $"The file \"{_file}\" is not contained in the MEG archive \"{_megFile}\"";

    /// <summary>
    /// Initializes a new instance of the <see cref="FileNotInMegException"/> class with a location reference which does not exist.
    /// </summary>
    /// <param name="locationReference">The non-existing data entry location.</param>
    public FileNotInMegException(MegDataEntryLocationReference locationReference)
    {
        _file = locationReference.DataEntry.FilePath;
        _megFile = locationReference.MegFile.FilePath;
    }
}