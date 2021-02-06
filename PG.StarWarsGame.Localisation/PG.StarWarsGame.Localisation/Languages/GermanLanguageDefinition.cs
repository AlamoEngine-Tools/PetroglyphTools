// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Languages
{
    [ExcludeFromCodeCoverage]
    public sealed class GermanLanguageDefinition : ILanguageDefinition
    {
        public string LanguageIdentifier => "GERMAN";
        public CultureInfo Culture => CultureInfo.GetCultureInfo("de-DE");
    }
}
