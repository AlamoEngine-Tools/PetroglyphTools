// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Immutable;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Repository.Content;

namespace PG.StarWarsGame.Components.Localisation.Repository;

/// <summary>
///     A Diff between all language contents of a given <see cref="ITranslationRepository" />.
/// </summary>
public interface ITranslationDiff
{
    /// <summary>
    ///     The base language for the diff.
    /// </summary>
    IAlamoLanguageDefinition DiffBase { get; }

    /// <summary>
    ///     All languages present in the diff.
    /// </summary>
    IImmutableSet<IAlamoLanguageDefinition> Languages { get; }

    /// <summary>
    ///     Collects all <see cref="ITranslationItemId" />s that are missing from a language when compared to the
    ///     <see cref="DiffBase" />
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    IImmutableSet<ITranslationItemId> GetMissingItemIdsForLanguage(IAlamoLanguageDefinition language);

    /// <summary>
    ///     Collects all <see cref="ITranslationItemId" />s that are still present for a language when compared to the
    ///     <see cref="DiffBase" />
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    IImmutableSet<ITranslationItemId> GetDanglingItemIdsForLanguage(IAlamoLanguageDefinition language);
}