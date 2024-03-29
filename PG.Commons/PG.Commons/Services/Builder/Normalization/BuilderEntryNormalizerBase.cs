// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Services.Builder.Normalization;

/// <summary>
/// Base class for an <see cref="IBuilderEntryNormalizer{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BuilderEntryNormalizerBase<T> : IBuilderEntryNormalizer<T>
{
    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="BuilderEntryNormalizerBase{T}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected BuilderEntryNormalizerBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public abstract T Normalize(T entry);

    /// <inheritdoc/>
    public bool TryNormalize(ref T entry, out string? notNormalizedMessage)
    {
        notNormalizedMessage = null;
        try
        {
            entry = Normalize(entry);
            return true;
        }
        catch (Exception e)
        {
            notNormalizedMessage = e.Message;
            return false;
        }
    }
}