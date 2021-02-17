// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PG.Core.Localisation;

namespace PG.StarWarsGame.Localisation.Languages
{
    /// <summary>
    /// The language definition for the French language.
    /// </summary>
    /// <remarks>
    /// Officially supported by the Alamo Engine.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public sealed class FrenchAlamoLanguageDefinition : IAlamoLanguageDefinition
    {
        [ExcludeFromCodeCoverage] public string LanguageIdentifier => "FRENCH";
        [ExcludeFromCodeCoverage] public CultureInfo Culture => CultureInfo.GetCultureInfo("fr-FR");
    }
}
