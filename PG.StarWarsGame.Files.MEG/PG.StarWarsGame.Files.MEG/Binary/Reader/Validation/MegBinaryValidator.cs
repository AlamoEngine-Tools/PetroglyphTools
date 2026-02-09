// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using System;
using System.Linq;

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

        var fileTable = metadata.FileTable;
        var fileNameCount = metadata.FileNameTable.Count;

        var totalDataSize = 0L;
        var currentOffset = actualMetadataSize;
        var fileNameIndices = new bool[fileNameCount];

        // The reader, which is calling the validation, already assured that
        // - the file records are sorted by CRC
        // - the written file record index matches the actual position in the MEG

        foreach (var record in fileTable.OrderBy(r => r.FileOffset))
        {
            totalDataSize += record.FileSize;

            if (record.FileOffset < actualMetadataSize)
                throw new BinaryCorruptedException($"The content of file record ({record.Crc32}) starts within the metadata.");

            if (record.FileOffset < currentOffset)
                throw new BinaryCorruptedException($"The content of file record ({record.Crc32}) overlaps with the content of a previous record.");

            currentOffset = record.FileOffset + record.FileSize;

            var fileNameIndex = record.FileNameIndex;
            if ((uint)fileNameIndex >= (uint)fileNameCount)
                throw new BinaryCorruptedException($"File record ({record.Crc32}) has an out-of-range filename index: {fileNameIndex}.");

            if (fileNameIndices[fileNameIndex])
                throw new BinaryCorruptedException($"File record ({record.Crc32}) has a duplicate filename index: {fileNameIndex}.");

            fileNameIndices[fileNameIndex] = true;
        }

        if (actualMetadataSize + totalDataSize != actualFileSize)
            throw new BinaryCorruptedException("The size of the MEG file does not match the expected file size.");

        // We cannot validate whether the file names in file name table actually match the CRC32 of the file record table,
        // because that would cause incompatibility with MIKE's tool as he allows non-ASCII chars while we do not.
        // This of course makes Mike's meg technically invalid, be we should allow situation.
        // Tools are free to handle this on their own.
    }
}