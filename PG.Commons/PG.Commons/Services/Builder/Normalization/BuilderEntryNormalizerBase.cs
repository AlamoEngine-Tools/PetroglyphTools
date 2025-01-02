// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Services.Builder.Normalization;

/// <summary>
/// Base class for an <see cref="IBuilderEntryNormalizer{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the element that shall be normalized.</typeparam>
public abstract class BuilderEntryNormalizerBase<T> : IBuilderEntryNormalizer<T>
{
    /// <summary>
    /// Returns the service provider.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

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

    /// <summary>
    /// Tries to normalize the specified entry.
    /// </summary>
    /// <param name="entry">
    /// A reference to the entry to normalize. When this method returns, contains the normalized value, if successful.
    /// The original value is not restored if <see langword="false"/> is returned.
    /// </param>
    /// <param name="notNormalizedMessage">
    /// When this method returns, contains an optional error message, if the operation was not successful.
    /// The value may be <see langword="null"/>, even if the operation was not successful.</param>
    /// <returns>The normalized entry.
    /// </returns>
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