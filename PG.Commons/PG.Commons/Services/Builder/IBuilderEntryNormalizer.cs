using System;

namespace PG.Commons.Services.Builder;

/// <summary>
/// Provides methods to normalize a data entry's file path to store it into a MEG archive.
/// </summary>
/// <typeparam name="T">The type of the entry to normalize.</typeparam>
public interface IBuilderEntryNormalizer<T>
{
    /// <summary>
    /// Normalizes the specified entry.
    /// </summary>
    /// <param name="entry">The entry to normalize.</param>
    /// <returns>The normalized entry.</returns>
    /// <exception cref="Exception">The normalization failed or <paramref name="entry"/> is not supported.</exception>
    T Normalize(T entry);


    /// <summary>
    /// Attempts to normalize the specified entry.
    /// </summary>
    /// <remarks>
    /// If normalization fails, it is <b>not</b> assured that <paramref name="entry"/> is restored to its original value.
    /// </remarks>
    /// <param name="entry">The entry to be normalized. The normalized entry is stored back in <paramref name="entry"/>.</param>
    /// <param name="notNormalizedMessage">An optional message why the normalization failed.</param>
    /// <returns><see langword="true"/> if the path was normalized successfully; otherwise, <see langword="false"/>.</returns>
    bool TryNormalize(ref T entry, out string? notNormalizedMessage);
}