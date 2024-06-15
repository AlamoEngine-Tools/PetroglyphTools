// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Components.Localisation.Attributes;

/// <summary>
///     Marks the default (fallback) language.
/// </summary>
public class DefaultLanguageAttribute : Attribute
{
    /// <summary>
    ///     .ctor
    /// </summary>
    public DefaultLanguageAttribute()
    {
        IsDefaultLanguage = true;
    }

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="isDefaultLanguage"></param>
    public DefaultLanguageAttribute(bool isDefaultLanguage)
    {
        IsDefaultLanguage = isDefaultLanguage;
    }

    /// <summary>
    ///     Returns true for the default language.
    /// </summary>
    public bool IsDefaultLanguage { get; }
}