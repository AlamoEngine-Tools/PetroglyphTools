// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal sealed class V1MegValidator(IServiceProvider serviceProvider) : MegBinaryValidator<MegMetadata>(serviceProvider)
{
    public override void Validate(MegMetadata metadata, long actualMetadataSize, long actualFileSize)
    {
        base.Validate(metadata, actualMetadataSize, actualFileSize);
        if (actualMetadataSize != metadata.Size)
            throw new BinaryCorruptedException("The size of the metadata does not match the actual read data.");

        var totalDataSize = metadata.FileTable.Sum<MegFileTableRecord>(d => d.FileSize);
        var expectedArchiveSize = actualMetadataSize + totalDataSize;
        
        if (expectedArchiveSize != actualFileSize)
            throw new BinaryCorruptedException("The size of the MEG file does not match the expected file size.");

        // NB: Since this validator is used for READING MEG files only, we do not validate if the MEG file is larger than 2GB here.
        // This allows this library to read larger MEG files and possible re-create them into smaller MEG files.
    }
}