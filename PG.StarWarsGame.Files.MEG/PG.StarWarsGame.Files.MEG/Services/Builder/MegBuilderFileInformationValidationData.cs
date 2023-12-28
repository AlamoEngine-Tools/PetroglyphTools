using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// 
/// </summary>
public readonly struct MegBuilderFileInformationValidationData
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileInformation"></param>
    /// <param name="dataEntries"></param>
    public MegBuilderFileInformationValidationData(MegFileInformation fileInformation, IReadOnlyCollection<MegFileDataEntryBuilderInfo> dataEntries)
    {
        FileInformation = fileInformation;
        DataEntries = dataEntries;
    }

    /// <summary>
    /// 
    /// </summary>
    public MegFileInformation FileInformation { get; }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyCollection<MegFileDataEntryBuilderInfo> DataEntries { get; }
}