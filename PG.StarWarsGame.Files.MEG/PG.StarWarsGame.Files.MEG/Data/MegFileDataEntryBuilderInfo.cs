// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Container with file information for building .MEG files.
/// </summary>
public sealed class MegFileDataEntryBuilderInfo
{
    /// <summary>
    /// The actual location of a MEG data entry file.
    /// </summary>
    public MegDataEntryOriginInfo OriginInfo { get; }

    /// <summary>
    /// Gets the file name to be used instead of the original file name. <see langword="null"/> if no custom file name shall be used.
    /// </summary>
    /// <exception cref="ArgumentException">If <paramref name="value"/>is empty or only whitespace.</exception>
    public string? OverrideFileName { get; init; }

    /// <summary>
    /// Gets whether the data entry file shall be encrypted or not. <see langword="null"/> if the original data entry's encryption state shall be used.
    /// </summary>
    public bool? OverrideEncrypted { get; init; }

    /// <summary>
    ///  Initializes a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from an existing .MEG file.
    /// </summary>
    /// <param name="originInfo">The data entry's origin.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="originInfo"/> is <see langword="null"/>.</exception>
    public MegFileDataEntryBuilderInfo(MegDataEntryOriginInfo originInfo)
    {
        OriginInfo = originInfo ?? throw new ArgumentNullException(nameof(originInfo));
    }
}