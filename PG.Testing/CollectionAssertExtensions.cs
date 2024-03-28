using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PG.Testing;

public static class CollectionAssertExtensions
{
    public static void EqualUnordered<T>(ICollection<T>? expected, ICollection<T>? actual)
    {
        Assert.Equal(expected is null, actual is null);
        if (expected == null)
            return;

        // Lookups are an aggregated collections (enumerable contents), but ordered.
        var e = expected.Cast<object>().ToLookup(key => key);
        var a = actual!.Cast<object>().ToLookup(key => key);

        // Dictionaries can't handle null keys, which is a possibility
        Assert.Equal(e.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()),
            a.Where(kv => kv.Key != null).ToDictionary(g => g.Key, g => g.Count()));

        // Get count of null keys.  Returns an empty sequence (and thus a 0 count) if no null key
        Assert.Equal(e[null!].Count(), a[null!].Count());
    }
}