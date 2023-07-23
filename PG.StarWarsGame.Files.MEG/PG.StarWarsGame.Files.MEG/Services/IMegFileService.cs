// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// A service to load and create Petroglyph's <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG files</a>
/// </summary>
public interface IMegFileService
{
    /// <summary>
    /// Packs a list of files as an unencrypted *.MEG archive in the given version.
    /// </summary>
    /// <param name="megArchiveName">The desired name of the archive.</param>
    /// <param name="targetDirectory">The target directory to which the .MEG archive will be written.</param>
    /// <param name="packedFileNameToAbsoluteFilePathsMap">A list of absolute file paths, identified by their name in the .MEG file.</param>
    /// <param name="megFileVersion">The file version of the .MEG file.</param>
    void CreateMegArchive(
        string megArchiveName,
        string targetDirectory, 
        IEnumerable<MegFileDataEntryInfo> packedFileNameToAbsoluteFilePathsMap, 
        MegFileVersion megFileVersion);

    /// <summary>
    /// Packs a list of files as an encrypted *.MEG V3 archive.
    /// </summary>
    /// <param name="megArchiveName">The desired name of the archive.</param>
    /// <param name="targetDirectory">The target directory to which the .MEG archive will be written.</param>
    /// <param name="packedFileNameToAbsoluteFilePathsMap">A list of absolute file paths, identified by their name in the .MEG file.</param>
    /// <param name="key">The encryption key.</param>
    /// <param name="iv">The initialization vector used for encryption.</param>
    void CreateMegArchive(
        string megArchiveName, 
        string targetDirectory, 
        IEnumerable<MegFileDataEntryInfo> packedFileNameToAbsoluteFilePathsMap,
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> iv);
    
    /// <summary>
    /// Loads a *.MEG file's metadata into a <see cref="MegFileHolder" />.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>The specified *.MEG file's metadata.</returns>
    IMegFile Load(string filePath);

    /// <summary>
    /// Loads a *.MEG file's metadata into a <see cref="MegFileHolder" />.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns>The specified *.MEG file's metadata.</returns>
    IMegFile Load(string filePath, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from a .MEG file.
    /// </summary>
    /// <param name="file">The .MEG file.</param>
    /// <param name="encrypted">Indicates whether the .MEG archive is encrypted or not.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid .MEG archive.</exception>
    MegFileVersion GetMegFileVersion(string file, out bool encrypted);

    /// <summary>
    /// Retrieves the <see cref="MegFileVersion"/> from a .MEG archive stream.
    /// </summary>
    /// <param name="stream">The .MEG's archive stream</param>
    /// <param name="encrypted">Indicates whether the .MEG archive is encrypted or not.</param>
    /// <returns>The version of the .MEG archive.</returns>
    /// <exception cref="BinaryCorruptedException">The input stream was not recognized as a valid .MEG archive.</exception>
    MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted);
}
