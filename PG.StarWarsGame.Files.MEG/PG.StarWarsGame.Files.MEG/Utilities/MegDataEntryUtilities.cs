// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal static class MegDataEntryUtilities
{
    /// <summary>
    /// Converts an already sorted list of <see cref="IMegDataEntry"/> into a table where the key is the <see cref="Crc32"/> checksum
    /// of the entry and the value is an <see cref="IndexRange"/>.
    /// </summary>
    /// <typeparam name="T">The actual type of the date entry.</typeparam>
    /// <param name="entries">CRC32 sorted list of MEG data entries.</param>
    /// <returns>The CRC to index range table.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="entries"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="entries"/> is not sorted</exception>
    internal static IReadOnlyDictionary<Crc32, IndexRange> EntriesToCrcIndexRangeTable<T>(IReadOnlyList<T> entries) where T : IMegDataEntry
    {
        if (entries == null) 
            throw new ArgumentNullException(nameof(entries));

        var dict = new Dictionary<Crc32, IndexRange>();

        var lastCrc = new Crc32(0);
        var currentRangeStart = 0;
        var currentRangeLength = 0;

        for (var i = 0; i < entries.Count; i++)
        {
            var currentCrc = entries[i].FileNameCrc32;

            if (currentCrc < lastCrc)
                throw new InvalidOperationException("Meg Data entries are not correctly sorted.");

            if (currentCrc == lastCrc) 
                currentRangeLength++;

            if (currentCrc > lastCrc)
            {
                currentRangeStart = i;
                currentRangeLength = 1;
            }

            dict[currentCrc] = new IndexRange(currentRangeStart, currentRangeLength);
            lastCrc = currentCrc;
        }
        return dict;
    }
}