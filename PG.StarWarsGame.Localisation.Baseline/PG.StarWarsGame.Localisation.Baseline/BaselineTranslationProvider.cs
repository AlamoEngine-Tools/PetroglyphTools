// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO;
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
            return GetMasterText(game, new[] { language });
        }

        /// <inheritdoc/>
        public IKeyedTranslationDatabase GetMasterText(GameContext game, IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            var db = _factory.CreateKeyed(languages);
            _importer.ImportAll(LoadModels(game, "mastertextfile", languages), db);
            return db;
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase GetCreditsText(GameContext game, IAlamoLanguageDefinition language)
        {
            if (language is null) throw new ArgumentNullException(nameof(language));
            return GetCreditsText(game, new[] { language });
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase GetCreditsText(GameContext game, IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            var db = _factory.CreateOrdered(languages);

            // Credits are positional and an ordered database appends on every write, so the per-language
            // models must be merged row-wise rather than imported one after another. ImportAll owns that.
            _importer.ImportAll(LoadModels(game, "creditstext", languages), db);
            return db;
        }

        private IReadOnlyList<KeyValuePair<IAlamoLanguageDefinition, IDatModel>> LoadModels(
            GameContext game, string filePrefix, IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            var models = new List<KeyValuePair<IAlamoLanguageDefinition, IDatModel>>();

            foreach (var language in languages)
            {
                if (language is null) throw new ArgumentNullException(nameof(languages));

                // Languages without an embedded resource are skipped silently; they are mapped ahead of the
                // data being added, so an absent file is expected rather than an error.
                var model = LoadModel(game, filePrefix, language);
                if (model is not null)
                    models.Add(new KeyValuePair<IAlamoLanguageDefinition, IDatModel>(language, model));
            }

            return models;
        }

        private IDatModel? LoadModel(GameContext game, string filePrefix, IAlamoLanguageDefinition language)
        {
            if (!LanguageMap.TryGetValue(language.LanguageIdentifier, out var map))
                return null;

            var resourceName = BuildResourceName(game, map.Folder, $"{filePrefix}_{map.NameSuffix}.dat");
            using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resourceStream is null) return null;

            // IDatFileService only loads from a path or a FileSystemStream, never a plain Stream, so the
            // embedded resource has to be spooled out first. Load fully materialises the model, so it stays
            // valid once the temp file is gone.
            var tempDirectory = FileSystem.Path.GetTempPath();
            var tempPath = FileSystem.Path.Combine(
                tempDirectory,
                $"pg_baseline_{Guid.NewGuid():N}.dat");

            try
            {
                FileSystem.Directory.CreateDirectory(tempDirectory);

                using (var fs = FileSystem.FileStream.New(tempPath, FileMode.Create, FileAccess.Write))
                    resourceStream.CopyTo(fs);

                return _datFileService.Load(tempPath).Content;
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
