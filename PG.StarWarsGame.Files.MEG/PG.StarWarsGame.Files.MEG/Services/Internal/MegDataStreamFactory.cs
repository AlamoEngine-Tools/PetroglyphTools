// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services;

internal sealed class MegDataStreamFactory(IServiceProvider serviceProvider)
    : ServiceBase(serviceProvider), IMegDataStreamFactory
{
    public Stream GetStream(MegDataEntryOriginInfo originInfo)
    {
        if (originInfo == null) 
            throw new ArgumentNullException(nameof(originInfo));

        if (originInfo.FileInfo is not null)
            return originInfo.FileInfo.OpenRead();

        return GetStream(originInfo.MegFileLocation!);
    }

    public MegEntryStream GetStream(MegDataEntryLocationReference locationReference)
    {
        if (locationReference == null) 
            throw new ArgumentNullException(nameof(locationReference));

        if (!locationReference.Exists)
            throw new EntryNotInMegException(locationReference);

        if (locationReference.DataEntry.Encrypted)
        {
            throw new NotImplementedException("Encrypted archives are currently not supported");
        }

        return CreateStream(locationReference.MegFile.FilePath, locationReference.DataEntry);
    }

    private MegEntryStream CreateStream(string megFilePath, MegDataEntry entry)
    {
        if (!FileSystem.File.Exists(megFilePath))
            throw new FileNotFoundException($"MEG file '{megFilePath}' does not exist", megFilePath);

        // Cause MIKE.NL's tool uses the offset megFile[megSize + 1] for empty Entries we would cause an ArgumentOutOfRangeException
        // when trying to access this index on a real file. Therefore, we return the Null stream.
        if (entry.Location.Size == 0)
            return MegEntryStream.CreateEmptyStream(entry.Path);

        var fs = Services.GetRequiredService<IFileSystem>();

        var megFileStream = fs.FileStream.New(megFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new MegEntryStream(entry.Path, megFileStream, entry.Location.Offset, entry.Location.Size);
    }
}