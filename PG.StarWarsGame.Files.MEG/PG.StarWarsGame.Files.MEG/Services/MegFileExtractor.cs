// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegFileExtractor"/>
public sealed class MegFileExtractor : ServiceBase,  IMegFileExtractor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileExtractor"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    public MegFileExtractor(IServiceProvider services) : base(services)
    {
    }


    /// <inheritdoc/>
    public string GetAbsoluteFilePath(IMegDataEntry dataEntry, string rootPath, bool preserveDirectoryHierarchy)
    {
        if (dataEntry is null) 
            throw new ArgumentNullException(nameof(dataEntry));
        if (rootPath is null) 
            throw new ArgumentNullException(nameof(rootPath));
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentException(nameof(rootPath));

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
    public Stream GetFileData(MegDataEntryReferenceLocation dataEntryLocation)
    {
        if (dataEntryLocation is null) 
            throw new ArgumentNullException(nameof(dataEntryLocation));

        var megFile = dataEntryLocation.MegFile;
        var dataEntry = dataEntryLocation.DataEntry;

        if (!FileSystem.File.Exists(megFile.FilePath))
            throw new FileNotFoundException("MEG file not found.", megFile.FilePath);

        if (!megFile.Content.Contains(dataEntry))
            throw new FileNotInMegException(dataEntry.FilePath, megFile.FilePath);

        return Services.GetRequiredService<IMegDataStreamFactory>()
            .GetDataStream(megFile, dataEntry);
    }

    /// <inheritdoc/>
    public bool ExtractFile(MegDataEntryReferenceLocation dataEntryLocation, string filePath, bool overwrite)
    {
        if (dataEntryLocation is null)
            throw new ArgumentNullException(nameof(dataEntryLocation));
        if (filePath is null)
            throw new ArgumentNullException(nameof(filePath));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path must not be empty or contain only whitespace", nameof(filePath));

        var megFile = dataEntryLocation.MegFile;
        var dataEntry = dataEntryLocation.DataEntry;

        if (!FileSystem.File.Exists(megFile.FilePath))
            throw new FileNotFoundException("MEG file not found.", megFile.FilePath);

        if (!megFile.Content.Contains(dataEntry))
            throw new FileNotInMegException(dataEntry.FilePath, megFile.FilePath);

        var fullFilePath = FileSystem.Path.GetFullPath(filePath);

        if (FileSystem.File.Exists(fullFilePath) && !overwrite)
            return false;

        var directory = FileSystem.Path.GetDirectoryName(fullFilePath);
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException("File location does not point to a directory", nameof(filePath));
        FileSystem.Directory.CreateDirectory(directory!);

        var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;

        using var destinationStream = FileSystem.FileStream.New(fullFilePath, fileMode, FileAccess.Write, FileShare.None);

        using var dataStream = Services.GetRequiredService<IMegDataStreamFactory>()
            .GetDataStream(megFile, dataEntry);
        dataStream.CopyTo(destinationStream);

        return true;
    }
}