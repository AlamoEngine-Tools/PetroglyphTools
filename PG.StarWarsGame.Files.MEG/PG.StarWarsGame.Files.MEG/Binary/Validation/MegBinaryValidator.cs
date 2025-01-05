// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal sealed class MegBinaryValidator(IServiceProvider serviceProvider) : IMegBinaryValidator
{
    private readonly IFileTableValidator _fileTableValidator = serviceProvider.GetRequiredService<IFileTableValidator>();
    private readonly IMegFileSizeValidator _sizeValidator = serviceProvider.GetRequiredService<IMegFileSizeValidator>();

    public bool Validate(MegBinaryValidationInformation info)
    {
        // We cannot add a validator that checks whether the string file name matches the CRC32,
        // because that would cause incompatibility with MIKE's tool as he allows non-ASCII chars while we do not.
        // This of course makes Mike's meg technically invalid, be we should allow situation.
        // Tool's are free to handle this on their own.

        return _sizeValidator.Validate(info) && _fileTableValidator.Validate(info.Metadata.FileTable);
    }
}