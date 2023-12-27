// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// Service for extracting file from a .MEG archive.
/// </summary>
public interface IMegFileExtractor
{
    /// <summary>
    /// Builds the absolute file path for a <see cref="IMegDataEntry"/> and a given base directory.
    /// Use this method to get an absolute path for extracting an entry. 
    /// <br/>
    /// If <paramref name="rootPath"/> is relative, the environment's <see cref="Environment.CurrentDirectory"/> will be prepended.
    /// </summary>
    /// <remarks>
    /// The following rules are applied for building the returned path:
    /// <br/>
    /// <br/>
    ///
    /// If <paramref name="preserveDirectoryHierarchy"/> is <see langword="false"/>,
    /// <code>
    ///     result :=  <paramref name="rootPath"/> + filename(<paramref name="dataEntry"/>).
    /// </code>
    /// <br/>
    /// <br/>
    ///
    /// If <paramref name="preserveDirectoryHierarchy"/> is <see langword="true"/>,
    /// <br/>
    /// a) if <paramref name="dataEntry"/> is not <em>rooted</em> <see href="https://learn.microsoft.com/en-us/dotnet/api/system.io.path.ispathrooted">(see here)</see>
    /// <br/>
    /// <code>
    ///     result := <paramref name="rootPath"/> + path(<paramref name="dataEntry"/>).
    /// </code>
    /// 
    /// <br/>
    /// b) if <paramref name="dataEntry"/> is rooted.
    /// <code>
    ///     result := path(<paramref name="dataEntry"/>)
    /// </code>
    ///
    /// <br/>
    /// 
    /// This method does not ensure whether the returned path is valid on the current file system e.g, containing illegal characters.
    ///
    /// <br/>
    /// <br/>
    /// 
    /// <b>Security Note:</b>
    /// This method does <b>not</b> ensure whether the returned absolute path leaves <paramref name="rootPath"/>, nor the returned path is valid on the target system.
    /// It's the consumers responsibility to prevent path traversals.
    /// </remarks>
    /// <param name="dataEntry">The file to get the path from.</param>
    /// <param name="rootPath">Base directory of the built file path.</param>
    /// <param name="preserveDirectoryHierarchy">option to preserve the directory hierarchy of the <paramref name="dataEntry"/> file name.</param>
    /// <returns>The absolute file path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dataEntry"/> or <paramref name="rootPath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="rootPath"/> is empty or contains only whitespace.</exception>
    /// <exception cref="InvalidOperationException">The absolute path could not be determined.</exception>
    string GetAbsoluteFilePath(IMegDataEntry dataEntry, string rootPath, bool preserveDirectoryHierarchy);


    /// <summary>
    /// Gets the data stream of the given <paramref name="dataEntryLocation"/>.
    /// </summary>
    /// <param name="dataEntryLocation">The data entry information.</param>
    /// <returns>A stream containing the files contents.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dataEntryLocation"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="dataEntryLocation"/> has <see langword="null"/> properties.</exception>
    /// <exception cref="FileNotInMegException">The data entry does not exist in the .MEG file.</exception>
    /// <exception cref="UnauthorizedAccessException">The operation is not permitted by the operating system due to missing permissions.</exception>
    Stream GetFileData(MegDataEntryLocationReference dataEntryLocation);


    /// <summary>
    /// Extracts a single file from a .MEG archive to a given location.
    /// </summary>
    /// <remarks>
    /// All necessary folders will be created automatically.
    /// <br/>
    /// <br/>
    /// <b>Note: </b><paramref name="filePath"/> will first be resolved by <see cref="Path.GetFullPath(string)"/>. File path information of the data entry will not be used.
    /// <br/>
    /// Use <see cref="GetAbsoluteFilePath"/> to create a <see cref="IMegDataEntry"/>-based absolute path.
    /// </remarks>
    /// <param name="dataEntryLocation">The data entry information.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="overwrite">When set to <see langword="true"/> existing files will be overwritten; otherwise the extraction will be skipped.</param>
    /// <returns><see langword="true"/> if the file was extracted. <see langword="false"/> if and only if the extraction was skipped.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dataEntryLocation"/> or <paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty, contains only whitespace or is not a legal file path in general.</exception>
    /// <exception cref="ArgumentException"><paramref name="dataEntryLocation"/> has <see langword="null"/> properties.</exception>
    /// <exception cref="FileNotInMegException">The data entry does not exist in the .MEG file.</exception>
    /// <exception cref="IOException">Data extraction failed due to an IO error.</exception>
    /// <exception cref="UnauthorizedAccessException">The operation is not permitted by the operating system due to missing permissions.</exception>
    bool ExtractFile(MegDataEntryLocationReference dataEntryLocation, string filePath, bool overwrite);
}