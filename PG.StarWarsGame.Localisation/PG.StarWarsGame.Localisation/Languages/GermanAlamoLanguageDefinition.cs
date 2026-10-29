// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.StarWarsGame.Localisation.Languages.Attributes;

namespace PG.StarWarsGame.Localisation.Languages
{
    /// <summary>The language definition for German (DE).</summary>
    /// <remarks>Officially supported by the Alamo Engine.</remarks>
    [OfficiallySupportedLanguage]
    [ExcludeFromCodeCoverage]
    public sealed class GermanAlamoLanguageDefinition : AlamoLanguageDefinitionBase
    {
        /// <inheritdoc/>
        protected override string ConfiguredLanguageIdentifier => "GERMAN";

        /// <inheritdoc/>
        protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("de-DE");
    }
}
