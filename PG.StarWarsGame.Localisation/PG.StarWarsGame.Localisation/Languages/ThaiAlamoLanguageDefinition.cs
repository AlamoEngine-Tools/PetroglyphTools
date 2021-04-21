// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.Core.Attributes;
using PG.Core.Localisation;
using PG.Core.Localisation.Attributes;

namespace PG.StarWarsGame.Localisation.Languages
{
    /// <summary>
    /// The language definition for the Thai language.
    /// </summary>
    /// <remarks>
    /// Officially supported by the Alamo Engine.
    /// </remarks>
    [Order(5000)]
    [ExcludeFromCodeCoverage]
    [OfficiallySupportedLanguage]
    public sealed class ThaiAlamoLanguageDefinition : AbstractAlamoLanguageDefinition
    {
        protected override string ConfiguredLanguageIdentifier => "THAI";
        protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("th");
    }
}
