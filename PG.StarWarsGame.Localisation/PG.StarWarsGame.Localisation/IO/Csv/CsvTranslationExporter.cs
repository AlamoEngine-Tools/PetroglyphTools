// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using PG.StarWarsGame.Localisation.Data;

namespace PG.StarWarsGame.Localisation.IO.Csv
{
    /// <summary>
    /// Default implementation of <see cref="ICsvTranslationExporter"/>.
    /// Produces a CSV with header row: <c>key,LANG1,LANG2,...</c>
    /// </summary>
    public sealed class CsvTranslationExporter : ICsvTranslationExporter
    {
        /// <inheritdoc/>
        public string Export(ITranslationDatabase source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                NewLine = "\n",
            };

            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            using var csv = new CsvWriter(writer, config);

            var langs = source.Languages;

            csv.WriteField("key");
            foreach (var lang in langs)
                csv.WriteField(lang.LanguageIdentifier);
            csv.NextRecord();

            var rows = source is IKeyedTranslationDatabase
                ? source.OrderBy(e => e.Key, StringComparer.Ordinal)
                : source.AsEnumerable();

            foreach (var entry in rows)
            {
                csv.WriteField(entry.Key);
                foreach (var lang in langs)
                {
                    entry.TryGetTranslation(lang, out var val);
                    csv.WriteField(val ?? string.Empty);
                }
                csv.NextRecord();
            }

            writer.Flush();
            return sb.ToString();
        }
    }
}
