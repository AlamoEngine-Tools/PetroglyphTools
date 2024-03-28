// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// A service to work with DAT models.
/// </summary>
public interface IDatModelService
{
    /// <summary>
    /// Gets all duplicate keys of the current model and stores them into a dictionary.
    /// The key of the dictionary represents a duplicate key in model.
    /// The value of the dictionary contains a list of the duplicate entries. This list always has two or more items and preserves the entries' order.
    /// </summary>
    /// <param name="datModel">The DAT model to check.</param>
    /// <returns>A dictionary of duplicate keys in <paramref name="datModel"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="datModel"/> is <see langword="null"/>.</exception>
    IReadOnlyDictionary<string, IReadOnlyList<DatStringEntry>> GetDuplicateEntries(IDatModel datModel);

    /// <summary>
    /// Creates a new DAT model by removing any duplicate keys of the input model keeping the first found entry.
    /// </summary>
    /// <param name="datModel">The DAT model.</param>
    /// <returns>A new duplicate-free DAT model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="datModel"/> is <see langword="null"/>.</exception>
    IDatModel RemoveDuplicates(IDatModel datModel);

    /// <summary>
    /// Creates a new DAT model by sorting all entries by their CRC32 checksum.
    /// </summary>
    /// <param name="datModel">The DAT model to sort.</param>
    /// <returns>A new CRC32-sorted DAT model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="datModel"/> is <see langword="null"/>.</exception>
    IDatModel SortModel(IDatModel datModel);

    /// <summary>
    /// Gets a set of keys missing in a specified DAT model which are present in a specified base model. 
    /// </summary>
    /// <example>
    /// <code>
    /// BaseModel       := [A, B, C]
    /// ModelToCompare  := [A, C, D]
    /// Result          := [B]
    /// </code>
    /// </example>
    /// <param name="baseDatModel">The base DAT model.</param>
    /// <param name="datToCompare">The DAT model to get missing keys from.</param>
    /// <returns>A set of keys missing in <paramref name="datToCompare"/> which are present in <paramref name="baseDatModel"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseDatModel"/> or <paramref name="datToCompare"/> is <see langword="null"/>.</exception>
    ISet<string> GetMissingKeysFromBase(IDatModel baseDatModel, IDatModel datToCompare);

    /// <summary>
    /// Creates a new <see cref="IDatModel"/> by merging one sorted model into another sorted model.
    /// <br/>
    /// When <paramref name="mergeOptions"/> is <see cref="SortedDatMergeOptions.Overwrite"/>,
    /// this method overwrites the first occurence of an entry in <paramref name="baseDatModel"/>
    /// with the first occurence of <paramref name="datToMerge"/>.
    /// <br/>
    /// <code>
    /// Example:
    /// BaseDat := [A, A', B, C] where A' is a duplicate of the key A with a different value.
    /// ToMerge := [A'', A''', D] where A'' and A''' are duplicates of the key A with a different values
    /// Result  := [A'', A', B, C, D] 
    /// </code>
    /// </summary>
    /// <remarks>
    /// A common use-case for this is if you have an English model and a foreign language model.
    /// In most cases the english model is the reference model which contains the newest keys. In that case, call
    /// <code>Merge(ForeignModel, EnglishModel, DatMergeOptions.KeepExisting)</code>
    /// This will create a new model will all foreign text, but only adds the new keys (with english values).
    /// </remarks>
    /// <param name="baseDatModel">The base model.</param>
    /// <param name="datToMerge">The model to merge into <paramref name="baseDatModel"/>.</param>
    /// <param name="mergedKeys">Keys that got added or overwritten will be stored into this variable.</param>
    /// <param name="mergeOptions">Specifies how to treat existing keys.</param>
    /// <returns>The merged model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseDatModel"/> or <paramref name="datToMerge"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="baseDatModel"/> or <paramref name="datToMerge"/> is not sorted.</exception>
    IDatModel MergeSorted(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys,
        SortedDatMergeOptions mergeOptions = SortedDatMergeOptions.KeepExisting);

    /// <summary>
    /// Creates a new <see cref="IDatModel"/> by merging one unsorted model into another unsorted model.
    /// </summary>
    /// <param name="baseDatModel">The base model.</param>
    /// <param name="datToMerge">The model to merge into <paramref name="baseDatModel"/>.</param>
    /// <param name="mergedKeys">Keys that got added or overwritten will be stored into this variable.</param>
    /// <param name="mergeOptions">Specifies how to treat existing keys.</param>
    /// <returns>The merged model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseDatModel"/> or <paramref name="datToMerge"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="baseDatModel"/> or <paramref name="datToMerge"/> is not unsorted.</exception>
    IDatModel Merge(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys,
        UnsortedDatMergeOptions mergeOptions = UnsortedDatMergeOptions.ByIndex);
}