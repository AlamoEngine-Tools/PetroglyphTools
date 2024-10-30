// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Components.Localisation.Attributes;

/// <summary>
///     Marks a language as officially supported.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[ExcludeFromCodeCoverage]
public sealed class OfficiallySupportedLanguageAttribute : Attribute
{
    /// <summary>
    ///     .ctor
    /// </summary>
    public OfficiallySupportedLanguageAttribute()
    {
        IsOfficiallySupported = true;
    }

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="isOfficiallySupported"></param>
    public OfficiallySupportedLanguageAttribute(bool isOfficiallySupported)
    {
        IsOfficiallySupported = isOfficiallySupported;
    }

    /// <summary>
    ///     Returns true if the language is officially supported.
    /// </summary>
    public bool IsOfficiallySupported { get; }
}