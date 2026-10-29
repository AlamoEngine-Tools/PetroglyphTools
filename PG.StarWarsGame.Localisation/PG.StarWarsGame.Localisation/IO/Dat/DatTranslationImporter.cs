// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO.Dat
{
    /// <summary>
    /// Default implementation of <see cref="IDatTranslationImporter"/>.
    /// </summary>
    public sealed class DatTranslationImporter : IDatTranslationImporter
    {
        /// <inheritdoc/>
        public void Import(IDatModel source, IAlamoLanguageDefinition language, ITranslationDatabase target)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (language is null) throw new ArgumentNullException(nameof(language));
            if (target is null) throw new ArgumentNullException(nameof(target));

            foreach (var entry in source)
                target.SetTranslation(entry.Key, language, entry.Value);
        }
    }
}
