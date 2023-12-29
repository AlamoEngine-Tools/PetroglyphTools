// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
///     A service to load and create Petroglyph
///     <a href="https://modtools.petrolution.net/docs/DatFileFormat"> .DAT files</a>
/// </summary>
public interface IDatFileService
{
    /// <summary>
    ///     Takes a list of <see cref="DatFileEntry" />s and packs them into a DAT file.
    /// </summary>
    /// <param name="datFileName">
    ///     The desired name of the DAT file. Usually something like
    ///     <code>mastertextfile_english.dat</code>
    /// </param>
    /// <param name="targetDirectory">The target directory to which the DAT archive will be written.</param>
    /// <param name="entries">A list of key-value-pairs to be stored in the DAT file.</param>
    /// <param name="datFileType">
    ///     Determines whether the output file's entries will be ordered (usually used for
    ///     <code>mastertextfile_LANGUAGE.dat</code>) or the sort-order of the provided entries will be preserved (usually used
    ///     for <code>creditstextfile_LANGUAGE.dat</code>).
    /// </param>
    void StoreDatFile(string datFileName,
        string targetDirectory,
        IEnumerable<DatFileEntry> entries,
        DatFileType datFileType);

    /// <summary>
    ///     Loads a *.DAT file from the provided path into a <see cref="DatFileHolder" />
    /// </summary>
    /// <param name="filePath">The absolute path to the DAT file.</param>
    /// <returns></returns>
    IDatFile LoadDatFile(string filePath);

    /// <summary>
    ///     Determines whether a provided DAT file is <see cref="DatFileType.OrderedByCrc32" /> or
    ///     <see cref="DatFileType.NotOrdered" />.
    /// </summary>
    /// <param name="filePath">The absolute path to the DAT file.</param>
    /// <returns></returns>
    DatFileType PeekDatFileType(string filePath);
}