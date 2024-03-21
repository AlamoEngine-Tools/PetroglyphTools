using System.Collections.Generic;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
/// 
/// </summary>
public interface IDatModel : IReadOnlyList<DatFileEntry>
{
    /// <summary>
    /// 
    /// </summary>
    public DatKind Kind { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool ContainsKey(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyCrc"></param>
    /// <returns></returns>
    bool ContainsKey(Crc32 keyCrc);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    ReadOnlyFrugalList<DatFileEntry> EntriesWithCrc(Crc32 crc);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ReadOnlyFrugalList<DatFileEntry> EntriesWithKey(string key);
}


/// <summary>
/// 
/// </summary>
public enum DatKind
{
    /// <summary>
    /// 
    /// </summary>
    LocalizedStrings,
    /// <summary>
    /// 
    /// </summary>
    Credits
}