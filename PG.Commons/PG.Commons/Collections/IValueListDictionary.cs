namespace PG.Commons.Collections;

/// <summary>
/// Represents a generic collection of key/value-list pairs.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the value-list in the dictionary.</typeparam>
public interface IValueListDictionary<TKey, TValue> : IReadOnlyValueListDictionary<TKey, TValue> where TKey : notnull
{
    /// <summary>
    /// Adds an element with the provided key to the value-list of the dictionary.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <returns><see langword="true"/> if values of the same <paramref name="key"/> already existed; otherwise, <see langword="false"/>.</returns>
    bool Add(TKey key, TValue value);
}