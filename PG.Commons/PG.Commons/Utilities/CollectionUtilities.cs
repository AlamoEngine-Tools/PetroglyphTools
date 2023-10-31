using System;
using System.Collections;
using System.Collections.Generic;

namespace PG.Commons.Utilities;

internal static class CollectionUtilities
{
    internal static int TryGetCountWithoutEnumerating<T>(IEnumerable<T?> enumerable)
    {
        switch (enumerable)
        {
            case null:
                throw new ArgumentNullException(nameof(enumerable));
            case ICollection<T> collT:
                return collT.Count;
            case ICollection collection:
                return collection.Count;
            case IReadOnlyCollection<T> roc:
                return roc.Count;
            case string str:
                return str.Length;
            default:
                return -1; // -1 means, count could not be determined
        }
    }
}