// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
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

        var totalDataSize = 0L;
        var fileNameIndices = new bool[metadata.FileNameTable.Count];
        var lastCrc = default(Crc32);

        for (var i = 0; i < metadata.FileTable.Count; i++)
        {
            var record = metadata.FileTable[i];

            totalDataSize += record.FileSize;

            if (record.Crc32 < lastCrc)
                throw new BinaryCorruptedException("The MEG file table is not sorted by CRC32.");

            lastCrc = record.Crc32;

            if (record.Index != i)
                throw new BinaryCorruptedException($"The file record at index {i} has a mismatched index property: {record.Index}.");

            if (record.FileNameIndex < 0 || record.FileNameIndex >= metadata.FileNameTable.Count)
                throw new BinaryCorruptedException($"The file record at index {i} has an out-of-range filename index: {record.FileNameIndex}.");

            if (fileNameIndices[record.FileNameIndex])
                throw new BinaryCorruptedException($"The file record at index {i} has a duplicate filename index: {record.FileNameIndex}.");
            
            fileNameIndices[record.FileNameIndex] = true;
        }

        var expectedArchiveSize = actualMetadataSize + totalDataSize;

        if (expectedArchiveSize != actualFileSize)
            throw new BinaryCorruptedException("The size of the MEG file does not match the expected file size.");

        // We cannot validate whether the file names in file name table actually match the CRC32 of the file record table,
        // because that would cause incompatibility with MIKE's tool as he allows non-ASCII chars while we do not.
        // This of course makes Mike's meg technically invalid, be we should allow situation.
        // Tools are free to handle this on their own.
    }
}