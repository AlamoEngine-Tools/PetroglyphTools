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
    /// The language definition for the English (US) language. 
    /// </summary>
    /// <remarks>
    /// Officially supported by the Alamo Engine.<br/>
    /// This language is the default game language and development language of all Alamo Engine games.
    /// </remarks>
    [Default]
    [Order(5000)]
    [ExcludeFromCodeCoverage]
    [OfficiallySupportedLanguage]
    public sealed class EnglishAlamoLanguageDefinition : AbstractAlamoLanguageDefinition
    {
        protected override string ConfiguredLanguageIdentifier => "ENGLISH";
        protected override CultureInfo ConfiguredCulture => CultureInfo.GetCultureInfo("en-US");
    }
}
