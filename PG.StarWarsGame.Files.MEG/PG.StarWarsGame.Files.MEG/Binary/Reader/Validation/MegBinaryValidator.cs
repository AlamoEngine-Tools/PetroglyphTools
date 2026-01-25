// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
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
        // We cannot validate whether the file names in file name table actually match the CRC32 of the file record table,
        // because that would cause incompatibility with MIKE's tool as he allows non-ASCII chars while we do not.
        // This of course makes Mike's meg technically invalid, be we should allow situation.
        // Tools are free to handle this on their own.
    }
}