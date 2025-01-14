// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
#if NETSTANDARD2_0 || NETFRAMEWORK
using AnakinRaW.CommonUtilities.FileSystem;
#endif

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Base class for a Petroglyph MEG file information validator.
/// </summary>
public abstract class PetroglyphMegFileInformationValidator : IMegFileInformationValidator
{
    // The game arbitrary varies between 260 and 256, so we chose the larger value here. Mind that the value is 260 - 1,
    // because we need to reserve one byte for the zero-terminator '\0'.
    /// <summary>
    /// The max number of characters allowed in a PG game for file paths.
    /// </summary>
    protected const int PetroglyphMaxFilePathLength = 259;

    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    private protected PetroglyphMegFileInformationValidator(IServiceProvider serviceProvider)
    {
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    }

    /// <inheritdoc />
    public virtual MegFileInfoValidationResult Validate(MegBuilderFileInformationValidationData infoValidationData)
    {
        if (!DefaultMegFileInformationValidator.Instance.Validate(infoValidationData).IsValid)
            return MegFileInfoValidationResult.Failed;

        var fileName = FileSystem.Path.GetFileName(infoValidationData.FileInformation.FilePath.AsSpan());
        return fileName.Length <= PetroglyphMaxFilePathLength
            ? MegFileInfoValidationResult.Valid
            : MegFileInfoValidationResult.FromFailed("File path is too long.");
    }
}