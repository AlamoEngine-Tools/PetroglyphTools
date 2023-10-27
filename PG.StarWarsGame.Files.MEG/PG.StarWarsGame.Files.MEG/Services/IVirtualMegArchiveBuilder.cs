using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// 
/// </summary>
public interface IVirtualMegArchiveBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataEntries"></param>
    /// <returns></returns>
    IVirtualMegArchive Build(IEnumerable<MegFileDataEntryBuilderInfo> dataEntries);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="archive"></param>
    /// <returns></returns>
    IVirtualMegArchive Build(IMegArchive archive);
}