// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO.Dat
{
    /// <summary>
    /// Default implementation of <see cref="IDatTranslationExporter"/>.
    /// Produces a sorted (CRC32-ordered) model for <see cref="IKeyedTranslationDatabase"/>
    /// and an unsorted model for <see cref="IOrderedTranslationDatabase"/>.
    /// </summary>
    public sealed class DatTranslationExporter : IDatTranslationExporter
    {
        private readonly IServiceProvider _services;

        /// <summary>Initialises a new instance with the given service provider.</summary>
        public DatTranslationExporter(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc/>
        public IDatModel Export(ITranslationDatabase source, IAlamoLanguageDefinition language)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (language is null) throw new ArgumentNullException(nameof(language));

            IDatBuilder builder = source is IKeyedTranslationDatabase
                ? new EmpireAtWarMasterTextBuilder(false, _services)
                : new EmpireAtWarCreditsTextBuilder(_services);

            foreach (var entry in source)
            {
                if (entry.TryGetTranslation(language, out var value) && value is not null)
                    builder.AddEntry(entry.Key, value);
            }

            return builder.BuildModel();
        }
    }
}
