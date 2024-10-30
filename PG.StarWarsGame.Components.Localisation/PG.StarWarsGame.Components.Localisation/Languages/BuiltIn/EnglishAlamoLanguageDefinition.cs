// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.StarWarsGame.Components.Localisation.Attributes;

namespace PG.StarWarsGame.Components.Localisation.Languages.BuiltIn;

/// <summary>
///     The language definition for the English (US) language.
/// </summary>
/// <remarks>
///     Officially supported by the Alamo Engine.<br />
///     This language is the default game language and development language of all Alamo Engine games.
/// </remarks>
[ExcludeFromCodeCoverage]
[DefaultLanguage]
[OfficiallySupportedLanguage]
public sealed class EnglishAlamoLanguageDefinition : AlamoLanguageDefinitionBase
{
    /// <inheritdoc />
    protected override string ConfiguredLanguageIdentifier => "ENGLISH";

    /// <inheritdoc />
    protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("en-US");
}