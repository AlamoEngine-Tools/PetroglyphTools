// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Services;

//internal class DatModelService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider), IDatModelService
//{
//    public IReadOnlyDictionary<string, IReadOnlyList<DatStringEntry>> GetDuplicateEntries(IDatModel datModel)
//    {
//        if (datModel == null) 
//            throw new ArgumentNullException(nameof(datModel));

//        var duplicates = new Dictionary<string, IReadOnlyList<DatStringEntry>>();

//        foreach (var key in datModel.CrcKeys)
//        {
//            var entries = datModel.EntriesWithCrc(key);
//            if (entries.Count > 1)
//                duplicates.Add(entries.First().Key, entries);
//        }
//        return duplicates;
//    }

//    public T RemoveDuplicates<T>(T datModel) where T : class, IDatModel
//    {
//        if (datModel == null)
//            throw new ArgumentNullException(nameof(datModel));

//        IList<DatStringEntry> newEntries = new List<DatStringEntry>();

//        foreach (var key in datModel.CrcKeys)
//        {
//            var lastEntry = datModel.EntriesWithCrc(key).Last();
//            newEntries.Add(lastEntry);
//        }

//        if (datModel.KeySortOder == DatFileType.OrderedByCrc32)
//            newEntries = Crc32Utilities.SortByCrc32(newEntries);

//        return new DatModel(newEntries);
//    }

//    public ISet<string> GetMissingKeysFromBase(IDatModel baseDatModel, IDatModel datToCompare)
//    {
//        if (baseDatModel == null)
//            throw new ArgumentNullException(nameof(baseDatModel));
//        if (datToCompare == null)
//            throw new ArgumentNullException(nameof(datToCompare));

//        var keys = baseDatModel.Keys;
//        keys.ExceptWith(datToCompare.Keys);
//        return keys;
//    }

//    public IDatModel Merge(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys,
//        DatMergeOptions mergeOptions = DatMergeOptions.KeepExisting)
//    {
//        if (baseDatModel == null) 
//            throw new ArgumentNullException(nameof(baseDatModel));
//        if (datToMerge == null) 
//            throw new ArgumentNullException(nameof(datToMerge));

//        if (baseDatModel.KeySortOder != datToMerge.KeySortOder)
//            throw new NotSupportedException("Key sort order type must be equal for both DAT models.");

//        if (baseDatModel.KeySortOder == DatFileType.OrderedByCrc32)
//            return MergeSorted(baseDatModel, datToMerge, out mergedKeys, mergeOptions);

//        return MergeUnsorted(baseDatModel, datToMerge, out mergedKeys, mergeOptions);
//    }

//    private IDatModel MergeUnsorted(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys, DatMergeOptions mergeOptions)
//    {
//        var newEntries = baseDatModel.ToList();

        
//        var maxIteration = Math.Min(newEntries.Count, datToMerge.Count);
//        var totalKeys = Math.Max(newEntries.Count, datToMerge.Count);

//        mergedKeys = new List<MergedKeyResult>(totalKeys);

//        int currentIndex;
//        for (currentIndex = 0; currentIndex < maxIteration; currentIndex++)
//        {
//            var baseEntry = baseDatModel[currentIndex];
//            var newEntry = datToMerge[currentIndex];

//            if (baseEntry.Crc32 == newEntry.Crc32 && mergeOptions == DatMergeOptions.KeepExisting)
//                continue;

//            var oldEntry = newEntries[currentIndex];
//            newEntries[currentIndex] = newEntry;
//            mergedKeys.Add(new MergedKeyResult(newEntry, oldEntry));
//        }


//        // Append the remaining 
//        if (datToMerge.Count > maxIteration)
//        {
//            foreach (var newEntry in datToMerge.Take(currentIndex))
//            {
//                newEntries.Add(newEntry);
//                mergedKeys.Add(new MergedKeyResult(newEntry));
//            }
//        }

//        return new DatModel(newEntries);

//    }

//    private IDatModel MergeSorted(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys, DatMergeOptions mergeOptions)
//    {
//        var newEntries = baseDatModel.ToList();
//        var visitedCrc = new HashSet<Crc32>();

//        mergedKeys = new List<MergedKeyResult>();

//        foreach (var entryToMerge in datToMerge)
//        {
//            if (!visitedCrc.Add(entryToMerge.Crc32))
//                continue;

//            var lastInBase = baseDatModel.EntriesWithCrc(entryToMerge.Crc32).LastOrDefault();

//            // The base DAT does not have the current CRC
//            if (lastInBase.Equals(default))
//            {
//                mergedKeys.Add(new MergedKeyResult(entryToMerge));
//                newEntries.Add(entryToMerge);
//                continue;
//            }


//            if (mergeOptions == DatMergeOptions.KeepExisting)
//                continue;


//            var lastToMerge = datToMerge.EntriesWithCrc(entryToMerge.Crc32).Last();

//            var indexToOverwrite = newEntries.IndexOf(lastInBase);
//            Debug.Assert(indexToOverwrite >= 0);

//            newEntries[indexToOverwrite] = lastToMerge;
//            mergedKeys.Add(new MergedKeyResult(lastToMerge, lastInBase));
//        }

//        return new DatModel(Crc32Utilities.SortByCrc32(newEntries));
//    }
//}