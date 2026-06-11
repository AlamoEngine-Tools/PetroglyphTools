// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO.Dat;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Baseline
{
    internal sealed class BaselineTranslationProvider : ServiceBase, IBaselineTranslationProvider
    {
        // The 5 western languages shipped with the original games have embedded baseline data.
        // The remaining officially supported languages are mapped for future resource additions.
        private static readonly IReadOnlyDictionary<string, (string Folder, string NameSuffix)> LanguageMap =
            new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
            {
                ["ENGLISH"]  = ("en", "english"),
                ["GERMAN"]   = ("de", "german"),
                ["SPANISH"]  = ("es", "spanish"),
                ["FRENCH"]   = ("fr", "french"),
                ["ITALIAN"]  = ("it", "italian"),
                ["CHINESE"]  = ("zh", "chinese"),
                ["POLISH"]   = ("pl", "polish"),
                ["RUSSIAN"]  = ("ru", "russian"),
                ["JAPANESE"] = ("ja", "japanese"),
                ["KOREAN"]   = ("ko", "korean"),
                ["THAI"]     = ("th", "thai"),
            };

        private readonly IDatFileService _datFileService;
        private readonly IDatTranslationImporter _importer;
        private readonly ITranslationDatabaseFactory _factory;

        public BaselineTranslationProvider(IServiceProvider services) : base(services)
        {
            _datFileService = services.GetRequiredService<IDatFileService>();
            _importer       = services.GetRequiredService<IDatTranslationImporter>();
            _factory        = services.GetRequiredService<ITranslationDatabaseFactory>();
        }

        /// <inheritdoc/>
        public IKeyedTranslationDatabase GetMasterText(GameContext game, IAlamoLanguageDefinition language)
        {
            if (language is null) throw new ArgumentNullException(nameof(language));
            var db = _factory.CreateKeyed(new[] { language });
            LoadInto(db, game, "mastertextfile", language);
            return db;
        }

        /// <inheritdoc/>
        public IKeyedTranslationDatabase GetMasterText(GameContext game, IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            var db = _factory.CreateKeyed(languages);
            foreach (var lang in languages)
                LoadInto(db, game, "mastertextfile", lang);
            return db;
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase GetCreditsText(GameContext game, IAlamoLanguageDefinition language)
        {
            if (language is null) throw new ArgumentNullException(nameof(language));
            var db = _factory.CreateOrdered(new[] { language });
            LoadInto(db, game, "creditstext", language);
            return db;
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase GetCreditsText(GameContext game, IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            var db = _factory.CreateOrdered(languages);
            foreach (var lang in languages)
                LoadInto(db, game, "creditstext", lang);
            return db;
        }

        private void LoadInto(ITranslationDatabase db, GameContext game, string filePrefix, IAlamoLanguageDefinition language)
        {
            if (!LanguageMap.TryGetValue(language.LanguageIdentifier, out var map))
                return;

            var resourceName = BuildResourceName(game, map.Folder, $"{filePrefix}_{map.NameSuffix}.dat");
            using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resourceStream is null) return;

            var tempPath = FileSystem.Path.Combine(
                FileSystem.Path.GetTempPath(),
                $"pg_baseline_{Guid.NewGuid():N}.dat");

            try
            {
                using (var fs = FileSystem.FileStream.New(tempPath, FileMode.Create, FileAccess.Write))
                    resourceStream.CopyTo(fs);

                var datFile = _datFileService.Load(tempPath);
                _importer.Import(datFile.Content, language, db);
            }
            finally
            {
                try
                {
                    FileSystem.File.Delete(tempPath);
                }
                catch
                {
                    // NOP
                }
            }
        }

        private static string BuildResourceName(GameContext game, string langFolder, string fileName)
        {
            var gameFolder = game == GameContext.EaW ? "EaW" : "FoC";
            return $"PG.StarWarsGame.Localisation.Baseline.Resources.{gameFolder}.{langFolder}.{fileName}";
        }
    }
}
