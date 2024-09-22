// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.FileSystem;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
///  Validates a MEG file information whether it is compliant to the Petroglyph game Empire at War. 
/// </summary>
public sealed class EmpireAtWarMegFileInformationValidator : PetroglyphMegFileInformationValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegFileInformationValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EmpireAtWarMegFileInformationValidator(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override MegFileInfoValidationResult Validate(MegBuilderFileInformationValidationData infoValidationData)
    {
        var baseResult = base.Validate(infoValidationData);
        if (!baseResult.IsValid)
            return baseResult;

        if (infoValidationData.FileInformation.HasEncryption || infoValidationData.FileInformation.FileVersion != MegFileVersion.V1)
            return MegFileInfoValidationResult.FromFailed("File version must be V1.");

        try
        {
            // As we cannot know the actual path on the target system where the game will be installed,
            // it does not make sense to check the full path. Instead, we just check for the file name whether that's valid.
            var fileName = FileSystem.Path.GetFileName(infoValidationData.FileInformation.FilePath.AsSpan());
            if (!FileNameUtilities.IsValidFileName(fileName, out var result))
                return MegFileInfoValidationResult.FromFailed($"File name is not valid: '{result}'");
            return MegFileInfoValidationResult.Valid;
        }
        catch (Exception e)
        {
            return MegFileInfoValidationResult.FromFailed($"Validation failed with {e.Message}");
        }
    }
}