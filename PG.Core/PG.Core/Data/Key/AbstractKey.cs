// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Core.Data.Key
{
    public abstract class AbstractKey<T> : IKey
    {
        public T Key { get; }

        public AbstractKey(T key)
        {
            Key = key;
        }

        public virtual int CompareTo(IKey other)
        {
            return other is not AbstractKey<T> otherKey ? int.MinValue : Key.GetHashCode().CompareTo(otherKey.Key.GetHashCode());
        }

        public virtual bool Equals(IKey other)
        {
            return other is AbstractKey<T> otherKey && Key.Equals(otherKey.Key);
        }
    }
}
