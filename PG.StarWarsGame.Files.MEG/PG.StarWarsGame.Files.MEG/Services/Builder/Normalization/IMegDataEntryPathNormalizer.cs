using PG.Commons.Services.Builder.Normalization;
using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Provides methods to normalize a data entry's file path to store it into a MEG archive.
/// </summary>
public interface IMegDataEntryPathNormalizer : IBuilderEntryNormalizer<string>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="destination"></param>
    int Normalize(ReadOnlySpan<char> filePath, Span<char> destination);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="destination"></param>
    /// <param name="charsWritten"></param>
    /// <param name="notNormalizedMessage"></param>
    bool TryNormalize(ReadOnlySpan<char> filePath, Span<char> destination, out int charsWritten, out string? notNormalizedMessage);
}