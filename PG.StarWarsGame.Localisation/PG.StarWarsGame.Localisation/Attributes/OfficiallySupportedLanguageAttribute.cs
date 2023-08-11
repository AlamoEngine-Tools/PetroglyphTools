// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Localisation.Attributes;

/// <summary>
///     An <see cref="Attribute" /> that marks a language as officially supported.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[ExcludeFromCodeCoverage]
public sealed class OfficiallySupportedLanguageAttribute : Attribute
{
    /// <summary>
    ///     Attribute value.
    /// </summary>
    public bool IsOfficiallySupported { get; }

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
    public OfficiallySupportedLanguageAttribute(bool isOfficiallySupported)
    {
        IsOfficiallySupported = isOfficiallySupported;
    }
}