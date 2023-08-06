// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Localisation.Attributes;

public class DefaultLanguageAttribute : Attribute
{
    public bool IsDefaultLanguage { get; }

    public DefaultLanguageAttribute()
    {
        IsDefaultLanguage = true;
    }

    public DefaultLanguageAttribute(bool isDefaultLanguage)
    {
        IsDefaultLanguage = isDefaultLanguage;
    }
}