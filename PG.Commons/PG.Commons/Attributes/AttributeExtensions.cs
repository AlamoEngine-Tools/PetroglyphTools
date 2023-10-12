// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PG.Commons.Attributes;

/// <summary>
///     Extension for the <see cref="Attribute" /> class to retrieve an Attribute's value or a defined default.
/// </summary>
[ExcludeFromCodeCoverage]
public static class AttributeExtensions
{
    /// <summary>
    ///     Returns the value of an attribute or a default.
    /// </summary>
    /// <param name="type">The Type of the attribute.</param>
    /// <param name="valueSelector">The function to retrieve the value.</param>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <typeparam name="TValue">The return type.</typeparam>
    public static TValue? GetAttributeValueOrDefault<TAttribute, TValue>(this Type type,
        Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
    {
        return type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute attribute
            ? valueSelector(attribute)
            : default;
    }
}