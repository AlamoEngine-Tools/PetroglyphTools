// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using PG.Core.Attributes;
using PG.Core.Data.Bean;
using PG.Core.Data.Key;

namespace PG.Core.Data.Repository
{
    /// <summary>
    /// The contract for a basic data repository holding entities adhering to the <see cref="IBean{TKey}"/> contract.
    /// </summary>
    /// <typeparam name="TKey">The primary key type.</typeparam>
    /// <typeparam name="TBean">The entity type identified by the primary key of type <see cref="TKey"/></typeparam>
    [Order]
    public interface IRepository<in TKey,TBean> where TKey : IKey where TBean : IBean<TKey>
    {
        /// <summary>
        /// Tries to add a <see cref="IBean{TKey}"/> to the repository.
        /// </summary>
        /// <param name="bean">The <see cref="IBean{TKey}"/> to add to the repository.</param>
        /// <returns><code>true</code> if the <see cref="IBean{TKey}"/> was added successfully, <code>false</code> else.</returns>
        bool TryAdd(TBean bean);
        /// <summary>
        /// Tries to update an existing <see cref="IBean{TKey}"/> in the the repository.
        /// </summary>
        /// <param name="bean">The <see cref="IBean{TKey}"/> to update in the repository.</param>
        /// <returns><code>true</code> if the <see cref="IBean{TKey}"/> was updated successfully, <code>false</code> else.</returns>
        bool TryUpdate(TBean bean);
        /// <summary>
        /// Tries to add a <see cref="IBean{TKey}"/> to the repository. If the <see cref="IBean{TKey}"/> already exists, the <see cref="IBean{TKey}"/> will be updated instead.
        /// </summary>
        /// <param name="bean">The <see cref="IBean{TKey}"/> to add/update.</param>
        /// <returns><code>true</code> if the <see cref="IBean{TKey}"/> was added or updated successfully, <code>false</code> else.</returns>
        bool TryAddOrUpdate(TBean bean);
        /// <summary>
        /// Tries to remove a <see cref="IBean{TKey}"/> from the repository.
        /// </summary>
        /// <param name="bean">The <see cref="IBean{TKey}"/> to remove.</param>
        /// <returns><code>true</code> if the <see cref="IBean{TKey}"/> was removed successfully, <code>false</code> else.</returns>
        bool TryRemove(TBean bean);
        /// <summary>
        /// Tries to remove a <see cref="IBean{TKey}"/> from the repository.
        /// </summary>
        /// <param name="key">The <see cref="IKey"/> of the <see cref="IBean{TKey}"/> to remove.</param>
        /// <returns><code>true</code> if the <see cref="IBean{TKey}"/> was removed successfully, <code>false</code> else.</returns>
        bool TryRemove(TKey key);

        /// <summary>
        /// Tries to fetch a <see cref="IBean{TKey}"/> from the repository.
        /// </summary>
        /// <param name="key">The <see cref="IKey"/> of the <see cref="IBean{TKey}"/> to fetch.</param>
        /// <param name="bean">The fetched <see cref="IBean{TKey}"/> or null.</param>
        /// <returns><code>true</code> if the <see cref="IBean{TKey}"/> was fetched successfully, <code>false</code> else.</returns>
        bool TryGet(TKey key, [MaybeNullWhen(false)] out TBean bean);
    }
}
