// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO.Abstractions;

namespace PG.Core.Data.Key
{
    public abstract class AbstractIntegerKey : AbstractKey<int>
    {
        protected AbstractIntegerKey(int key) : base(key)
        {
        }

        public new int CompareTo(IKey other)
        {
            return !(other is AbstractIntegerKey otherKey) ? int.MinValue : Key.CompareTo(otherKey.Key);
        }

        public new bool Equals(IKey other)
        {
            return other is AbstractIntegerKey otherKey && Key.Equals(otherKey.Key);
        }
    }
}
