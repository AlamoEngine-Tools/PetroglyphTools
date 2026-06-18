// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MTD.Data;

namespace PG.StarWarsGame.Files.MTD.Services;

/// <summary>
/// A service to load Petroglyph <a href="https://modtools.petrolution.net/docs/MtdFileFormat"> .MTD files</a>
/// </summary>
/// <remarks>
/// This service extends <see cref="IMtdFileService"/> with operations that are not tied to a MTD file on disk,
/// such as loading a <see cref="IMegaTextureDirectory"/> from an arbitrary stream or in-memory buffer.
/// </remarks>
public interface IMtdService : IMtdFileService
{
    /// <summary>
    /// Loads a *.MTD stream into a <see cref="IMegaTextureDirectory"/>.
    /// </summary>
    /// <param name="stream">The MTD file stream.</param>
    /// <returns>A representation of the MTD file.</returns>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a valid MTD file.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IMegaTextureDirectory LoadModel(Stream stream);

    /// <summary>
    /// Loads a *.MTD byte buffer into a <see cref="IMegaTextureDirectory"/>.
    /// </summary>
    /// <param name="data">The MTD file bytes.</param>
    /// <returns>A representation of the MTD file.</returns>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a valid MTD file.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    IMegaTextureDirectory LoadModel(byte[] data);

    /// <summary>
    /// Loads a *.MTD byte buffer into a <see cref="IMegaTextureDirectory"/>.
    /// </summary>
    /// <param name="data">The MTD file bytes.</param>
    /// <returns>A representation of the MTD file.</returns>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a valid MTD file.</exception>
    IMegaTextureDirectory LoadModel(ReadOnlySpan<byte> data);
}
