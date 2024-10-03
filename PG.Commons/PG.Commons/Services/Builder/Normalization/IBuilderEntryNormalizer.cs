// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Services.Builder.Normalization;

/// <summary>
/// Provides methods to normalize a data entry's file path to store it into a MEG archive.
/// </summary>
/// <typeparam name="T">The type of the entry to normalize.</typeparam>
public interface IBuilderEntryNormalizer<T>
{
    /// <summary>
    /// Normalizes the specified entry.
    /// </summary>
    /// <param name="entry">The entry to normalize.</param>
    /// <returns>The normalized entry.</returns>
    /// <exception cref="Exception">The normalization failed or <paramref name="entry"/> is not supported.</exception>
    T Normalize(T entry);
}