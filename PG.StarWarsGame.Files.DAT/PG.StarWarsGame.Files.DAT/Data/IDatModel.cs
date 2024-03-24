// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
/// 
/// </summary>
public interface IDatModel : IReadOnlyList<DatStringEntry>
{
    /// <summary>
    /// 
    /// </summary>
    ISet<string> Keys { get; }

    /// <summary>
    /// 
    /// </summary>
    ISet<Crc32> CrcKeys { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Credit models <b>may</b> also be sorted by pure chance. So this property does not provide a safe way to determine the semantics of this model.
    /// </remarks>
    public DatFileType KeySortOder { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool ContainsKey(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string GetValue(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(string key, [NotNullWhen(true)] out string? value);

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
    ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 crc);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ReadOnlyFrugalList<DatStringEntry> EntriesWithKey(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string GetValue(Crc32 key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(Crc32 key, [NotNullWhen(true)] out string? value);
}