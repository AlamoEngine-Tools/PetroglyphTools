// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal class MegFileTableValidator : AbstractValidator<IMegFileTable>, IFileTableValidator
{
    public MegFileTableValidator()
    {
        RuleFor(t => t).Must(t =>
        {
            var lastCrc = new Crc32(0);
            for (var i = 0; i < t.Count; i++)
            {
                var descriptor = t[i];
                if (descriptor.Index != i)
                    return false;
                if (descriptor.Crc32 < lastCrc)
                    return false;
                lastCrc = descriptor.Crc32;
            }
            return true;
        });
    }
}