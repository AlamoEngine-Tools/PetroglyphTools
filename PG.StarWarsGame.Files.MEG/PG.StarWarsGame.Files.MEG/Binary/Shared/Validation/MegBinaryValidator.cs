// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal sealed class MegBinaryValidator : AbstractValidator<IMegBinaryValidationInformation>, IMegBinaryValidator
{
    public MegBinaryValidator(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        // We cannot add a validator that checks whether the string file name matches the CRC32,
        // cause that would cause incompatibility with MIKE's tool as he allows non-ASCII chars while we do not.
        // This of course makes Mike's meg technically invalid, be we should allow situation.
        // Tool's are free to handle this on their own.

        RuleFor(i => i.Metadata.FileTable)
            .SetValidator(serviceProvider.GetRequiredService<IFileTableValidator>());
           // .WithMessage("The meg's file table is invalid.");

           RuleFor(i => i)
               .SetValidator(serviceProvider.GetRequiredService<IMegFileSizeValidator>());
           //.WithMessage("The read bytes do not match expected size.");


    }
}