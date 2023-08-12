// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Validation;

namespace PG.Commons.Repository;

/// <summary>
///     Base contract for a object storage repository.
/// </summary>
/// <typeparam name="TKey">The key to access values.</typeparam>
/// <typeparam name="TValue">The value.</typeparam>
public interface IRepository<in TKey, TValue> : IValidatable where TValue : IValidatable
{
    /// <summary>
    ///     Tries to insert an object into the repository.
    /// </summary>
    /// <param name="key">The object's key.</param>
    /// <param name="value">the object</param>
    /// <returns>True if the insert succeeded, false else.</returns>
    bool TryCreate(TKey key, TValue value);

    /// <summary>
    ///     Tries to load the value for a given key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The object to load.</param>
    /// <returns>True if the read succeeded, false else.</returns>
    bool TryRead(TKey key, out TValue? value);

    /// <summary>
    ///     Tries to update the value for a given key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The object to load.</param>
    /// <returns>True if the update succeeded, false else.</returns>
    bool TryUpdate(TKey key, TValue value);

    /// <summary>
    ///     Tries to delete a given object from the repository.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>True if the delete succeeded, false else.</returns>
    bool TryDelete(TKey key);
}