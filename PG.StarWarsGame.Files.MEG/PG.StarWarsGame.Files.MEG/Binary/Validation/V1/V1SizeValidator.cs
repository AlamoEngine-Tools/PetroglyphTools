﻿using FluentValidation;
using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation.V1;

internal class V1SizeValidator : AbstractValidator<IMegBinaryValidationInformation>
{
    public V1SizeValidator()
    {
        RuleFor(i => i.BytesRead)
            .Equal(i => i.Metadata.Size);
        RuleFor(i => i.FileSize)
            .Must((i, a) =>
            {
                var totalDataSize = i.Metadata.FileTable.Sum(d => d.FileSize);
                var expectedArchiveSize = i.BytesRead + totalDataSize;
                return expectedArchiveSize == a;
            });
    }
}