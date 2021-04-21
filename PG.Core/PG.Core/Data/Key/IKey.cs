// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Core.Attributes;

namespace PG.Core.Data.Key
{
    /// <summary>
    /// The contract for a basic key.
    /// </summary>
    [Order(OrderAttribute.DEFAULT_ORDER)]
    public interface IKey : IComparable<IKey>, IEquatable<IKey>
    {
    }
}
