using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Base class for an <see cref="IMegDataEntryPathNormalizer"/>.
/// </summary>
public abstract class MegDataEntryPathNormalizerBase : IMegDataEntryPathNormalizer
{
    /// <inheritdoc/>
    public abstract string NormalizePath(string filePath);

    /// <inheritdoc/>
    public bool TryNormalizePath(ref string filePath, out string? notNormalizedMessage)
    {
        notNormalizedMessage = null;
        try
        {
            filePath = NormalizePath(filePath);
            return true;
        }
        catch (Exception e)
        {
            notNormalizedMessage = e.Message;
            return false;
        }
    }
}