// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.StarWarsGame.Localisation.Attributes;

namespace PG.StarWarsGame.Localisation.Languages.Builtin;

/// <summary>
///     The language definition for the Japanese language.
/// </summary>
/// <remarks>
///     Officially supported by the Alamo Engine.
/// </remarks>
[ExcludeFromCodeCoverage]
[OfficiallySupportedLanguage]
public sealed class JapaneseAlamoLanguageDefinition : AlamoLanguageDefinitionBase
{
    /// <inheritdoc />
    protected override string ConfiguredLanguageIdentifier => "JAPANESE";

    /// <inheritdoc />
    protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("ja");
}