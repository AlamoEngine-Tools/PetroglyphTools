// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services.Builder.Normalization;
using PG.Commons.Utilities;

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
    public abstract void Normalize(ReadOnlySpan<char> filePath, ref ValueStringBuilder stringBuilder);

    /// <inheritdoc />
    public bool TryNormalize(ReadOnlySpan<char> filePath, Span<char> destination, out int charsWritten)
    {
        var sb = new ValueStringBuilder(stackalloc char[260]);
        Normalize(filePath, ref sb);
        var result = sb.TryCopyTo(destination, out charsWritten);
        sb.Dispose();
        return result;
    }

    /// <inheritdoc />
    public override string Normalize(string entry)
    {
        var sb = new ValueStringBuilder(stackalloc char[260]);
        Normalize(entry.AsSpan(), ref sb);
        return sb.ToString();
    }
}