// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PG.StarWarsGame.Localisation.Data.Config.v2;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Config
{
    /// <summary>
    /// Immutable descriptor for a translation project.
    /// </summary>
    public sealed class TranslationProjectDescriptor : IEquatable<TranslationProjectDescriptor>
    {
        private readonly HashSet<IAlamoLanguageDefinition> _languagesSet;

        /// <summary>Gets the game this project provides translations for.</summary>
        public GameContext Game { get; }

        /// <summary>Gets whether this project provides core, expansion, or mod text.</summary>
        public OverrideType OverrideType { get; }

        /// <summary>Gets the file format used to store translation resources.</summary>
        public TranslationResourceType ResourceType { get; }

        /// <summary>Gets the languages covered by this translation project.</summary>
        public IReadOnlyCollection<IAlamoLanguageDefinition> Languages { get; }

        /// <summary>Initialises a new <see cref="TranslationProjectDescriptor"/>.</summary>
        public TranslationProjectDescriptor(
            GameContext game,
            OverrideType overrideType,
            TranslationResourceType resourceType,
            IEnumerable<IAlamoLanguageDefinition> languages)
        {
            if (languages is null)
                throw new ArgumentNullException(nameof(languages));

            _languagesSet = new HashSet<IAlamoLanguageDefinition>(languages);
            Languages = new ReadOnlyCollection<IAlamoLanguageDefinition>(_languagesSet.ToList());
            Game = game;
            OverrideType = overrideType;
            ResourceType = resourceType;
        }

        /// <inheritdoc/>
        public bool Equals(TranslationProjectDescriptor? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Game == other.Game
                   && OverrideType == other.OverrideType
                   && ResourceType == other.ResourceType
                   && _languagesSet.SetEquals(other._languagesSet);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is TranslationProjectDescriptor other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = HashCode.Combine((int)Game, (int)OverrideType, (int)ResourceType);
            foreach (var lang in _languagesSet.OrderBy(l => l.LanguageIdentifier, StringComparer.Ordinal))
                hash = HashCode.Combine(hash, lang.GetHashCode());
            return hash;
        }
    }
}
