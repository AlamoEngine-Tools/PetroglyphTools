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
    /// Creates a MEG file from a collection of data entries. It's recommended to use <see cref="IMegBuilder"/> instead, as this provides more data validation.
    /// </summary>
    /// <remarks>
    /// Notes:
    /// <br/>
    /// - Any MEG entry file path will be re-encoded to ASCII automatically.
    /// <br/>
    /// - In the case <paramref name="builderInformation"/> contains an encrypted <see cref="MegDataEntry"/>, it will be decrypted first.
    /// <br/>
    /// - The items of <paramref name="builderInformation"/> will be correctly sorted by this operation.
    /// </remarks>
    /// <param name="megFileParameters">The desired file properties of the new MEG archive.</param>
    /// <param name="builderInformation">A collection of file references to be packed into the MEG archive.</param>
    /// <exception cref="ArgumentNullException"><paramref name="megFileParameters"/> or <paramref name="builderInformation"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="megFileParameters"/> contains invalid data.</exception>
    /// <exception cref="IOException">The MEG file could not be created.</exception>
    /// <exception cref="FileNotFoundException">A data entry file was not found.</exception>
    /// <exception cref="NotSupportedException">This library does not support creating the desired MEG archive.</exception>
    void CreateMegArchive(MegFileHolderParam megFileParameters, IEnumerable<MegFileDataEntryBuilderInfo> builderInformation);

    /// <summary>
    /// Loads a *.MEG file's metadata into a <see cref="IMegFile" />.
    /// </summary>
    /// <param name="filePath">The MEG file path.</param>
    /// <returns>The MEG file's metadata.</returns>
    /// <exception cref="NotSupportedException">This library does not support the specified MEG archive.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a MEG archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegFile Load(string filePath);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from a .MEG file.
    /// </summary>
    /// <param name="file">The .MEG file.</param>
    /// <param name="encrypted">Indicates whether the .MEG archive is encrypted or not.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid MEG archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="file"/> is not found.</exception>
    MegFileVersion GetMegFileVersion(string file, out bool encrypted);
}
