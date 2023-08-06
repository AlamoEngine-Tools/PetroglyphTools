// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Localisation.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[ExcludeFromCodeCoverage]
public sealed class OfficiallySupportedLanguageAttribute : Attribute
{
    public bool IsOfficiallySupported { get; }

    public OfficiallySupportedLanguageAttribute()
    {
        IsOfficiallySupported = true;
    }

    public OfficiallySupportedLanguageAttribute(bool isOfficiallySupported)
    {
        IsOfficiallySupported = isOfficiallySupported;
    }
}