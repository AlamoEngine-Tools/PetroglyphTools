// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
#if NETSTANDARD2_0
using AnakinRaW.CommonUtilities.FileSystem;
#endif

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
///  Validates a MEG file information whether it is compliant to a Petroglyph Star Wars game. 
/// </summary>
public sealed class EmpireAtWarMegFileInformationValidator : BinaryMegFileInformationValidator
{
    private readonly IFileSystem _fileSystem;

    // The game arbitrary varies between 260 and 256, so we chose the larger value here. Mind that the value is 260 - 1,
    // because we need to reserve one byte for the zero-terminator '\0'.
    /// <summary>
    /// The max number of characters allowed in a PG game for file paths.
    /// </summary>
    public const int PetroglyphMaxFilePathLength = 259;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegFileInformationValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EmpireAtWarMegFileInformationValidator(IServiceProvider serviceProvider)
    {
        _fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    }

    /// <remarks>
    /// This method performs several checks to ensure the validity of the MEG data entry:
    /// <list type="bullet">
    /// <item>Ensures the file version is V1.</item>
    /// <item>The file name does not contain any illegal characters and is not too long.</item>
    /// </list>
    /// </remarks>
    /// <inheritdoc />
    protected override MegFileInfoValidationResult ValidateCore(MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries)
    {
        if (fileInformation.FileVersion != MegFileVersion.V1)
            return new MegFileInfoValidationResult(false, "File version must be V1.");

        var fileName = _fileSystem.Path.GetFileName(fileInformation.FilePath.AsSpan());

        // As we cannot know the actual path on the target system where the game will be installed,
        // it does not make sense to check the full path. Instead, we just check for the file name whether that's valid.
        if (fileName.Length > PetroglyphMaxFilePathLength)
            return new MegFileInfoValidationResult(false, "File path is too long.");
        return !PGFileNameUtilities.IsValidFileName(fileName, out var result)
            ? new MegFileInfoValidationResult(false, $"File name is not valid: '{result}'")
            : MegFileInfoValidationResult.Valid;
    }

}