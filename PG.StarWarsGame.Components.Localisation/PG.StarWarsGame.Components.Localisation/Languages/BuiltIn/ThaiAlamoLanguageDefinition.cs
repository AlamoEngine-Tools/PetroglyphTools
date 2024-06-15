// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.StarWarsGame.Components.Localisation.Attributes;

namespace PG.StarWarsGame.Components.Localisation.Languages.BuiltIn;

/// <summary>
///     The language definition for the Thai language.
/// </summary>
/// <remarks>
///     Officially supported by the Alamo Engine.
/// </remarks>
[ExcludeFromCodeCoverage]
[OfficiallySupportedLanguage]
public sealed class ThaiAlamoLanguageDefinition : AlamoLanguageDefinitionBase
{
    /// <inheritdoc />
    protected override string ConfiguredLanguageIdentifier => "THAI";

    /// <inheritdoc />
    protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("th");
}