// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class MegFileSizeValidatorBase<T> : ServiceBase, IMegFileSizeValidator where T : IMegFileMetadata
{
    protected MegFileSizeValidatorBase(IServiceProvider services) : base(services)
    {
    }

    public bool Validate(long bytesRead, long archiveSize, IMegFileMetadata metadata)
    {
        if (metadata is null) 
            throw new ArgumentNullException(nameof(metadata));
        if (metadata is not T tMetadata)
            throw new InvalidCastException($"Metadata is not of the expected type '{typeof(T)}'.");
        if (bytesRead < 0)
            return false;
        if (archiveSize < 0)
            return false;
        if (archiveSize < bytesRead)
            return false;
        return ValidateCore(bytesRead, archiveSize, tMetadata);
    }

    protected internal abstract bool ValidateCore(long bytesRead, long archiveSize, T metadata);
}