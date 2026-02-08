// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary.Size;
#if NETSTANDARD2_0
using AnakinRaW.CommonUtilities.FileSystem;
#endif

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
///  Validates a MEG file information whether it is compliant to a Petroglyph Star Wars game. 
/// </summary>
public sealed class EmpireAtWarMegFileInformationValidator : BinaryMegFileInformationValidator
{
    // The game arbitrary varies between 260 and 256, so we chose the larger value here. Mind that the value is 260 - 1,
    // because we need to reserve one byte for the zero-terminator '\0'.
    /// <summary>
    /// The max number of characters allowed in a PG game for file paths.
    /// </summary>
    public const int PetroglyphMaxFilePathLength = 259;

    /// <summary>
    /// Gets the collection of supported <see cref="MegFileVersion"/> values for the validator.
    /// </summary>
    /// <value>
    /// A read-only collection containing the supported versions of the .MEG file format
    /// that are validated by this implementation.
    /// For <see cref="EmpireAtWarMegFileInformationValidator"/>, this includes only <see cref="MegFileVersion.V1"/>.
    /// </value>
    protected override IReadOnlyCollection<MegFileVersion> SupportedVersions { get; } = [MegFileVersion.V1];

    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Gets the maximum allowed size, in bytes, for a MEG file when validating 
    /// compliance with the Petroglyph Star Wars: Empire at War and Forces of Corruption game.
    /// </summary>
    /// <value>
    /// The maximum allowed size, in bytes, for a MEG file when
    /// validating compliance with the Petroglyph Star Wars: Empire at War and Forces of Corruption game.
    /// Maximum file size is limited to 2GB (2^32 - 1 bytes).
    /// </value>
    protected override uint MaxMegFileSize { get; } =
        MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.EawFoc).MaxFileSize;

    /// <summary>
    /// Gets the maximum allowed size, in bytes, for a MEG data entry when validating 
    /// compliance with the Petroglyph Star Wars: Empire at War and Forces of Corruption game.
    /// </summary>
    /// <value>
    /// The maximum allowed size, in bytes, for a MEG data entry when
    /// validating compliance with the Petroglyph Star Wars: Empire at War and Forces of Corruption game.
    /// Maximum entry size is limited to 2GB (2^32 - 1 bytes).
    /// </value>
    protected override uint MaxMegEntrySize { get; } =
        MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.EawFoc).MaxEntrySize;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegFileInformationValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EmpireAtWarMegFileInformationValidator(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _fileSystem = ServiceProvider.GetRequiredService<IFileSystem>();
    }

    /// <remarks>
    /// This method performs several checks to ensure the validity of the MEG data entry:
    /// <list type="bullet">
    /// <item>Ensures the file version is V1.</item>
    /// <item>The file name does not contain any illegal characters and is not too long.</item>
    /// </list>
    /// </remarks>
    /// <inheritdoc />
    protected override MegFileInfoValidationResult ValidateCore(MegFileInformation fileInformation,
        IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries)
    {
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