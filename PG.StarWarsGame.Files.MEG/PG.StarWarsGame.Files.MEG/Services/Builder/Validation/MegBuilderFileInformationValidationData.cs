// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Contains information in order to validate a <see cref="MegFileInformation"/>.
/// </summary>
public class MegBuilderFileInformationValidationData
{
    internal MegBuilderFileInformationValidationData(
        MegFileInformation fileInformation, 
        IReadOnlyCollection<MegFileDataEntryBuilderInfo> dataEntries)
    {
        FileInformation = fileInformation;
        DataEntries = dataEntries;
    }
    
    /// <summary>
    /// Gets the file information to validate.
    /// </summary>
    public MegFileInformation FileInformation { get; }

    /// <summary>
    /// Gets the current data entries which shall be added to a MEG archive.
    /// </summary>
    public IReadOnlyCollection<MegFileDataEntryBuilderInfo> DataEntries { get; }
}