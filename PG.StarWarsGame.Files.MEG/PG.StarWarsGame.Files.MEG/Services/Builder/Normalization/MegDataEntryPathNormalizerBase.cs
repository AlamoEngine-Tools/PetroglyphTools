using System;
using System.Buffers;
using System.Globalization;
using System.IO.Abstractions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Base class for an <see cref="IMegDataEntryPathNormalizer"/>.
/// </summary>
public abstract class MegDataEntryPathNormalizerBase : BuilderEntryNormalizerBase<string>, IMegDataEntryPathNormalizer
{
    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryPathNormalizerBase"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected MegDataEntryPathNormalizerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        FileSystem = ServiceProvider.GetRequiredService<IFileSystem>();
    }

    /// <inheritdoc />
    public abstract int Normalize(ReadOnlySpan<char> filePath, Span<char> destination);

    /// <inheritdoc />
    public override string Normalize(string entry)
    {
        char[]? pooledCharArray = null;
        try
        {
            // Since normalization *might* add a few  
            var buffer = entry.Length + 10 > 260
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(entry.Length)
                : stackalloc char[260];

            var length = Normalize(entry, buffer);
            var result = buffer.Slice(0, length);
            return result.ToString();
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }

    /// <inheritdoc />
    public bool TryNormalize(ReadOnlySpan<char> filePath, Span<char> destination, out int charsWritten, out string? notNormalizedMessage)
    {
        throw new NotImplementedException();
    }
}