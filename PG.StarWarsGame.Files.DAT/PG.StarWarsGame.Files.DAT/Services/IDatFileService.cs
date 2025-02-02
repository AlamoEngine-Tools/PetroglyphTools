// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.Binary;
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
    /// <param name="fileStream">The file stream to write the DAT content to.</param>
    /// <param name="entries">A list of key-value-pairs to be stored in the DAT file.</param>
    /// <param name="fileType">
    ///     Determines whether the output file's entries will be ordered (usually used for
    ///     <code>mastertextfile_LANGUAGE.dat</code>) or the sort-order of the provided entries will be preserved (usually used
    ///     for <code>creditstextfile_LANGUAGE.dat</code>).
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="fileStream"/> or <paramref name="entries"/> is <see langword="null"/>.</exception>
    /// <exception cref="IOException">The DAT file could not be created.</exception>
    void CreateDatFile(FileSystemStream fileStream, IEnumerable<DatStringEntry> entries, DatFileType fileType);

    /// <summary>
    ///     Loads a *.DAT file from the provided path into a <see cref="DatFile" />
    /// </summary>
    /// <param name="filePath">The path to the DAT file.</param>
    /// <returns>The loaded DAT file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a DAT archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    IDatFile Load(string filePath);

    /// <summary>
    ///  Loads a *.DAT file from the provided path and 
    /// </summary>
    /// <param name="filePath">The path to the DAT file.</param>
    /// <param name="requestedFileType">The requested type of the DAT model.</param>
    /// <returns>The loaded DAT file</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedFileType"/> is not compatible to the loaded file.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a DAT archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    IDatFile LoadAs(string filePath, DatFileType requestedFileType);

    /// <summary>
    ///     Loads a *.DAT file from the provided path into a <see cref="DatFile" />
    /// </summary>
    /// <param name="fileStream">The DAT file stream.</param>
    /// <returns>The loaded DAT file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileStream"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="fileStream"/> is not a DAT archive.</exception>
    /// <exception cref="NotSupportedException"><paramref name="fileStream"/> is not readable or seekable.</exception>
    IDatFile Load(FileSystemStream fileStream);

    /// <summary>
    ///  Loads a *.DAT file from the provided path and 
    /// </summary>
    /// <param name="fileStream">The DAT file stream.</param>
    /// <param name="requestedFileType">The requested type of the DAT model.</param>
    /// <returns>The loaded DAT file</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedFileType"/> is not compatible to the loaded file.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="fileStream"/> is not a DAT archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileStream"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException"><paramref name="fileStream"/> is not readable or seekable.</exception>
    IDatFile LoadAs(FileSystemStream fileStream, DatFileType requestedFileType);

    /// <summary>
    /// Determines whether a provided DAT file is <see cref="DatFileType.OrderedByCrc32" /> or <see cref="DatFileType.NotOrdered" />.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT files this method returns <see cref="DatFileType.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="filePath">The path to the DAT file.</param>
    /// <returns>The loaded DAT file</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    DatFileType GetDatFileType(string filePath);
}