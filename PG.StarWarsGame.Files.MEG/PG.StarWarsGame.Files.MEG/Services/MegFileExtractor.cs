// // Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

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
    public string GetAbsoluteFilePath(MegFileDataEntry dataEntry, string rootPath, bool preserveDirectoryHierarchy)
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
    public Stream GetFileData(IMegFile megFile, MegFileDataEntry dataEntry)
    {
        if (megFile is null) 
            throw new ArgumentNullException(nameof(megFile));
        if (dataEntry is null) 
            throw new ArgumentNullException(nameof(dataEntry));

        if (!megFile.Content.Contains(dataEntry))
            throw new FileNotInMegException(dataEntry.FilePath, megFile.FilePath);

        return Services.GetRequiredService<IMegDataStreamFactory>()
            .CreateDataStream(megFile.FilePath, dataEntry.Offset, dataEntry.Size);
    }

    /// <inheritdoc/>
    public bool ExtractFile(IMegFile megFile, MegFileDataEntry dataEntry, string filePath, bool overwrite)
    {
        if (megFile is null)
            throw new ArgumentNullException(nameof(megFile));
        if (dataEntry is null)
            throw new ArgumentNullException(nameof(dataEntry));

        if (!megFile.Content.Contains(dataEntry))
            throw new FileNotInMegException(dataEntry.FilePath, megFile.FilePath);

        if (FileSystem.File.Exists(filePath) && !overwrite)
            return false;

        // Not sure if this extra barrier is actually necessary. 
        var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;

        using var destinationStream = FileSystem.FileStream.New(filePath, fileMode, FileAccess.Write, FileShare.None);

        using var dataStream = Services.GetRequiredService<IMegDataStreamFactory>().CreateDataStream(megFile.FilePath, dataEntry.Offset, dataEntry.Size);
        dataStream.CopyTo(destinationStream);

        return true;
    }
}