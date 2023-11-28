using PG.Commons.DataTypes;
using System;
using System.Collections.Generic;
using PG.Commons.Hashing;
using System.Linq;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides utility methods working with CRC32 checksums.
/// </summary>
public static class Crc32Utilities
{
    /// <summary>
    /// Converts a list already sorted by CRC32 into a table where the key is the <see cref="Crc32"/> checksum
    /// of <typeparamref name="T"/> and the value is an <see cref="IndexRange"/> indicating
    /// a range of indexes of <paramref name="items"/> which share the same key.
    /// </summary>
    /// <typeparam name="T">The actual type of the date entry.</typeparam>
    /// <param name="items">CRC32 sorted list of MEG data entries.</param>
    /// <returns>The CRC to index range table.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="items"/> is not sorted</exception>
    /// <example>
    /// The given input list [1,2,2,2,3] returns the following dictionary: {{1, (0, 1)}, {2, (1, 3)}, {3, (4, 1)}}
    /// </example>
    internal static IReadOnlyDictionary<Crc32, IndexRange> ListToCrcIndexRangeTable<T>(IList<T> items) where T : IHasCrc32
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var dict = new Dictionary<Crc32, IndexRange>();

        var lastCrc = default(Crc32);
        var currentRangeStart = 0;
        var currentRangeLength = 0;

        for (var i = 0; i < items.Count; i++)
        {
            var currentCrc = items[i].Crc32;

            if (currentCrc < lastCrc)
                throw new ArgumentException("Items are not sorted by CRC32", nameof(items));

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

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by their CRC32 checksum.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="items">A sequence of values to order by CRC32.</param>
    /// <returns>An <see cref="IList{T}"/> whose elements are sorted according to the CRC32 checksum.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public static IList<T> SortByCrc32<T>(IEnumerable<T> items) where T : IHasCrc32
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        return items.OrderBy(o => o.Crc32).ToList();
    }

    /// <summary>
    /// Checks whether all elements of a sequence are sorted in ascending order by their CRC32 checksum.
    /// Throws an <see cref="ArgumentException"/> if <paramref name="items"/> is not sorted.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="items">A sequence of values to check for correct sorting.</param>
    /// <exception cref="ArgumentException"><paramref name="items"/> is not sorted by CRC32 checksum.</exception>
    public static void EnsureSortedByCrc32<T>(IEnumerable<T> items) where T : IHasCrc32
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var lastCrc = default(Crc32);
        foreach (var item in items)
        {
            var currentCrc = item.Crc32;
            if (currentCrc < lastCrc)
                throw new ArgumentException("Items are not sorted by CRC32", nameof(items));
            lastCrc = currentCrc;
        }
    }
}