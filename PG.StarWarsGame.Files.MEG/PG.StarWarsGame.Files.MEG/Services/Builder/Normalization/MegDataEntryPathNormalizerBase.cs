using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Base class for an <see cref="IMegDataEntryPathNormalizer"/>.
/// </summary>
public abstract class MegDataEntryPathNormalizerBase : IMegDataEntryPathNormalizer
{
    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryPathNormalizerBase"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected MegDataEntryPathNormalizerBase(IServiceProvider serviceProvider)
    {
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    }

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