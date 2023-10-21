namespace PG.StarWarsGame.Files.MEG.Data;

/// <inheritdoc/>
public interface IVirtualMegArchive : IMegArchive
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    MegDataEntryOriginInfo? GetOrigin(MegDataEntry entry);
}