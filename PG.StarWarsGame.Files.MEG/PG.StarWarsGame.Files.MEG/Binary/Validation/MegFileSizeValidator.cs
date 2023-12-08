// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation.V1;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal sealed class MegFileSizeValidator : AbstractValidator<IMegBinaryValidationInformation>, IMegFileSizeValidator
{
    public MegFileSizeValidator()
    {
        RuleFor(i => i.Metadata).NotNull();
        RuleFor(i => i.BytesRead).GreaterThan(0);
        RuleFor(i => i.FileSize).GreaterThan(0);
        RuleFor(i => i.FileSize).GreaterThanOrEqualTo(i => i.BytesRead);

        // We cannot use SetInheritanceValidator cause we need the full information in the sub validator.
        When(x => x.Metadata is MegMetadata, () =>
        {
            RuleFor(x => x).SetValidator(new V1SizeValidator());
            // TODO: Other Binary Versions
        });
    }
}