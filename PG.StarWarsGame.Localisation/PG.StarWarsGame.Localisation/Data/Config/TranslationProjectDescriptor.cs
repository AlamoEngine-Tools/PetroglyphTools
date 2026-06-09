// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Localisation.Data.Config.v2;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Config
{
    /// <summary>
    /// Immutable descriptor for a translation project.
    /// </summary>
    public sealed class TranslationProjectDescriptor : ITranslationProjectDescriptor,
        IEquatable<TranslationProjectDescriptor>
    {
        /// <inheritdoc/>
        public GameType Game { get; }

        /// <inheritdoc/>
        public OverrideType OverrideType { get; }

        /// <inheritdoc/>
        public TranslationResourceType ResourceType { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IAlamoLanguageDefinition> Languages { get; }

        /// <summary>Initialises a new <see cref="TranslationProjectDescriptor"/>.</summary>
        public TranslationProjectDescriptor(
            GameType game,
            OverrideType overrideType,
            TranslationResourceType resourceType,
            IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            if (languages is null)
                throw new ArgumentNullException(nameof(languages));

            Game = game;
            OverrideType = overrideType;
            ResourceType = resourceType;
            Languages = languages.ToList().AsReadOnly();
        }

        /// <inheritdoc/>
        public bool Equals(TranslationProjectDescriptor? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Game == other.Game
                   && OverrideType == other.OverrideType
                   && ResourceType == other.ResourceType
                   && Languages.SequenceEqual(other.Languages);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is TranslationProjectDescriptor other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = HashCode.Combine((int)Game, (int)OverrideType, (int)ResourceType);
            foreach (var lang in Languages)
                hash = HashCode.Combine(hash, lang.GetHashCode());
            return hash;
        }
    }
}
