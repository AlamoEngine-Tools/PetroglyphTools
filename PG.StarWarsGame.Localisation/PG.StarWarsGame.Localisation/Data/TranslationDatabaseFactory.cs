// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Data.Config;
using PG.StarWarsGame.Localisation.Data.Internal;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// Default implementation of <see cref="ITranslationDatabaseFactory"/>.
    /// </summary>
    public sealed class TranslationDatabaseFactory : ITranslationDatabaseFactory
    {
        /// <inheritdoc/>
        public IKeyedTranslationDatabase CreateKeyed(IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            return new KeyedTranslationDatabase(languages);
        }

        /// <inheritdoc/>
        public IKeyedTranslationDatabase CreateKeyed(ITranslationProjectDescriptor descriptor)
        {
            if (descriptor is null) throw new ArgumentNullException(nameof(descriptor));
            return CreateKeyed(descriptor.Languages);
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase CreateOrdered(IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            return new OrderedTranslationDatabase(languages);
        }

        /// <inheritdoc/>
        public IOrderedTranslationDatabase CreateOrdered(ITranslationProjectDescriptor descriptor)
        {
            if (descriptor is null) throw new ArgumentNullException(nameof(descriptor));
            return CreateOrdered(descriptor.Languages);
        }

        /// <inheritdoc/>
        public ITranslationDatabaseBuilder CreateDatabase() => new TranslationDatabaseBuilder();
    }
}
