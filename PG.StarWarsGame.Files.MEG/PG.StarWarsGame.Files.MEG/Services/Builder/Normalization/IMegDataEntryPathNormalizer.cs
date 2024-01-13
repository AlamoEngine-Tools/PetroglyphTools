using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Provides methods to normalize a data entry's file path to store it into a MEG archive.
/// </summary>
public interface IMegDataEntryPathNormalizer
{
    /// <summary>
    /// Normalizes the specified path.
    /// </summary>
    /// <param name="filePath">The file path to normalize.</param>
    /// <returns>The normalized file path.</returns>
    /// <exception cref="Exception">The normalization failed or <paramref name="filePath"/> is not supported.</exception>
    string NormalizePath(string filePath);

    /// <summary>
    /// Attempts to normalize the specified path.
    /// </summary>
    /// <remarks>
    /// If normalization fails, it is <b>not</b> assured that <paramref name="filePath"/> is restored to its original value.
    /// </remarks>
    /// <param name="filePath">The path variable to be normalized. The normalized path is stored in <paramref name="filePath"/>.</param>
    /// <param name="notNormalizedMessage">An optional message why the normalization failed.</param>
    /// <returns><see langword="true"/> if the path was normalized successfully; otherwise, <see langword="false"/>.</returns>
    bool TryNormalizePath(ref string filePath, out string? notNormalizedMessage);
}