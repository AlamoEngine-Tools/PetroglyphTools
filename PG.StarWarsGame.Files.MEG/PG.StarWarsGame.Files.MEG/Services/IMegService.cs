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
/// A service to load and create Petroglyph <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG archives.</a>
/// </summary>
public interface IMegService
{
    /// <summary>
    /// Creates a binary MEG archive from a collection of data entries and writes it to a specified stream.
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
    /// <param name="stream">The destination stream to write the MEG archive to.</param>
    /// <param name="fileVersion">The MEG file version to use.</param>
    /// <param name="encryptionData">Optional encryption data.</param>
    /// <param name="builderInformation">A collection of file references to be packed into the MEG archive.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="builderInformation"/> is <see langword="null"/>.</exception>
    /// <exception cref="IOException">The MEG file could not be created.</exception>
    /// <exception cref="FileNotFoundException">A data entry file was not found.</exception>
    /// <exception cref="NotSupportedException">This library does not support creating the MEG archive from the specified arguments.</exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="InvalidOperationException">Attempted to create MEG archive which does not match the expected binary result.</exception>
    void CreateMegArchive(Stream stream, MegVersion fileVersion, MegEncryptionData? encryptionData, IEnumerable<MegDataEntryBuilderInfo> builderInformation);

    /// <summary>
    /// Loads a file into a <see cref="IMegFile"/>.
    /// </summary>
    /// <param name="filePath">The MEG file path.</param>
    /// <returns>The loaded <see cref="IMegFile"/>.</returns>
    /// <exception cref="NotSupportedException">This library does not support the specified MEG archive.</exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a MEG archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegFile LoadFile(string filePath);

    /// <summary>
    /// Loads a file into a <see cref="IMegFile"/>.
    /// </summary>
    /// <param name="stream">The MEG file path.</param>
    /// <returns>The loaded <see cref="IMegFile"/>.</returns>
    /// <exception cref="NotSupportedException">
    /// <para>
    /// This library does not support the specified MEG archive.
    /// </para>
    /// <para>
    /// OR
    /// </para>
    /// <para>
    /// <paramref name="stream"/> is not readable or seekable.
    /// </para>
    /// </exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a MEG archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegFile LoadFile(FileSystemStream stream);

    /// <summary>
    /// Loads a MEG archive from a stream.
    /// </summary>
    /// <remarks>
    /// If <paramref name="stream"/> is a file system stream, the  method returns an <see cref="IMegFile"/>.
    /// Otherwise, the whole stream is copied into memory and making the returned MEG self-contained.
    /// The caller may dispose <paramref name="stream"/> immediately afterwards.
    /// </remarks>
    /// <param name="stream">The stream containing the MEG archive.</param>
    /// <returns>The loaded <see cref="IMegDataSource"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">This library does not support the specified MEG archive.</exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a MEG archive.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegDataSource LoadArchive(Stream stream);

    /// <summary>
    /// Loads a MEG archive from an in-memory byte buffer.
    /// </summary>
    /// <remarks>
    /// The buffer is copied, so that subsequent mutations of <paramref name="data"/> are not reflected.
    /// </remarks>
    /// <param name="data">The bytes of the whole MEG archive.</param>
    /// <returns>The loaded <see cref="IMegDataSource"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">This library does not support the specified MEG archive.</exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a MEG archive.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegDataSource LoadArchive(byte[] data);

    /// <summary>
    /// Loads a MEG archive from a read-only span of bytes.
    /// </summary>
    /// <remarks>
    /// The span is copied, so that subsequent mutations of the underlying memory are not reflected.
    /// </remarks>
    /// <param name="data">The bytes of the whole MEG archive.</param>
    /// <returns>The loaded <see cref="IMegDataSource"/>.</returns>
    /// <exception cref="NotSupportedException">This library does not support the specified MEG archive.</exception>
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a MEG archive.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegDataSource LoadArchive(ReadOnlySpan<byte> data);

    /// <summary>
    /// Retrieves the <see cref="MegVersion"/> from a MEG file.
    /// </summary>
    /// <param name="file">The .MEG file.</param>
    /// <param name="encrypted">Indicates whether the .MEG archive is encrypted or not.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid MEG archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="file"/> is not found.</exception>
    MegVersion GetMegVersion(string file, out bool encrypted);

    /// <summary>
    /// Retrieves the <see cref="MegVersion"/> from a MEG archive stream.
    /// </summary>
    /// <param name="stream">The stream containing the .MEG archive. Read from its current position.</param>
    /// <param name="encrypted">When this method returns, contains a value indicating whether the .MEG archive is encrypted.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid MEG archive.</exception>
    MegVersion GetMegVersion(Stream stream, out bool encrypted);

    /// <summary>
    /// Retrieves the <see cref="MegVersion"/> from an in-memory .MEG archive buffer.
    /// </summary>
    /// <param name="data">The bytes of the .MEG archive.</param>
    /// <param name="encrypted">When this method returns, contains a value indicating whether the .MEG archive is encrypted.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException">The input was not recognized as a valid MEG archive.</exception>
    MegVersion GetMegVersion(byte[] data, out bool encrypted);

    /// <summary>
    /// Retrieves the <see cref="MegVersion"/> from a read-only span of .MEG archive bytes.
    /// </summary>
    /// <param name="data">The bytes of the .MEG archive.</param>
    /// <param name="encrypted">When this method returns, contains a value indicating whether the .MEG archive is encrypted.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input was not recognized as a valid MEG archive.</exception>
    MegVersion GetMegVersion(ReadOnlySpan<byte> data, out bool encrypted);
}
