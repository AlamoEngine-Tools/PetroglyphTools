// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Attributes;
using PG.Core.Localisation;
using PG.Core.Localisation.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Languages
{
    /// <summary>
    /// The language definition for the Italian language.
    /// </summary>
    /// <remarks>
    /// Officially supported by the Alamo Engine.
    /// </remarks>
    [Order(5000)]
    [ExcludeFromCodeCoverage]
    [OfficiallySupportedLanguage]
    public sealed class ItalianAlamoLanguageDefinition : AbstractAlamoLanguageDefinition
    {
        protected override string ConfiguredLanguageIdentifier => "ITALIAN";
        protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("it-IT");
    }
}
