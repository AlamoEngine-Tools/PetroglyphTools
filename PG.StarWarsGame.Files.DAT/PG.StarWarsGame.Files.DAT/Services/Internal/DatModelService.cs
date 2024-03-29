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

internal class DatModelService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider), IDatModelService
{
    public IReadOnlyDictionary<string, IReadOnlyList<DatStringEntry>> GetDuplicateEntries(IDatModel datModel)
    {
        if (datModel == null)
            throw new ArgumentNullException(nameof(datModel));

        var duplicates = new Dictionary<string, IReadOnlyList<DatStringEntry>>();

        foreach (var key in datModel.CrcKeys)
        {
            var entries = datModel.EntriesWithCrc(key);
            if (entries.Count > 1)
                duplicates.Add(entries.First().Key, entries);
        }
        return duplicates;
    }

    public IDatModel RemoveDuplicates(IDatModel datModel)
    {
        if (datModel == null)
            throw new ArgumentNullException(nameof(datModel));

        var newEntries = new LinkedHashSet<DatStringEntry>(datModel, CrcBasedDatStringEntryEqualityComparer.Instance)
            .ToList();
        
        if (datModel.KeySortOder == DatFileType.OrderedByCrc32)
        {
            newEntries = Crc32Utilities.SortByCrc32(newEntries);
            return new SortedDatModel(newEntries);
        }

        return new UnsortedDatModel(newEntries);
    }

    public IDatModel SortModel(IDatModel datModel)
    {
        if (datModel == null)
            throw new ArgumentNullException(nameof(datModel));

        if (datModel is ISortedDatModel)
            return datModel;
        if (datModel is IUnsortedDatModel unsortedDat)
            return unsortedDat.ToSortedModel();
        return new SortedDatModel(Crc32Utilities.SortByCrc32(datModel.ToList()));
    }

    public ISet<string> GetMissingKeysFromBase(IDatModel baseDatModel, IDatModel datToCompare)
    {
        if (baseDatModel == null)
            throw new ArgumentNullException(nameof(baseDatModel));
        if (datToCompare == null)
            throw new ArgumentNullException(nameof(datToCompare));

        var keys = baseDatModel.Keys;
        keys.ExceptWith(datToCompare.Keys);
        return keys;
    }

    public IDatModel MergeSorted(IDatModel baseDatModel, IDatModel datToMerge,
        out ICollection<MergedKeyResult> mergedKeys,
        SortedDatMergeOptions mergeOptions = SortedDatMergeOptions.KeepExisting)
    {
        if (baseDatModel == null)
            throw new ArgumentNullException(nameof(baseDatModel));
        if (datToMerge == null)
            throw new ArgumentNullException(nameof(datToMerge));

        if (baseDatModel.KeySortOder != DatFileType.OrderedByCrc32)
            throw new ArgumentException("DAT model not sorted.", nameof(baseDatModel));
        if (datToMerge.KeySortOder != DatFileType.OrderedByCrc32)
            throw new ArgumentException("DAT model not sorted.", nameof(datToMerge));

        var newEntries = baseDatModel.ToList();
        var visitedCrc = new HashSet<Crc32>();

        mergedKeys = new List<MergedKeyResult>();

        foreach (var entryToMerge in datToMerge)
        {
            if (!visitedCrc.Add(entryToMerge.Crc32))
                continue;

            var firstInBase = baseDatModel.EntriesWithCrc(entryToMerge.Crc32).FirstOrDefault();

            // The base DAT does not have the current CRC
            if (firstInBase.Equals(default))
            {
                mergedKeys.Add(new MergedKeyResult(entryToMerge));
                newEntries.Add(entryToMerge);
                continue;
            }

            if (mergeOptions == SortedDatMergeOptions.KeepExisting)
                continue;

            var firstToMerge = datToMerge.EntriesWithCrc(entryToMerge.Crc32).First();

            var indexToOverwrite = newEntries.IndexOf(firstInBase);
            Debug.Assert(indexToOverwrite >= 0);

            newEntries[indexToOverwrite] = firstToMerge;
            mergedKeys.Add(new MergedKeyResult(firstToMerge, firstInBase));
        }

        return new SortedDatModel(Crc32Utilities.SortByCrc32(newEntries));
    }

