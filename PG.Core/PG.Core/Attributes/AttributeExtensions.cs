// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PG.Core.Attributes
{
    
    [ExcludeFromCodeCoverage]
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValueOrDefault<TAttribute, TValue>(this Type type,
            Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute attribute
                ? valueSelector(attribute)
                : default(TValue);
        }
    }
}
