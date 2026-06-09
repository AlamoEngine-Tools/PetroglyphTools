// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;
using PG.StarWarsGame.Localisation.Services;

namespace PG.StarWarsGame.Localisation.IO.Csv
{
    /// <summary>
    /// Default implementation of <see cref="ICsvTranslationImporter"/>.
    /// Expects a CSV with header row: <c>key,LANG1,LANG2,...</c>
    /// </summary>
    public sealed class CsvTranslationImporter : ICsvTranslationImporter
    {
        private readonly ILanguageService _languageService;

        /// <summary>Initialises a new instance with the given language service.</summary>
        public CsvTranslationImporter(ILanguageService languageService)
        {
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        }

        /// <inheritdoc/>
        public void Import(TextReader source, ITranslationDatabase target)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (target is null) throw new ArgumentNullException(nameof(target));

            var headerLine = source.ReadLine();
            if (headerLine is null) return;

            var headers = ParseCsvLine(headerLine);
            if (headers.Count < 2) return;

            var langColumns = new List<IAlamoLanguageDefinition?>();
            for (var i = 1; i < headers.Count; i++)
            {
                _languageService.TryGetByIdentifier(headers[i], out var lang);
                langColumns.Add(lang);
            }

            string? line;
            while ((line = source.ReadLine()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var fields = ParseCsvLine(line);
                if (fields.Count == 0) continue;

                var key = fields[0];
                for (var i = 1; i < fields.Count && i - 1 < langColumns.Count; i++)
                {
                    var lang = langColumns[i - 1];
                    if (lang is null) continue;
                    if (!target.Languages.Contains(lang)) continue;
                    target.SetTranslation(key, lang, fields[i]);
                }
            }
        }

        private static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var i = 0;
            while (i <= line.Length)
            {
                if (i == line.Length)
                {
                    fields.Add(string.Empty);
                    break;
                }

                if (line[i] == '"')
                {
                    i++;
                    var sb = new System.Text.StringBuilder();
                    while (i < line.Length)
                    {
                        if (line[i] == '"')
                        {
                            i++;
                            if (i < line.Length && line[i] == '"')
                            {
                                sb.Append('"');
                                i++;
                            }
                            else break;
                        }
                        else
                        {
                            sb.Append(line[i]);
                            i++;
                        }
                    }
                    fields.Add(sb.ToString());
                    if (i < line.Length && line[i] == ',') i++;
                }
                else
                {
                    var start = i;
                    while (i < line.Length && line[i] != ',') i++;
                    fields.Add(line.Substring(start, i - start));
                    if (i < line.Length) i++;
                }
            }
            return fields;
        }
    }
}
