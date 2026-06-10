// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.StarWarsGame.Localisation.Languages.Attributes;

namespace PG.StarWarsGame.Localisation.Languages
{
    /// <summary>The language definition for Thai (TH).</summary>
    /// <remarks>Officially supported by the Alamo Engine.</remarks>
    [OfficiallySupportedLanguage(AlamoGameContext.BaseGame)]
    [ExcludeFromCodeCoverage]
    public sealed class ThaiAlamoLanguageDefinition : AlamoLanguageDefinitionBase
    {
        /// <inheritdoc/>
        protected override string ConfiguredLanguageIdentifier => "THAI";

        /// <inheritdoc/>
        protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("th");
    }
}
