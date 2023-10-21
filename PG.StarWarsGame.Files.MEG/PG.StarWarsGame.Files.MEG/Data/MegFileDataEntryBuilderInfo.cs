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
    /// 
    /// </summary>
    public MegDataEntryOriginInfo OriginInfo { get; }

    /// <summary>
    /// 
    /// </summary>
    public string? OverrideFileName { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public bool? OverrideEncrypted { get; init; }

    /// <summary>
    ///  Initializes a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from an existing .MEG file.
    /// </summary>
    /// <param name="fileDataEntry"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MegFileDataEntryBuilderInfo(MegFileDataEntry fileDataEntry)
    {
        if (fileDataEntry == null) throw new ArgumentNullException(nameof(fileDataEntry));
        OriginInfo = new MegDataEntryOriginInfo(fileDataEntry);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from a given file.
    /// </summary>
    /// <param name="filePath"></param>
    public MegFileDataEntryBuilderInfo(string filePath)
    {
        if (filePath == null) 
            throw new ArgumentNullException(nameof(filePath));
        OriginInfo = new MegDataEntryOriginInfo(filePath);
    }
}