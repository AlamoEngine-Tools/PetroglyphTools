// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Attributes;
using PG.Core.Data.Key;

namespace PG.Core.Data.Bean
{
    /// <summary>
    /// The base contract for an entity that can be uniquely identified by a <see cref="IKey"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    [Order]
    public interface IBean<TKey> where TKey : IKey
    {
        TKey Key { get; }
    }
}
