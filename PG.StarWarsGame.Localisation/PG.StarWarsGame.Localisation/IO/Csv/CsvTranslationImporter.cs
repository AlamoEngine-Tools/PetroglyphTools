// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
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

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
            };

            using var csv = new CsvReader(source, config);

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;
            if (headers is null || headers.Length < 2) return;

            var langColumns = new IAlamoLanguageDefinition?[headers.Length - 1];
            for (var i = 1; i < headers.Length; i++)
            {
                _languageService.TryGetByIdentifier(headers[i], out langColumns[i - 1]);
            }

            var dbLanguages = target.Languages;

            while (csv.Read())
            {
                var key = csv.GetField(0);
                if (key is null) continue;

                for (var i = 1; i < headers.Length; i++)
                {
                    var lang = langColumns[i - 1];
                    if (lang is null) continue;
                    if (!dbLanguages.Any(l => l.Equals(lang))) continue;
                    var val = csv.GetField(i);
                    if (val is null) continue;
                    target.SetTranslation(key, lang, val);
                }
            }
        }
    }
}
