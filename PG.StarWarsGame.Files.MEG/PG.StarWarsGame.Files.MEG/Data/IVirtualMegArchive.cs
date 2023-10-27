namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// 
/// </summary>
// Unfortunately this interface cannot directly inherit from IMegArchive as otherwise an IMegFile could have a virtual archive as a model.
// The alternative would be to have IMegArchive as a base for IVirtualMegArchive and something like IPhysicalMegArchive.
// This however causes problems:
//      a) the IMegBinaryConverter cannot use the IMegArchive base interface. Instead it needs the more specific types for return and parameter types.
//      b) there needs to be a conversion IMegArchive --> IPhysicalMegArchive. However, the conversion is not always legal
//         e.g., IVirtualMegArchive --> IPhysicalMegArchive should not be allowed.
//         Ultimately this conversion would make this API hard to understand and use.
public interface IVirtualMegArchive
{
    /// <summary>
    /// 
    /// </summary>
    IMegArchive Archive { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    MegDataEntryOriginInfo? GetOrigin(MegDataEntry entry);
}