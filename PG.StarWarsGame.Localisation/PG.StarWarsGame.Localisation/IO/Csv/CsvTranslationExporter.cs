// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Text;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO.Csv
{
    /// <summary>
    /// Default implementation of <see cref="ICsvTranslationExporter"/>.
    /// Produces a CSV with header row: <c>key,LANG1,LANG2,...</c>
    /// Values containing commas or quotes are wrapped in double quotes with internal quotes escaped.
    /// </summary>
    public sealed class CsvTranslationExporter : ICsvTranslationExporter
    {
        /// <inheritdoc/>
        public string Export(ITranslationDatabase source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var langs = source.Languages;
            var sb = new StringBuilder();

            // Header
            sb.Append("key");
            foreach (var lang in langs)
            {
                sb.Append(',');
                sb.Append(Escape(lang.LanguageIdentifier));
            }
            sb.Append('\n');

            // Rows — keyed databases sort alphabetically for reproducible diffs
            var rows = source is IKeyedTranslationDatabase
                ? source.OrderBy(e => e.Key, StringComparer.Ordinal).AsEnumerable()
                : source.AsEnumerable();

            foreach (var entry in rows)
            {
                sb.Append(Escape(entry.Key));
                foreach (var lang in langs)
                {
                    sb.Append(',');
                    if (entry.TryGetTranslation(lang, out var val) && val is not null)
                        sb.Append(Escape(val));
                }
                sb.Append('\n');
            }

            return sb.ToString();
        }

        private static string Escape(string value)
        {
            if (value.IndexOf(',') >= 0 || value.IndexOf('"') >= 0 || value.IndexOf('\n') >= 0)
                return '"' + value.Replace("\"", "\"\"") + '"';
            return value;
        }
    }
}
