// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a <see cref="MegFileDataEntryBuilderInfo"/> whether it is compliant to a Petroglyph game.
/// </summary>
public abstract class PetroglyphMegBuilderDataEntryValidator : IMegBuilderInfoValidator
{
    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphMegBuilderDataEntryValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected PetroglyphMegBuilderDataEntryValidator(IServiceProvider serviceProvider)
    {
       FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    }


    /// <inheritdoc />
    public bool Validate(MegFileDataEntryBuilderInfo? builderInfo)
    {
        return builderInfo is not null && Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size);
    }

    /// <inheritdoc />
    public abstract bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size);
}