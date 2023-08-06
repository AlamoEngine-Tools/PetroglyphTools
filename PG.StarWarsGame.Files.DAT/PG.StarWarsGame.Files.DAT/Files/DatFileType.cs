// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Services;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <summary>
///     Represents the available entry ordering of a Petroglyph translation data file archive as defined
///     in the <a href="https://modtools.petrolution.net/docs/DatFileFormat">.DAT file specification</a>.
/// </summary>
public enum DatFileType
{
    /// <summary>
    ///     Represents a sorted .DAT file. This file only allows for unique text keys and is expected to
    ///     be sorted on the text key's <see cref="Crc32" /> in an ascending manner. Any
    ///     mastertextfile_&lt;language&gt;.dat file is of this archive type.
    /// </summary>
    OrderedByCrc32,

    /// <summary>
    ///     Represents an unsorted .DAT file. This file type allows (and encourages) non-unique text
    ///     keys, as this format is used for the creditstextfile_&lt;language&gt;.dat, where the keys
    ///     act as formatting information/instructions rather than queriable keys. The order of the
    ///     entries is also equivalent to the order of the credits diplayed on screen, so the insert
    ///     order of the key-value pairs is important and should be retained.
    /// </summary>
    NotOrdered
}