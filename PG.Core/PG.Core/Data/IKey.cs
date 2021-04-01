// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Data
{
    /// <summary>
    /// The contract for a basic key.
    /// </summary>
    public interface IKey : IComparable<IKey>, IEquatable<IKey>
    {
        /// <summary>
        /// Unwraps the inner system UID.
        /// </summary>
        /// <returns></returns>
        long Unwrap();

        /// <summary>
        /// The global key.
        /// </summary>
        Guid GlobalKey { get; }
    }
}
