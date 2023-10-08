// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data;


/// <summary>
/// Container with file information for building .MEG files.
/// </summary>
public sealed class MegFileDataEntryInfo
{
    /// <summary>
    /// Gets file information from a an existing .MEG file
    /// </summary>
    public (IMegFile MegFile, MegFileDataEntry DataEntry, string? OverrideFileName)? MegFileEntry { get; }

    /// <summary>
    /// Gets file information from a local file.
    /// </summary>
    public MegFileDataEntryLocation? LocalFile { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryInfo"/> class from an existing .MEG file.
    /// </summary>
    /// <param name="megFile">The .MEG file holder instance</param>
    /// <param name="megFileDataEntry">The file information from the .MEG file.</param>
    /// <param name="overrideFileName">The new file name which shall be used. When <see langword="null"/> the original file name will be used.</param>
    public MegFileDataEntryInfo(IMegFile megFile, MegFileDataEntry megFileDataEntry, string? overrideFileName = null)
    {
        if (megFile == null)
            throw new ArgumentNullException(nameof(megFile));
        if (megFileDataEntry == null)
            throw new ArgumentNullException(nameof(megFileDataEntry));
        MegFileEntry = (megFile, megFileDataEntry, overrideFileName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryInfo"/> class from a given file.
    /// </summary>
    /// <param name="localFileDataEntry"></param>
    public MegFileDataEntryInfo(MegFileDataEntryLocation localFileDataEntry)
    {
        if (localFileDataEntry == null) 
            throw new ArgumentNullException(nameof(localFileDataEntry));
        LocalFile = localFileDataEntry ?? throw new ArgumentNullException(nameof(localFileDataEntry));
    }
}