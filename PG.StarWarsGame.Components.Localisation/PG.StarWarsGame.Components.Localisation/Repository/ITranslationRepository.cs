// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Repository.Content;

namespace PG.StarWarsGame.Components.Localisation.Repository;

/// <summary>
///     The base interface representing a repository containing translations.
/// </summary>
public interface ITranslationRepository
{
    /// <summary>
    ///     The merge strategy used when adding a new language to the repository.
    /// </summary>
    enum MergeStrategy
    {
        /// <summary>
        ///     If the <see cref="IAlamoLanguageDefinition" /> already exists, the new items will replace the existing items.
        /// </summary>
        Replace,

        /// <summary>
        ///     If the <see cref="ITranslationItemId" /> already exists, the <see cref="ITranslationItem" /> will be merged,
        ///     otherwise added.
        /// </summary>
        MergeByKey,

        /// <summary>
        ///     The <see cref="ITranslationItem" />s will be appended if possible.
        /// </summary>
        Append
    }

    /// <summary>
    ///     Readonly dictionary of all contained <see cref="ITranslationItem" />s separated by language.
    /// </summary>
    IReadOnlyDictionary<IAlamoLanguageDefinition, IReadOnlyCollection<IReadOnlyTranslationItem>> Content { get; }

    /// <summary>
    ///     Readonly collection of all contained <see cref="ITranslationItem" />s.
    /// </summary>
    IReadOnlyCollection<IReadOnlyTranslationItem> GetTranslationItemsByLanguage(IAlamoLanguageDefinition language);

    /// <summary>
    ///     Adds a new language to the repository.
    /// </summary>
    /// <param name="language"></param>
    /// <returns>True, if the language was added successfully.</returns>
    bool AddLanguage(IAlamoLanguageDefinition language);

    /// <summary>
    ///     Adds a new language with initial values to the repository. If the language is already present, the translation
    ///     items will be added to the existing set according to the <see cref="MergeStrategy" />
    /// </summary>
    /// <param name="language"></param>
    /// <param name="translationItems"></param>
    /// <param name="strategy">The merge strategy used if the language already exists.</param>
    /// <returns>True, if the language was added successfully.</returns>
    bool AddOrUpdateLanguage(IAlamoLanguageDefinition language, ICollection<ITranslationItem> translationItems,
        MergeStrategy strategy = MergeStrategy.MergeByKey);

    /// <summary>
    ///     Removes a language from the repository.
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    bool RemoveLanguage(IAlamoLanguageDefinition language);

    /// <summary>
    ///     Fetches a given translation item
    /// </summary>
    /// <param name="language"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    ITranslationItem GetTranslationItem(IAlamoLanguageDefinition language, ITranslationItemId id);

    /// <summary>
    ///     Removes a given translation item from the repository. This operation applies to <b>ALL</b> languages in the
    ///     repository.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool RemoveTranslationItem(ITranslationItemId id);

    /// <summary>
    ///     Adds a translation item to a given language.
    /// </summary>
    /// <param name="language"></param>
    /// <param name="item"></param>
    void AddOrUpdateTranslationItem(IAlamoLanguageDefinition language, ITranslationItem item);

    /// <summary>
    ///     Creates a diff between the present languages.
    /// </summary>
    /// <param name="diffBase"></param>
    /// <returns></returns>
    ITranslationDiff CreateTranslationDiff(IAlamoLanguageDefinition diffBase);

    /// <summary>
    ///     Applies a diff to th repository. Missing items in languages will be inserted, items not present in the master
    ///     language will be removed.
    /// </summary>
    /// <param name="diff"></param>
    /// <returns></returns>
    bool ApplyTranslationDiff(ITranslationDiff diff);
}