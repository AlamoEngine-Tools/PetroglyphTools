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
/// A service to load and create Petroglyph <a href="https://modtools.petrolution.net/docs/DatFileFormat"> Localized String Tables (.DAT)</a>.
/// </summary>
public interface IDatService
{
    /// <summary>
    /// Creates a binary DAT from a collection of string entries and writes it to a specified stream.
    /// </summary>
    /// <param name="stream">The stream to write the DAT content to.</param>
    /// <param name="entries">A list of key-value-pairs to be stored in the DAT file.</param>
    /// <param name="fileType">
    /// Determines whether the output file's entries will be ordered (usually used for
    /// <c>mastertextfile_LANGUAGE.dat</c>) or the sort-order of the provided entries will be preserved
    /// (usually used for <c>creditstextfile_LANGUAGE.dat</c>).
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="entries"/> is <see langword="null"/>.</exception>
    /// <exception cref="IOException">The DAT file could not be created.</exception>
    void CreateDatBinary(Stream stream, IEnumerable<DatStringEntry> entries, DatLayoutKind fileType);

    /// <summary>
    /// Loads a DAT file into a <see cref="IDatFile"/>
    /// </summary>
    /// <param name="filePath">The path to the DAT file.</param>
    /// <returns>The loaded DAT file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a DAT archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    IDatFile LoadFile(string filePath);

    /// <summary>
    /// Loads a DAT file into a <see cref="IDatFile"/> using the specified layout.
    /// </summary>
    /// <param name="filePath">The path to the DAT file.</param>
    /// <param name="requestedLayout">The requested type of the DAT model.</param>
    /// <returns>The loaded DAT file</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedLayout"/> is not compatible to the loaded file.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="filePath"/> is not a DAT archive.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="filePath"/> is not found.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    IDatFile LoadFileAs(string filePath, DatLayoutKind requestedLayout);

    /// <summary>
    /// Loads a DAT file into a <see cref="DatFile"/>.
    /// </summary>
    /// <param name="fileStream">The DAT file stream.</param>
    /// <returns>The loaded DAT file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileStream"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="fileStream"/> is not a DAT archive.</exception>
    /// <exception cref="NotSupportedException"><paramref name="fileStream"/> is not readable or seekable.</exception>
    IDatFile LoadFile(FileSystemStream fileStream);

    /// <summary>
    /// Loads a DAT file into a <see cref="IDatFile"/> using the specified layout.
    /// </summary>
    /// <param name="fileStream">The DAT file stream.</param>
    /// <param name="requestedLayout">The requested type of the DAT model.</param>
    /// <returns>The loaded DAT file</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedLayout"/> is not compatible to the loaded file.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="fileStream"/> is not a DAT archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileStream"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException"><paramref name="fileStream"/> is not readable or seekable.</exception>
    IDatFile LoadFileAs(FileSystemStream fileStream, DatLayoutKind requestedLayout);

    /// <summary>
    /// Loads a DAT model from the specified stream.
    /// </summary>
    /// <param name="stream">The DAT stream.</param>
    /// <returns>The loaded DAT model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a DAT archive.</exception>
    IDatModel LoadModel(Stream stream);

    /// <summary>
    /// Loads a DAT model from the specified byte array.
    /// </summary>
    /// <param name="data">The DAT bytes.</param>
    /// <returns>The loaded DAT model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    IDatModel LoadModel(byte[] data);

    /// <summary>
    /// Loads a DAT model from the specified read-only span of bytes.
    /// </summary>
    /// <param name="data">The DAT bytes.</param>
    /// <returns>The loaded DAT model.</returns>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    IDatModel LoadModel(ReadOnlySpan<byte> data);

    /// <summary>
    /// Loads a DAT model from the specified stream and layout kind.
    /// </summary>
    /// <param name="stream">The DAT stream.</param>
    /// <param name="requestedLayout">The requested layout of the DAT model.</param>
    /// <returns>The loaded DAT model</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedLayout"/> is not compatible to the loaded DAT.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="stream"/> is not a DAT archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IDatModel LoadModelAs(Stream stream, DatLayoutKind requestedLayout);

    /// <summary>
    /// Loads a DAT model from the specified bytes and type.
    /// </summary>
    /// <param name="data">The DAT bytes.</param>
    /// <param name="requestedLayout">The requested layout of the DAT model.</param>
    /// <returns>The loaded DAT model</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedLayout"/> is not compatible to the loaded DAT.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    IDatModel LoadModelAs(byte[] data, DatLayoutKind requestedLayout);

    /// <summary>
    /// Loads a DAT model from the specified read-only span of bytes and layout kind.
    /// </summary>
    /// <param name="data">The DAT bytes.</param>
    /// <param name="requestedLayout">The requested layout of the DAT model.</param>
    /// <returns>The loaded DAT model</returns>
    /// <exception cref="InvalidOperationException"><paramref name="requestedLayout"/> is not compatible to the loaded DAT.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="data"/> is not a DAT archive.</exception>
    IDatModel LoadModelAs(ReadOnlySpan<byte> data, DatLayoutKind requestedLayout);

    /// <summary>
    /// Determines whether the specified DAT file is <see cref="DatLayoutKind.OrderedByCrc32" /> or <see cref="DatLayoutKind.NotOrdered" />.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT files this method returns <see cref="DatLayoutKind.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="filePath">The path to the DAT file.</param>
    /// <returns>The layout of the DAT.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    DatLayoutKind GetDatLayoutKind(string filePath);

    /// <summary>
    /// Determines whether the specified DAT stream is <see cref="DatLayoutKind.OrderedByCrc32"/> or <see cref="DatLayoutKind.NotOrdered"/>.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT data this method returns <see cref="DatLayoutKind.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="stream">The DAT data stream.</param>
    /// <returns>The layout of the DAT.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    DatLayoutKind GetDatLayoutKind(Stream stream);

    /// <summary>
    /// Determines whether the specified byte array is a DAT using the layout <see cref="DatLayoutKind.OrderedByCrc32"/> or <see cref="DatLayoutKind.NotOrdered"/>.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT data this method returns <see cref="DatLayoutKind.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="data">The DAT data bytes.</param>
    /// <returns>The layout of the DAT.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    DatLayoutKind GetDatLayoutKind(byte[] data);

    /// <summary>
    /// Determines whether the specified read-only span of bytes is a DAT using the layout
    /// <see cref="DatLayoutKind.OrderedByCrc32" /> or <see cref="DatLayoutKind.NotOrdered"/>.
    /// </summary>
    /// <remarks>
    /// For empty or single-entry DAT data this method returns <see cref="DatLayoutKind.OrderedByCrc32"/>
    /// </remarks>
    /// <param name="data">The DAT bytes.</param>
    /// <returns>The layout of the DAT.</returns>
    DatLayoutKind GetDatLayoutKind(ReadOnlySpan<byte> data);
}
