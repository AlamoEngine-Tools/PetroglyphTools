using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// 
/// </summary>
public sealed class PetroglyphDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly PetroglyphDataEntryPathNormalizer Instance = new();

    /// <inheritdoc />
    public override string NormalizePath(string filePath)
    {
        throw new NotImplementedException();
    }
}