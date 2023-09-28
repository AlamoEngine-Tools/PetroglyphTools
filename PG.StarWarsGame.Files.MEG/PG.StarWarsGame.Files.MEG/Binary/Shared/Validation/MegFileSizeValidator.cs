// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using FluentValidation;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal class MegFileSizeValidator : AbstractValidator<IMegSizeValidationInformation<IMegFileMetadata>>
{
    public MegFileSizeValidator()
    {
        RuleFor(i => i.Metadata).NotNull();
        RuleFor(i => i.BytesRead).GreaterThan(0);
        RuleFor(i => i.ArchiveSize).GreaterThan(0);
        RuleFor(i => i.ArchiveSize).GreaterThanOrEqualTo(i => i.BytesRead);

        RuleFor(x => x).SetInheritanceValidator(v =>
        {
            v.Add(new V1Validator());
        });
    }

    private class V1Validator : AbstractValidator<IMegSizeValidationInformation<MegMetadata>>
    {
        public V1Validator()
        {
            RuleFor(i => i.BytesRead).Equal(i => i.Metadata.Size);
            RuleFor(i => i.ArchiveSize).Must((i, a) => 
            {
                var totalDataSize = ((IMegFileTable)i.Metadata.FileTable).Sum(d => d.FileSize);
                var expectedArchiveSize = i.BytesRead + totalDataSize;
                return expectedArchiveSize == a;
            });
        }
    }
}