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

    public bool Validate(long bytesRead, IMegFileMetadata metadata)
    {
        if (metadata is not T tMetadata)
            throw new InvalidCastException($"Metadata is not of the expected type '{typeof(T)}'.");
        return ValidateCore(bytesRead, tMetadata);
    }

    protected abstract bool ValidateCore(long bytesRead, T metadata);
}