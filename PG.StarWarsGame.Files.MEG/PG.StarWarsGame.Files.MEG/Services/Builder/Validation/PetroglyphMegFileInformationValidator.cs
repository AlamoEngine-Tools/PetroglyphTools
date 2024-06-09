// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.FileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Base class for a Petroglyph MEG file information validator.
/// </summary>
public abstract class PetroglyphMegFileInformationValidator : IMegFileInformationValidator
{
    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphMegFileInformationValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected PetroglyphMegFileInformationValidator(IServiceProvider serviceProvider)
    {
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    }

    /// <inheritdoc />
    public virtual MegFileInfoValidationResult Validate(MegBuilderFileInformationValidationData infoValidationData)
    {
        if (!DefaultMegFileInformationValidator.Instance.Validate(infoValidationData).IsValid)
            return MegFileInfoValidationResult.Failed;

        var fileName = FileSystem.Path.GetFileName(infoValidationData.FileInformation.FilePath.AsSpan());
        return fileName.Length <= 260
            ? MegFileInfoValidationResult.Valid
            : MegFileInfoValidationResult.FromFailed("File path is too long.");
    }
}