// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services;

internal sealed class MegDataStreamFactory : ServiceBase, IMegDataStreamFactory
{
    public MegDataStreamFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public Stream GetDataStream(MegDataEntryOriginInfo originInfo)
    {
        if (originInfo == null) 
            throw new ArgumentNullException(nameof(originInfo));

        if (originInfo.FilePath is not null)
            return FileSystem.File.OpenRead(originInfo.FilePath);

        var megFile = originInfo.MegFileLocation!.MegFile;
        var entry = originInfo.MegFileLocation.DataEntry;

        return GetDataStream(megFile, entry);
    }

    public Stream GetDataStream(IMegFile megFile, MegDataEntry dataEntry)
    {
        if (megFile == null) 
            throw new ArgumentNullException(nameof(megFile));
        if (dataEntry == null) 
            throw new ArgumentNullException(nameof(dataEntry));


        if (dataEntry.Encrypted)
        {
            throw new NotImplementedException();
        }

        return CreateDataStream(megFile.FilePath, dataEntry.Location.Offset, dataEntry.Location.Size);
    }

    private Stream CreateDataStream(string path, uint offset, uint size)
    {
        // Cause MIKE.NL's tool uses the offset megFile[megSize + 1] for empty Entries we would cause an ArgumentOutOfRangeException
        // when trying to access this index on a real file. Therefore we return the Null stream.
        if (size == 0)
            return Stream.Null;

        var fs = Services.GetRequiredService<IFileSystem>();
        return new MegFileDataStream(fs.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read), offset, size);
    }
}