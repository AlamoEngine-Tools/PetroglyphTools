// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// Service for extracting file from a .MEG archive.
/// </summary>
public interface IMegFileExtractor
{
    /// <summary>
    /// Builds the absolute file path for a <see cref="MegFileDataEntry"/> and a given base directory.
    /// </summary>
    /// <remarks>
    /// The following rules are applied for building the returned path:
    /// <br/>
    /// <br/>
    ///
    /// If <paramref name="preserveDirectoryHierarchy"/> is <see langword="false"/>,
    /// <code>
    ///     result :=  <paramref name="targetDirectory"/> + filename(<paramref name="dataEntry"/>).
    /// </code>
    /// <br/>
    /// <br/>
    ///
    /// If <paramref name="preserveDirectoryHierarchy"/> is <see langword="true"/>,
    /// <br/>
    /// a) if <paramref name="dataEntry"/> has a relative file path
    /// <br/>
    /// <code>
    ///     result := <paramref name="targetDirectory"/> + path(<paramref name="dataEntry"/>).
    /// </code>
    /// 
    /// <br/>
    /// b) if <paramref name="dataEntry"/> has an absolute file path
    /// <code>
    ///     result := path(<paramref name="dataEntry"/>)
    /// </code> 
    /// </remarks>
    /// <param name="dataEntry">The file to get the path from.</param>
    /// <param name="targetDirectory">Base directory of the built file path.</param>
    /// <param name="preserveDirectoryHierarchy">option to preserve the directory hierarchy of the <paramref name="dataEntry"/> file name.</param>
    /// <param name="absoluteEntry">Indicates whether <paramref name="dataEntry"/> has an absolute file name.</param>
    /// <returns>The absolute file path.</returns>
    string GetAbsoluteFilePath(MegFileDataEntry dataEntry, string targetDirectory, bool preserveDirectoryHierarchy, out bool absoluteEntry);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="megFile"></param>
    /// <param name="dataEntry"></param>
    /// <returns></returns>
    Stream ExtractFile(IMegFile megFile, MegFileDataEntry dataEntry);


    /// <summary>
    /// Extracts a single file from a .MEG archive to a given location.
    /// Existing files will be overwritten.
    /// </summary>
    /// <param name="megFile">The .MEG file.</param>
    /// <param name="dataEntry">The file to extract.</param>
    /// <param name="targetDirectory">Directory to extract the file to.</param>
    /// <param name="preserveDirectoryHierarchy">
    /// If set to <see langword="false"/>, any directory structure within the meg file will be disregarded.
    /// </param>
    /// <exception cref="FileNotInMegException">When <paramref name="dataEntry"/> is not in the .MEG file.</exception>
    /// <exception cref="InvalidOperationException">
    /// If <paramref name="dataEntry"/> has an absolute path and <paramref name="preserveDirectoryHierarchy"/>
    /// is set to <see langowrd="true"/>.
    /// </exception>
    void ExtractFile(IMegFile megFile, MegFileDataEntry dataEntry, string targetDirectory, bool preserveDirectoryHierarchy = true);


    /// <summary>
    /// Extracts all files in a .MEG file to a given location.
    /// Existing files will be overwritten.
    /// </summary>
    /// <remarks>
    /// When <paramref name="preserveDirectoryHierarchy"/> is <see langword="true"/>,
    /// any file with an absolute file name inside the .MEG archive will be skipped. 
    /// </remarks>
    /// <param name="megFile">The .MEG file.</param>
    /// <param name="targetDirectory">Directory to extract the file to.</param>
    /// <param name="preserveDirectoryHierarchy">
    /// If set to <see langword="false"/>, any directory structure within the meg file will be disregarded.
    /// </param>
    void ExtractAllFiles(IMegFile megFile, string targetDirectory, bool preserveDirectoryHierarchy = true);
}