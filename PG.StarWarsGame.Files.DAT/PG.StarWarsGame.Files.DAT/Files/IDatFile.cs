// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Commons.Files;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <summary>
///     The provided holder for Petroglyph's <a href="https://modtools.petrolution.net/docs/DatFileFormat">.DAT files </a>.
///     <br />DAT files hold key-value pairs of strings that are used to localize the game.
/// </summary>
public interface IDatFile : IPetroglyphFileHolder<,,>
{
    /// <summary>
    ///     Gets the sorting type of the DAT file's entries.
    /// </summary>
    public DatFileType Order { get; }
}