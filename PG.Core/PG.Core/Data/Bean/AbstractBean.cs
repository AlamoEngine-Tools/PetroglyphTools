// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Core.Data.Key;

namespace PG.Core.Data.Bean
{
    public abstract class AbstractBean<TKey> : IBean<TKey> where TKey : IKey
    {
        protected AbstractBean(TKey key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public TKey Key { get; }
    }
}
