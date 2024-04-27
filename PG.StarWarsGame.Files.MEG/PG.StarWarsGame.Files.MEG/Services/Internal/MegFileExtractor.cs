// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using AnakinRaW.CommonUtilities;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegFileExtractor"/>
/// <summary>
/// Initializes a new instance of the <see cref="MegFileExtractor"/> class.
/// </summary>
/// <param name="services">The service provider.</param>
internal sealed class MegFileExtractor(IServiceProvider services) : ServiceBase(services),  IMegFileExtractor
{
    /// <inheritdoc/>
    public string GetAbsoluteFilePath(IMegDataEntry dataEntry, string rootPath, bool preserveDirectoryHierarchy)
    {
        if (dataEntry is null) 
            throw new ArgumentNullException(nameof(dataEntry));
        ThrowHelper.ThrowIfNullOrWhiteSpace(rootPath);

        var entryPath = dataEntry.FilePath;
        var absoluteRootPath = FileSystem.Path.GetFullPath(rootPath);

        if (!preserveDirectoryHierarchy)
        {
            var fileName = FileSystem.Path.GetFileName(entryPath);
            if (string.IsNullOrEmpty(fileName))
                throw new InvalidOperationException("Empty file names are not allowed!");

            return Path.GetFullPath(Path.Combine(absoluteRootPath, fileName));
        }

        if (FileSystem.Path.IsPathRooted(entryPath))
        {
            // Need to call GetFullPath again cause entryPath may still contain path operators (. or ..)
            return FileSystem.Path.GetFullPath(entryPath);
        }

        return Path.GetFullPath(Path.Combine(absoluteRootPath, dataEntry.FilePath));
    }

    /// <inheritdoc/>
    public MegFileDataStream GetFileData(MegDataEntryLocationReference dataEntryLocation)
    {
        if (dataEntryLocation is null) 
            throw new ArgumentNullException(nameof(dataEntryLocation));
        
        return Services.GetRequiredService<IMegDataStreamFactory>().GetDataStream(dataEntryLocation);
    }

    /// <inheritdoc/>
    public bool ExtractFile(MegDataEntryLocationReference dataEntryLocation, string filePath, bool overwrite)
    {
        if (dataEntryLocation is null)
            throw new ArgumentNullException(nameof(dataEntryLocation));
        ThrowHelper.ThrowIfNullOrWhiteSpace(filePath);

        var fullFilePath = FileSystem.Path.GetFullPath(filePath);

        if (FileSystem.File.Exists(fullFilePath) && !overwrite)
            return false;

        var directory = FileSystem.Path.GetDirectoryName(fullFilePath);
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException("File location does not point to a directory", nameof(filePath));
        FileSystem.Directory.CreateDirectory(directory!);

        var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;

        using var destinationStream = FileSystem.FileStream.New(fullFilePath, fileMode, FileAccess.Write, FileShare.None);

        using var dataStream = Services.GetRequiredService<IMegDataStreamFactory>().GetDataStream(dataEntryLocation);
        dataStream.CopyTo(destinationStream);

        return true;
    }
}