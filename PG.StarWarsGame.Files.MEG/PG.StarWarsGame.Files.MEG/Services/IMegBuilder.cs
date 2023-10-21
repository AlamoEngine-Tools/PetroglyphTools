// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// High-level builder service which takes certain restrictions form the PG games into account.
/// </summary>
public interface IMegBuilder
{
    /// <summary>
    /// Gets a list of all files of this builder instance which shall be packed to a .MEG file, sorted by their file names CRC32 value.
    /// </summary>
    public IReadOnlyList<MegFileDataEntryBuilderInfo> Files { get; }

    /// <summary>
    /// The file name of the .MEG archive.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// The destination directory path of the .MEG archive.
    /// </summary>
    public string Destination { get; }

    /// <summary>
    /// Adds a given <see cref="MegFileDataEntryBuilderInfo"/> to this instance
    /// </summary>
    /// <remarks>
    /// File names will be normalized by using:a    
    /// <code>
    ///     upper case characters
    ///     unified path separator character,
    ///     single byte encoding for strings
    /// </code>
    /// <br/>
    /// When adding a file info with an already existing file name (by CRC32), the previous entry will be removed.
    /// </remarks>
    /// <param name="dataEntryBuilderInfo">The file information to add</param>
    /// <returns>This instance.</returns>
    IMegBuilder AddFile(MegFileDataEntryBuilderInfo dataEntryBuilderInfo);

    /// <summary>
    /// Build the .MEG file from this instance.
    /// </summary>
    /// <param name="megVersion">The .MEG archive file version to use.</param>
    void Build(MegFileVersion megVersion);

    /// <summary>
    /// Build an encrypted .MEG v3 file from this instance.
    /// </summary>
    /// <param name="key">The encryption key.</param>
    /// <param name="iv">The encryption initialization vector.</param>
    void BuildEncrypted(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);
}