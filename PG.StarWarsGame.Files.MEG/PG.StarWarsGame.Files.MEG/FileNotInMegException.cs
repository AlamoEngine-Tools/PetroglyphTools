// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// Exception thrown, if a file is not contained in a .MEG archive.
/// </summary>
public sealed class FileNotInMegException : Exception
{
    private readonly string _file;
    private readonly string _megFile;

    /// <inheritdoc/>
    public override string Message => $"The file \"{_file}\" is not contained in the MEG archive \"{_megFile}\"";

    /// <summary>
    /// Initializes a new instance of the <see cref="FileNotInMegException"/> class.
    /// </summary>
    /// <param name="file">The requested file which was not found.</param>
    /// <param name="megFile">The .meg file that was queried.</param>
    public FileNotInMegException(string file, string megFile)
    {
        _file = file;
        _megFile = megFile;
    }
}