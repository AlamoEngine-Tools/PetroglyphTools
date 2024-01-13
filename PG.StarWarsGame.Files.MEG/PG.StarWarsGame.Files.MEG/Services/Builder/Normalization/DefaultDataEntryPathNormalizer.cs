using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// 
/// </summary>
public sealed class DefaultDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly DefaultDataEntryPathNormalizer Instance = new();

    /// <inheritdoc />
    public override string NormalizePath(string filePath)
    {
        throw new NotImplementedException();
    }
}