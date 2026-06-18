// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// A service to load and create Petroglyph <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG archives.</a>
/// </summary>
/// <remarks>
/// This service extends <see cref="IMegFileService"/> with operations that are not tied to a MEG file on disk,
/// such as loading a readable MEG from an arbitrary stream.
/// </remarks>
public interface IMegService : IMegFileService
{
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
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
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
    /// <exception cref="MegSizeException">The MEG archive or its entries are exceeding the supported file size.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a MEG archive.</exception>
    /// <exception cref="InvalidOperationException">Attempts to load an encrypted MEG archive.</exception>
    IMegDataSource LoadArchive(ReadOnlySpan<byte> data);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from a MEG archive stream.
    /// </summary>
    /// <param name="stream">The stream containing the .MEG archive. Read from its current position.</param>
    /// <param name="encrypted">When this method returns, contains a value indicating whether the .MEG archive is encrypted.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid MEG archive.</exception>
    MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from an in-memory .MEG archive buffer.
    /// </summary>
    /// <param name="data">The bytes of the .MEG archive.</param>
    /// <param name="encrypted">When this method returns, contains a value indicating whether the .MEG archive is encrypted.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException">The input was not recognized as a valid MEG archive.</exception>
    MegFileVersion GetMegFileVersion(byte[] data, out bool encrypted);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from a read-only span of .MEG archive bytes.
    /// </summary>
    /// <param name="data">The bytes of the .MEG archive.</param>
    /// <param name="encrypted">When this method returns, contains a value indicating whether the .MEG archive is encrypted.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input was not recognized as a valid MEG archive.</exception>
    MegFileVersion GetMegFileVersion(ReadOnlySpan<byte> data, out bool encrypted);
}
