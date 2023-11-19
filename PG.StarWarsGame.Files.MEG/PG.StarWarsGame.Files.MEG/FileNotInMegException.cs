// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// The exception that is thrown when a data entry is not found inside a MEG archive.
/// </summary>
public sealed class FileNotInMegException : MegDataEntryNotFoundException
{
    private readonly string _file;
    private readonly string _megFile;

    /// <inheritdoc/>
    public override string Message => $"The file \"{_file}\" is not contained in the MEG archive \"{_megFile}\"";

    /// <summary>
    /// Initializes a new instance of the <see cref="FileNotInMegException"/> class with the failing search query,
    /// consisting of a MEG file and a data entry.
    /// </summary>
    /// <param name="file">The requested file which was not found.</param>
    /// <param name="megFile">The .MEG file that was searched.</param>
    public FileNotInMegException(string file, string megFile)
    {
        _file = file;
        _megFile = megFile;
    }
}