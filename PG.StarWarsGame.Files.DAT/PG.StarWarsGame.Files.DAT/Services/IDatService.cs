// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// A service to load and create Petroglyph <a href="https://modtools.petrolution.net/docs/DatFileFormat"> .DAT files</a>
/// </summary>
/// <remarks>
/// This service extends <see cref="IDatFileService"/> with operations that are not tied to a DAT file on disk,
/// such as loading a <see cref="IDatModel"/> from an arbitrary stream or in-memory buffer.
/// </remarks>
public interface IDatService : IDatFileService
{
    /// <summary>
    ///     Loads a *.DAT file from the provided stream into a <see cref="IDatModel" />
    /// </summary>
    /// <param name="stream">The DAT file stream.</param>
    /// <returns>The loaded DAT model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a DAT archive.</exception>
    IDatModel LoadModel(Stream stream);

    /// <summary>
    ///     Loads a *.DAT file from the provided bytes into a <see cref="IDatModel" />
    /// </summary>
    /// <param name="data">The DAT file bytes.</param>
    /// <returns>The loaded DAT model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    IDatModel LoadModel(byte[] data);

    /// <summary>
    ///     Loads a *.DAT file from the provided bytes into a <see cref="IDatModel" />
    /// </summary>
    /// <param name="data">The DAT file bytes.</param>
    /// <returns>The loaded DAT model.</returns>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    IDatModel LoadModel(ReadOnlySpan<byte> data);

    /// <summary>
    ///  Loads a *.DAT file from the provided stream and type.
    /// </summary>
    /// <param name="stream">The DAT file stream.</param>
    /// <param name="requestedFileType">The requested type of the DAT model.</param>
    /// <returns>The loaded DAT model</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedFileType"/> is not compatible to the loaded file.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a DAT archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IDatModel LoadModelAs(Stream stream, DatFileType requestedFileType);

    /// <summary>
    ///  Loads a *.DAT file from the provided bytes and type.
    /// </summary>
    /// <param name="data">The DAT file bytes.</param>
    /// <param name="requestedFileType">The requested type of the DAT model.</param>
    /// <returns>The loaded DAT model</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedFileType"/> is not compatible to the loaded file.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    IDatModel LoadModelAs(byte[] data, DatFileType requestedFileType);

    /// <summary>
    ///  Loads a *.DAT file from the provided bytes and type.
    /// </summary>
    /// <param name="data">The DAT file bytes.</param>
    /// <param name="requestedFileType">The requested type of the DAT model.</param>
    /// <returns>The loaded DAT model</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedFileType"/> is not compatible to the loaded file.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    IDatModel LoadModelAs(ReadOnlySpan<byte> data, DatFileType requestedFileType);

    /// <summary>
    /// Determines whether a provided DAT file is <see cref="DatFileType.OrderedByCrc32" /> or <see cref="DatFileType.NotOrdered" />.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT files this method returns <see cref="DatFileType.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="stream">The DAT file stream.</param>
    /// <returns>The type of the DAT file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    DatFileType GetDatFileType(Stream stream);

    /// <summary>
    /// Determines whether a provided DAT file is <see cref="DatFileType.OrderedByCrc32" /> or <see cref="DatFileType.NotOrdered" />.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT files this method returns <see cref="DatFileType.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="data">The DAT file bytes.</param>
    /// <returns>The type of the DAT file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    DatFileType GetDatFileType(byte[] data);

    /// <summary>
    /// Determines whether a provided DAT file is <see cref="DatFileType.OrderedByCrc32" /> or <see cref="DatFileType.NotOrdered" />.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT files this method returns <see cref="DatFileType.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="data">The DAT file bytes.</param>
    /// <returns>The type of the DAT file.</returns>
    DatFileType GetDatFileType(ReadOnlySpan<byte> data);
}
