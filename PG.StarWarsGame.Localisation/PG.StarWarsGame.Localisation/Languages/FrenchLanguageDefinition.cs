// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Languages
{
    [ExcludeFromCodeCoverage]
    public sealed class FrenchLanguageDefinition : ILanguageDefinition
    {
        public string LanguageIdentifier => "FRENCH";
        public CultureInfo Culture => CultureInfo.GetCultureInfo("fr-FR");
    }
}
