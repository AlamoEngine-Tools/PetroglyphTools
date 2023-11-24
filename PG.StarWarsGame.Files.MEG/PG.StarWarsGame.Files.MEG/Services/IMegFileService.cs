// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// A service to load and create Petroglyph's <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG files</a>
/// </summary>
public interface IMegFileService
{
    /// <summary>
    /// Packs a collection of files to a new *.MEG archive.
    /// </summary>
    /// <remarks>
    /// In the case <paramref name="builderInformation"/> contains an encrypted <see cref="MegDataEntry"/>, it will be decrypted first.
    /// <br/>
    /// The items of <paramref name="builderInformation"/> will be correctly sorted by this operation.
    /// </remarks>
    /// <param name="megFileParameters">The desired file properties of the new MEG archive.</param>
    /// <param name="builderInformation">A collection of file references to be packed into the MEG archive.</param>
    /// <param name="overwrite">When set to <see langword="true"/> existing files will be overwritten; otherwise the creation of the MEG file will cause an <see cref="IOException"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="megFileParameters"/> or <paramref name="builderInformation"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="megFileParameters"/> contains invalid data.</exception>
    /// <exception cref="IOException">The MEG file could not be created.</exception>
    void CreateMegArchive(MegFileHolderParam megFileParameters, IEnumerable<MegFileDataEntryBuilderInfo> builderInformation, bool overwrite);

    /// <summary>
    /// Loads a *.MEG file's metadata into a <see cref="MegFileHolder" />.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>The specified *.MEG file's metadata.</returns>
    /// <exception cref="NotSupportedException">When trying to load an encrypted MEG archive.</exception>
    /// <exception cref="BinaryCorruptedException">When <paramref name="filePath"/> could not be read as a MEG archive.</exception>
    /// <exception cref="FileNotFoundException">When <paramref name="filePath"/> was not found.</exception>
    IMegFile Load(string filePath);

    /// <summary>
    /// Loads a *.MEG file's metadata into a <see cref="MegFileHolder" />.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns>The specified *.MEG file's metadata.</returns>
    /// <exception cref="NotSupportedException">When trying to load a non-encrypted MEG archive.</exception>
    /// <exception cref="BinaryCorruptedException">When <paramref name="filePath"/> could not be read as a MEG archive.</exception>
    /// <exception cref="FileNotFoundException">When <paramref name="filePath"/> was not found.</exception>
    IMegFile Load(string filePath, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from a .MEG file.
    /// </summary>
    /// <param name="file">The .MEG file.</param>
    /// <param name="encrypted">Indicates whether the .MEG archive is encrypted or not.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid MEG archive.</exception>
    /// <exception cref="FileNotFoundException">When <paramref name="file"/> was not found.</exception>
    MegFileVersion GetMegFileVersion(string file, out bool encrypted);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from a .MEG archive stream.
    /// </summary>
    /// <param name="stream">The .MEG's archive stream</param>
    /// <param name="encrypted">Indicates whether the .MEG archive is encrypted or not.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid MEG archive.</exception>
    MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted);
}
