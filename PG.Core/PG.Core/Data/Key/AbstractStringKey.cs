// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data.Key
{
    public abstract class AbstractStringKey : AbstractKey<string>
    {
        public AbstractStringKey(string key) : base(key)
        {
        }

        public new int CompareTo(IKey other)
        {
            return other is not AbstractStringKey otherKey
                ? int.MinValue
                : string.Compare(Key, otherKey.Key, StringComparison.Ordinal);
        }

        public new bool Equals(IKey other)
        {
            return other is AbstractStringKey otherKey && Key.Equals(otherKey.Key, StringComparison.Ordinal);
        }
    }
}
