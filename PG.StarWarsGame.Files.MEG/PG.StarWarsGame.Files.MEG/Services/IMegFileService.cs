// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// A service to load and create Petroglyph <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG files</a>
/// </summary>
public interface IMegFileService
{
    /// <summary>
    /// Creates a MEG file from a collection of data entries and writes it to a specified file stream.
    /// </summary>
    /// <remarks>
    /// This is a low-level operation. It's recommended to use <see cref="IMegBuilder"/> instead, as this provides data validation and normalization.
    /// <para>
    /// Notes:
    /// <br/>
    /// - Any MEG entry file path will be re-encoded to ASCII automatically.
    /// <br/>
    /// - In the case <paramref name="builderInformation"/> references an encrypted MEG data entry, the entry will be decrypted first.
    /// <br/>
    /// - The items of <paramref name="builderInformation"/> will be correctly sorted by this operation.
    /// </para>
    /// </remarks>
    /// <param name="fileStream">The destination file stream to write the MEG archive to.</param>
    /// <param name="fileVersion">The MEG file version to use.</param>
    /// <param name="encryptionData">Optional encryption data.</param>
    /// <param name="builderInformation">A collection of file references to be packed into the MEG archive.</param>
    /// <exception cref="ArgumentNullException"><paramref name="fileStream"/> or <paramref name="builderInformation"/> is <see langword="null"/>.</exception>
    /// <exception cref="IOException">The MEG file could not be created.</exception>
    /// <exception cref="FileNotFoundException">A data entry file was not found.</exception>
    /// <exception cref="NotSupportedException">This library does not support creating the MEG archive from the specified arguments.</exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="InvalidOperationException">Attempted to create MEG archive which does not match the expected binary result.</exception>
    void CreateMegArchive(FileSystemStream fileStream, MegFileVersion fileVersion, MegEncryptionData? encryptionData, IEnumerable<MegDataEntryBuilderInfo> builderInformation);

    /// <summary>
    /// Loads a *.MEG file's metadata into a <see cref="IMegFile" />.
    /// </summary>
    /// <param name="filePath">The MEG file path.</param>
    /// <returns>The MEG file's metadata.</returns>
    /// <exception cref="NotSupportedException">This library does not support the specified MEG archive.</exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a MEG archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegFile Load(string filePath);

    /// <summary>
    /// Loads a *.MEG metadata stream into a <see cref="IMegFile" />.
    /// </summary>
    /// <param name="stream">The MEG file path.</param>
    /// <returns>The MEG file's metadata.</returns>
    /// <exception cref="NotSupportedException">
    /// <para>
    /// This library does not support the specified MEG archive.
    /// </para>
    /// <para>
    /// <paramref name="stream"/> is not readable or seekable.
    /// </para>
    /// </exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a MEG archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegFile Load(FileSystemStream stream);

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
