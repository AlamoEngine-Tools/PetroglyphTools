// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Components.Localisation.Languages;

namespace PG.StarWarsGame.Components.Localisation.Services;

/// <summary>
///     Helper service for handling <see cref="IAlamoLanguageDefinition" />s.
/// </summary>
public interface IAlamoLanguageSupportService
{
    /// <summary>
    ///     Checks if a given <see cref="IAlamoLanguageDefinition" /> is a builtin languge.
    /// </summary>
    /// <param name="definition"></param>
    /// <returns></returns>
    bool IsBuiltInLanguageDefinition(IAlamoLanguageDefinition definition);

    /// <summary>
    ///     Gets the fallback language if none can be resolved. By default, this is the return value of
    ///     <see cref="GetDefaultLanguageDefinition" /> but can be
    ///     overridden with <see cref="WithOverrideFallbackLanguage" />
    /// </summary>
    /// <returns></returns>
    IAlamoLanguageDefinition GetFallBackLanguageDefinition();

    /// <summary>
    ///     Allows for overriding the fallback language.
    /// </summary>
    /// <param name="definition"></param>
    /// <returns></returns>
    IAlamoLanguageSupportService WithOverrideFallbackLanguage(IAlamoLanguageDefinition definition);

    /// <summary>
    ///     Returns the default language marked with the
    ///     <see cref="PG.StarWarsGame.Components.Localisation.Attributes.DefaultLanguageAttribute" />
    /// </summary>
    /// <returns></returns>
    IAlamoLanguageDefinition GetDefaultLanguageDefinition();
}