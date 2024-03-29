// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Components.Localisation.Languages;

namespace PG.StarWarsGame.Components.Localisation.Commons.Helper;

/// <summary>
///     Helper service for dealing with <see cref="IAlamoLanguageDefinition" />s.
/// </summary>
public interface IAlamoLanguageDefinitionHelper
{
    /// <summary>
    ///     Returns all currently registered <see cref="IAlamoLanguageDefinition" /> implementations.
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<IAlamoLanguageDefinition> GetAllRegisteredAlamoLanguageDefinitions();

    /// <summary>
    ///     Returns the default <see cref="IAlamoLanguageDefinition" />.
    /// </summary>
    /// <returns></returns>
    IAlamoLanguageDefinition GetDefaultAlamoLanguageDefinition();

    /// <summary>
    ///     Checks if the game officially supports a given <see cref="IAlamoLanguageDefinition" />.
    /// </summary>
    /// <param name="languageDefinition">The definition to check.</param>
    /// <returns></returns>
    bool IsOfficiallySupported(IAlamoLanguageDefinition languageDefinition);
}