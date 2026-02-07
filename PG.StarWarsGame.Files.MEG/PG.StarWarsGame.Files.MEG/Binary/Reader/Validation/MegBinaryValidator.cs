// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal abstract class MegBinaryValidator<TMetadata>(IServiceProvider serviceProvider) 
    : IMegBinaryValidator<TMetadata> where TMetadata : IMegFileMetadata
{
    protected readonly IServiceProvider ServiceProvider = serviceProvider;

    public virtual void Validate(TMetadata metadata, long actualMetadataSize, long actualFileSize)
    {
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        if (actualMetadataSize != metadata.Size)
            throw new BinaryCorruptedException("The size of the metadata does not match the actual read data.");

        var totalDataSize = metadata.FileTable.Sum(d => d.FileSize);
        var expectedArchiveSize = actualMetadataSize + totalDataSize;

        if (expectedArchiveSize != actualFileSize)
            throw new BinaryCorruptedException("The size of the MEG file does not match the expected file size.");

        var sortedRecords = metadata.FileTable.OrderBy(r => r.FileOffset).ToList();

        var currentOffset = actualMetadataSize;
        foreach (var record in sortedRecords)
        {
            if (record.FileOffset != currentOffset)
            {
                if (record.FileOffset < currentOffset)
                    throw new BinaryCorruptedException("The MEG file has overlapping entries.");
                throw new BinaryCorruptedException("The MEG file has gaps between entries.");
            }

            currentOffset += record.FileSize;
        }

        // We cannot validate whether the file names in file name table actually match the CRC32 of the file record table,
        // because that would cause incompatibility with MIKE's tool as he allows non-ASCII chars while we do not.
        // This of course makes Mike's meg technically invalid, be we should allow situation.
        // Tools are free to handle this on their own.
    }
}