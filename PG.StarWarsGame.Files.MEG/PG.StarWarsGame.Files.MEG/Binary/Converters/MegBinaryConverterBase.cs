// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class MegBinaryConverterBase<TMegMetadata>(IServiceProvider services) 
    : ServiceBase(services), IMegBinaryConverter
    where TMegMetadata : IMegFileMetadata
{
    /// <inheritdoc/>
    public IMegFileMetadata ModelToBinary(IMegArchive model)
    {
        if (model == null) 
            throw new ArgumentNullException(nameof(model));
        return ModelToBinaryCore(model);
    }

    protected abstract TMegMetadata ModelToBinaryCore(IMegArchive model);

    /// <inheritdoc/>
    public IMegArchive BinaryToModel(IMegFileMetadata binary)
    {
        if (binary == null)
            throw new ArgumentNullException(nameof(binary));

        var files = new List<MegDataEntry>(binary.Header.FileNumber);

        // According to the specification: 
        //  - The Meg's FileTable is sorted by CRC32.
        //  - It's not specified how or whether the FileNameTable is sorted.
        //  - The game merges FileTable entries (and skips duplicates in the same file.)
        //  --> In theory:  For file entries with the same file name (and thus same CRC32),
        //                  the game uses the first file in the list.
        // 
        // Since an IMegFile expects a List<>, not a Collection<>, we have to preserve the order of the FileTable
        var lastCrc = new Crc32(0);
        for (var i = 0; i < binary.Header.FileNumber; i++)
        {
            var fileDescriptor = binary.FileTable[i];
            var crc = fileDescriptor.Crc32;

            if (crc < lastCrc)
                throw new BinaryCorruptedException("File Table is not sorted by CRC32.");
            lastCrc = crc;

            var fileOffset = fileDescriptor.FileOffset;
            var fileSize = fileDescriptor.FileSize;
            var fileNameIndex = fileDescriptor.FileNameIndex;
            var fileName = binary.FileNameTable[fileNameIndex].FileName;
            var originalFilePath = binary.FileNameTable[fileNameIndex].OriginalFilePath;

            var location = new MegDataEntryLocation(fileOffset, fileSize);

            files.Add(new MegDataEntry(fileName, crc, location, fileDescriptor.Encrypted, originalFilePath));
        }
        return new MegArchive(files);
    }
}