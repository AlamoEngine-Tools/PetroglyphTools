// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
/// Minimal file info for the <a href="https://modtools.petrolution.net/docs/MegFileFormat"> MEG archive file type</a>.
/// </summary>
public readonly struct MegAlamoFileType : IAlamoFileType
{
    /// <inheritdoc />
    public FileType Type => FileType.Binary;

    /// <inheritdoc />
    public string FileExtension => "meg";
}