    public IDatModel MergeUnsorted(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys,
        UnsortedDatMergeOptions mergeOptions = UnsortedDatMergeOptions.ByIndex)
    {
        if (baseDatModel == null)
            throw new ArgumentNullException(nameof(baseDatModel));
        if (datToMerge == null)
            throw new ArgumentNullException(nameof(datToMerge));

        if (baseDatModel.KeySortOder != DatFileType.NotOrdered)
            throw new ArgumentException("DAT model not unsorted.", nameof(baseDatModel));
        if (datToMerge.KeySortOder != DatFileType.NotOrdered)
            throw new ArgumentException("DAT model not unsorted.", nameof(datToMerge));

        mergedKeys = new List<MergedKeyResult>();

        if (datToMerge.Count == 0)
            return baseDatModel;

        var newModelEntries = baseDatModel.ToList();
        

        if (mergeOptions == UnsortedDatMergeOptions.Append)
        {
            foreach (var newEntry in datToMerge)
            {
                mergedKeys.Add(new MergedKeyResult(newEntry));
                newModelEntries.Add(newEntry);
            }
            
        }

        if (mergeOptions == UnsortedDatMergeOptions.ByIndex)
        {
            var maxIndex = Math.Min(newModelEntries.Count, datToMerge.Count);

            for (int i = 0; i < maxIndex; i++)
            {
                var newEntry = datToMerge[i];
                var oldEntry = newModelEntries[i];
                mergedKeys.Add(new MergedKeyResult(newEntry, oldEntry));
                newModelEntries[i] = newEntry;
            }

            if (datToMerge.Count > maxIndex)
            {
                foreach (var entry in datToMerge.Skip(maxIndex))
                {
                    mergedKeys.Add(new MergedKeyResult(entry));
                    newModelEntries.Add(entry);
                }
            }
        }

        if (mergeOptions == UnsortedDatMergeOptions.Overwrite)
        {
            var currentToMergeIndex = 0;

            var toMergeCount = datToMerge.Count;

            for (int i = 0; i < newModelEntries.Count; i++)
            {
                if (currentToMergeIndex >= toMergeCount)
                    break;

                var currentEntry = newModelEntries[i];
                if (currentEntry.Crc32 != datToMerge[currentToMergeIndex].Crc32)
                    continue;

                var newEntry = datToMerge[currentToMergeIndex];
                mergedKeys.Add(new MergedKeyResult(newEntry, currentEntry));
                newModelEntries[i] = newEntry;

                currentToMergeIndex++;
            }

            if (datToMerge.Count > currentToMergeIndex)
            {
                foreach (var entry in datToMerge.Skip(currentToMergeIndex))
                {
                    mergedKeys.Add(new MergedKeyResult(entry));
                    newModelEntries.Add(entry);
                }
            }
        }


        return new UnsortedDatModel(newModelEntries);
    }


    private readonly ref struct LinkedHashSet<T>
    {
        private readonly List<T> _list;

        public LinkedHashSet(IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
        {

            var collection = enumerable as ICollection<T>;
            var suggestedCapacity = collection?.Count ?? 0;
#if NETSTANDARD2_1 || NET
            var hashSet = new HashSet<T>(suggestedCapacity, comparer);
#else
            var hashSet = new HashSet<T>(comparer);
#endif
            _list = new List<T>(suggestedCapacity);

            foreach (var item in enumerable)
            {
                if (!hashSet.Add(item))
                    continue;
                _list.Add(item);
            }
            hashSet.Clear();
        }

        public IList<T> ToList()
        {
            return _list;
        }
    }
}

//internal class DatModelService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider), IDatModelService
//{
//    public IDatModel MergeUnsorted(IDatModel baseDatModel, IDatModel datToMerge, out ICollection<MergedKeyResult> mergedKeys,
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