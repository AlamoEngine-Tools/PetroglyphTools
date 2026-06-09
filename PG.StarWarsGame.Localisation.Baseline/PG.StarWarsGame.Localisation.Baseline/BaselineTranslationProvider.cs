// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Data.Config.v2;
using PG.StarWarsGame.Localisation.IO.Dat;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Baseline
{
    /// <summary>
    /// Default implementation of <see cref="IBaselineTranslationProvider"/>.
    /// Loads translation data from embedded EaW and FoC game DAT files.
    /// </summary>
    public sealed class BaselineTranslationProvider : IBaselineTranslationProvider
    {
        // Only 5 languages have baseline data (the ones that shipped with the original games).
        private static readonly IReadOnlyDictionary<string, (string Folder, string NameSuffix)> LanguageMap =
            new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
            {
                ["ENGLISH"] = ("en", "english"),
                ["GERMAN"]  = ("de", "german"),
                ["SPANISH"] = ("es", "spanish"),
                ["FRENCH"]  = ("fr", "french"),
                ["ITALIAN"] = ("it", "italian"),
            };

        private readonly IFileSystem _fileSystem;
        private readonly IDatFileService _datFileService;
        private readonly IDatTranslationImporter _importer;
        private readonly ITranslationDatabaseFactory _factory;

        /// <summary>Initialises a new instance using services from the given provider.</summary>
        public BaselineTranslationProvider(IServiceProvider services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            _fileSystem     = services.GetRequiredService<IFileSystem>();
            _datFileService = services.GetRequiredService<IDatFileService>();
            _importer       = services.GetRequiredService<IDatTranslationImporter>();
            _factory        = services.GetRequiredService<ITranslationDatabaseFactory>();
        }

        /// <inheritdoc/>
        public IKeyedTranslationDatabase GetMasterText(GameType game, IAlamoLanguageDefinition language)
        {
            if (language is null) throw new ArgumentNullException(nameof(language));
            ValidateGame(game);
            var db = _factory.CreateKeyed(new[] { language });
            LoadInto(db, game, "mastertextfile", language);
            return db;
        }

        /// <inheritdoc/>
        public IKeyedTranslationDatabase GetMasterText(GameType game, IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            ValidateGame(game);
            var db = _factory.CreateKeyed(languages);
            foreach (var lang in languages)
                LoadInto(db, game, "mastertextfile", lang);
            return db;
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase GetCreditsText(GameType game, IAlamoLanguageDefinition language)
        {
            if (language is null) throw new ArgumentNullException(nameof(language));
            ValidateGame(game);
            var db = _factory.CreateOrdered(new[] { language });
            LoadInto(db, game, "creditstext", language);
            return db;
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase GetCreditsText(GameType game, IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            ValidateGame(game);
            var db = _factory.CreateOrdered(languages);
            foreach (var lang in languages)
                LoadInto(db, game, "creditstext", lang);
            return db;
        }

        private void LoadInto(ITranslationDatabase db, GameType game, string filePrefix, IAlamoLanguageDefinition language)
        {
            if (!LanguageMap.TryGetValue(language.LanguageIdentifier, out var map))
                return;

            var resourceName = BuildResourceName(game, map.Folder, $"{filePrefix}_{map.NameSuffix}.dat");
            using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resourceStream is null) return;

            var tempPath = _fileSystem.Path.Combine(
                _fileSystem.Path.GetTempPath(),
                $"pg_baseline_{Guid.NewGuid():N}.dat");

            try
            {
                using (var fs = _fileSystem.FileStream.New(tempPath, FileMode.Create, FileAccess.Write))
                    resourceStream.CopyTo(fs);

                var datFile = _datFileService.Load(tempPath);
                _importer.Import(datFile.Content, language, db);
            }
            finally
            {
                if (_fileSystem.File.Exists(tempPath))
                    _fileSystem.File.Delete(tempPath);
            }
        }

        private static string BuildResourceName(GameType game, string langFolder, string fileName)
        {
            var gameFolder = game == GameType.EaW ? "EaW" : "FoC";
            return $"PG.StarWarsGame.Localisation.Baseline.Resources.{gameFolder}.{langFolder}.{fileName}";
        }

        private static void ValidateGame(GameType game)
        {
            if (game != GameType.EaW && game != GameType.FoC)
                throw new ArgumentException(
                    $"Baseline data is only available for EaW and FoC, not '{game}'.", nameof(game));
        }
    }
}
