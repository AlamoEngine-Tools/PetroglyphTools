// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.StarWarsGame.Components.Localisation.Attributes;

namespace PG.StarWarsGame.Components.Localisation.Languages.Builtin;

/// <summary>
///     The language definition for the Italian language.
/// </summary>
/// <remarks>
///     Officially supported by the Alamo Engine.
/// </remarks>
[ExcludeFromCodeCoverage]
[OfficiallySupportedLanguage]
public sealed class ItalianAlamoLanguageDefinition : AlamoLanguageDefinitionBase
{
    /// <inheritdoc />
    protected override string ConfiguredLanguageIdentifier => "ITALIAN";

    /// <inheritdoc />
    protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("it-IT");
}