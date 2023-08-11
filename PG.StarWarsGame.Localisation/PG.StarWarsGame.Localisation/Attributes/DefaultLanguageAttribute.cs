// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Attributes;

/// <summary>
///     <see cref="Attribute" /> that can be used to mark a <see cref="IAlamoLanguageDefinition" /> as the default
///     language.
/// </summary>
public class DefaultLanguageAttribute : Attribute
{
    /// <summary>
    ///     Is set to true, if the language is a default.
    /// </summary>
    public bool IsDefaultLanguage { get; }

    /// <summary>
    ///     .ctor: Marks the given <see cref="IAlamoLanguageDefinition" /> as default.
    /// </summary>
    public DefaultLanguageAttribute()
    {
        IsDefaultLanguage = true;
    }

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="isDefaultLanguage">Boolean parameter. Can be used to explicitly mark a language as not default.</param>
    public DefaultLanguageAttribute(bool isDefaultLanguage)
    {
        IsDefaultLanguage = isDefaultLanguage;
    }
}