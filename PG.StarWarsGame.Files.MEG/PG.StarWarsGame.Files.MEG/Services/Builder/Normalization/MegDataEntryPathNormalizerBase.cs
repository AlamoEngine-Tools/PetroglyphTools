// Copyright (c) Alamo Engine Tools- and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
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
    public abstract string Normalize(ReadOnlySpan<char> filePath);

    /// <inheritdoc />
    public override string Normalize(string entry)
    {
        return Normalize(entry.AsSpan());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    protected abstract int Normalize(ReadOnlySpan<char> filePath, Span<char> destination);

    /// <inheritdoc />
    public bool TryNormalize(ReadOnlySpan<char> filePath, Span<char> destination, out int charsWritten)
    {
        if (filePath.Length == 0)
        {
            charsWritten = 0;
            return true;
        }
        try
        {
            charsWritten = Normalize(filePath, destination);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            charsWritten = 0;
            return false;
        }
    }
}