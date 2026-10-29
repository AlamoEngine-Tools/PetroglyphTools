// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO.Properties
{
    /// <summary>
    /// Default implementation of <see cref="IPropertiesTranslationImporter"/>.
    /// Reads <c>KEY=Value</c> lines; lines starting with <c>#</c> are comments and are skipped.
    /// Only the first <c>=</c> is treated as a separator; the value may contain further <c>=</c> characters.
    /// </summary>
    public sealed class PropertiesTranslationImporter : IPropertiesTranslationImporter
    {
        /// <inheritdoc/>
        public void Import(TextReader source, IAlamoLanguageDefinition language, ITranslationDatabase target)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (language is null) throw new ArgumentNullException(nameof(language));
            if (target is null) throw new ArgumentNullException(nameof(target));

            string? line;
            while ((line = source.ReadLine()) is not null)
            {
                var trimmed = line.TrimStart();
                if (trimmed.Length == 0 || trimmed[0] == '#') continue;

                var separator = trimmed.IndexOf('=');
                if (separator < 0) continue;

                var key = UnescapeKey(trimmed.Substring(0, separator));
                var value = trimmed.Substring(separator + 1);
                target.SetTranslation(key, language, value);
            }
        }

        private static string UnescapeKey(string key) =>
            key.Replace("\\=", "=").Replace("\\:", ":").Replace("\\#", "#");
    }
}
