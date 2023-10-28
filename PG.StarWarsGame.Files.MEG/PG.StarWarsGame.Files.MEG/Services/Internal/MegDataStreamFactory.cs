// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Entries;
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

        return GetDataStream(originInfo.MegFileLocation!);
    }

    public Stream GetDataStream(MegFileDataEntryReference dateReference)
    {
        if (dateReference == null) 
            throw new ArgumentNullException(nameof(dateReference));


        if (dateReference.FileEntry.Encrypted)
        {
            throw new NotImplementedException();
        }

        return CreateDataStream(dateReference.MegFile.FilePath,
            dateReference.FileEntry.Location.Offset, dateReference.FileEntry.Location.Size);
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