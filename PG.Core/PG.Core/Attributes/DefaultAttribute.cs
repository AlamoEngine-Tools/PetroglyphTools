// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Core.Attributes
{
    /// <summary>
    /// Marks a given implementation as default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [ExcludeFromCodeCoverage]
    public class DefaultAttribute : Attribute
    {
        public bool IsDefault { get; }

        public DefaultAttribute() : this(true)
        {
        }

        public DefaultAttribute(bool isDefault)
        {
            IsDefault = isDefault;
        }
    }
}
