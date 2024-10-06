// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MTD.Files;

namespace PG.StarWarsGame.Files.MTD.Services;

/// <summary>
/// A service to load and create Petroglyph <a href="https://modtools.petrolution.net/docs/MtdFileFormat"> .MTD files</a>
/// </summary>
public interface IMtdFileService
{
    /// <summary>
    /// Loads a *.MTD stream into a <see cref="IMtdFile" />.
    /// </summary>
    /// <param name="filePath">The MTD file path.</param>
    /// <returns>A representation of the MTD file.</returns>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a valid MTD file.</exception>
    IMtdFile Load(string filePath);

    /// <summary>
    /// Loads a *.MTD stream into a <see cref="IMtdFile" />.
    /// </summary>
    /// <param name="stream">The MTD file stream.</param>
    /// <returns>A representation of the MTD file.</returns>
    /// <exception cref="NotSupportedException"><paramref name="stream"/> is not readable or seekable.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a valid MTD file.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IMtdFile Load(Stream stream);
}