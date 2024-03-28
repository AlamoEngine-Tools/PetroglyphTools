// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// A service to load and create Petroglyph <a href="https://modtools.petrolution.net/docs/DatFileFormat"> .DAT files</a>
/// </summary>
public interface IDatFileService
{
    /// <summary>
    ///     Takes a collection of <see cref="DatStringEntry" />s and packs them into a DAT file.
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="entries">A list of key-value-pairs to be stored in the DAT file.</param>
    /// <param name="fileType">
    ///     Determines whether the output file's entries will be ordered (usually used for
    ///     <code>mastertextfile_LANGUAGE.dat</code>) or the sort-order of the provided entries will be preserved (usually used
    ///     for <code>creditstextfile_LANGUAGE.dat</code>).
    /// </param>
    void CreateDatFile(FileSystemStream fileStream, IEnumerable<DatStringEntry> entries, DatFileType fileType);

    /// <summary>
    ///     Loads a *.DAT file from the provided path into a <see cref="DatFile" />
    /// </summary>
    /// <param name="filePath">The absolute path to the DAT file.</param>
    /// <returns></returns>
    IDatFile Load(string filePath);

    /// <summary>
    ///  Loads a *.DAT file from the provided path and 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="requestedFileType"></param>
    /// <returns></returns>
    IDatFile LoadAs(string filePath, DatFileType requestedFileType);

    /// <summary>
    /// Determines whether a provided DAT file is <see cref="DatFileType.OrderedByCrc32" /> or <see cref="DatFileType.NotOrdered" />.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT files this method returns <see cref="DatFileType.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="filePath">The path to the DAT file.</param>
    /// <returns></returns>
    DatFileType GetDatFileType(string filePath);
}