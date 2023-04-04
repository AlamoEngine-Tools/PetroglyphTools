// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Core.Attributes
{
    /// <summary>
    /// The sort order attribute of the class, interface or struct.
    /// Defaults to 5000
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    [ExcludeFromCodeCoverage]
    public sealed class OrderAttribute : Attribute
    {
        public const double DEFAULT_ORDER = 5000;

        /// <summary>
        /// The sort order of the class, interface or struct.
        /// Defaults to 5000
        /// </summary>
        public double Order { get; }

        public OrderAttribute() : this(DEFAULT_ORDER)
        {
        }

        public OrderAttribute(double order)
        {
            Order = order;
        }
    }
}
