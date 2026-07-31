// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Data.Internal;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO
{
    /// <summary>
    /// Extensions that compose single-language importers into multi-language loads.
    /// </summary>
    public static class TranslationImporterExtensions
    {
        /// <summary>
        /// Imports one source per language into <paramref name="target"/>, producing entries that carry every
        /// language rather than one copy of the data per language.
        /// </summary>
        /// <remarks>
        /// This is the supported way to fill an <see cref="IOrderedTranslationDatabase"/> from a single-language
        /// importer. Calling <see cref="ITranslationImporter{TSource}.Import"/> repeatedly against an ordered
        /// database cannot work, because ordered databases append on every write and the importer has no way to
        /// tell whether a value belongs on a new row or an existing one. Here the first source establishes the
        /// rows and each later source widens them positionally; a row that has no counterpart to widen is
        /// appended instead, so a mismatched source can never overwrite an unrelated row.
        /// <para>
        /// Keyed databases merge by key on their own, so they are simply imported in sequence.
        /// </para>
        /// </remarks>
        /// <param name="importer">The single-language importer to drive.</param>
        /// <param name="sources">The sources to read, in the order their rows should be established.</param>
        /// <param name="target">The database to fill.</param>
        /// <exception cref="ArgumentNullException">Any argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// A language in <paramref name="sources"/> is not tracked by <paramref name="target"/>.
        /// </exception>
        public static void ImportAll<TSource>(
            this ITranslationImporter<TSource> importer,
            IReadOnlyList<KeyValuePair<IAlamoLanguageDefinition, TSource>> sources,
            ITranslationDatabase target)
        {
            if (importer is null) throw new ArgumentNullException(nameof(importer));
            if (sources is null) throw new ArgumentNullException(nameof(sources));
            if (target is null) throw new ArgumentNullException(nameof(target));

            if (target is not IOrderedTranslationDatabase ordered)
            {
                foreach (var source in sources)
                    importer.Import(source.Value, source.Key, target);
                return;
            }

            foreach (var source in sources)
            {
                var language = source.Key;
                if (language is null) throw new ArgumentNullException(nameof(sources));

                // Deliberately not built through ITranslationDatabaseFactory. This buffer never leaves the
                // method, so resolving a consumer-registered factory for it would pull their implementation
                // into an internal step it has no stake in — and the merge below depends on the append
                // semantics of this specific type.
                var staging = new OrderedTranslationDatabase(new[] { language });
                importer.Import(source.Value, language, staging);
                MergeRows(ordered, staging, language);
            }
        }

        private static void MergeRows(
            IOrderedTranslationDatabase target,
            IOrderedTranslationDatabase staging,
            IAlamoLanguageDefinition language)
        {
            for (var i = 0; i < staging.Count; i++)
            {
                var row = staging[i];
                row.TryGetTranslation(language, out var value);

                if (i < target.Count && target[i].Key == row.Key)
                    target.SetTranslationAt(i, language, value ?? string.Empty);
                else
                    target.SetTranslation(row.Key, language, value ?? string.Empty);
            }
        }
    }
}
