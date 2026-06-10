// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;
using PG.StarWarsGame.Localisation.Languages.Attributes;

namespace PG.StarWarsGame.Localisation.Languages
{
    /// <summary>
    /// Base class for all Alamo language definitions.
    /// </summary>
    public abstract class AlamoLanguageDefinitionBase : IAlamoLanguageDefinition
    {
        private readonly bool _isOfficiallySupported;
        private readonly AlamoGameContext[] _supportedContexts;

        /// <inheritdoc/>
        public string LanguageIdentifier { get; }

        /// <inheritdoc/>
        public CultureInfo Culture { get; }

        /// <inheritdoc/>
        public bool IsDefault { get; }

        /// <summary>
        /// Gets the engine's language identifier string. Subclasses return a constant value.
        /// </summary>
        protected abstract string ConfiguredLanguageIdentifier { get; }

        /// <summary>
        /// Gets the .NET culture for this language. Subclasses return a constant value.
        /// </summary>
        protected abstract CultureInfo ConfiguredCulture { get; }

        /// <summary>Initialises the language definition by reading its attributes.</summary>
        protected AlamoLanguageDefinitionBase()
        {
            LanguageIdentifier = ConfiguredLanguageIdentifier;
            Culture = ConfiguredCulture;

            var attr = (OfficiallySupportedLanguageAttribute?)Attribute.GetCustomAttribute(
                GetType(), typeof(OfficiallySupportedLanguageAttribute));

            _isOfficiallySupported = attr != null;
            _supportedContexts = attr?.SupportedContexts ?? Array.Empty<AlamoGameContext>();
            IsDefault = GetType().IsDefined(typeof(DefaultLanguageAttribute), false);
        }

        /// <inheritdoc/>
        public bool IsOfficiallySupported() => _isOfficiallySupported;

        /// <inheritdoc/>
        public bool IsOfficiallySupported(AlamoGameContext context)
        {
            if (!_isOfficiallySupported) return false;
            return _supportedContexts.Length == 0 || Array.IndexOf(_supportedContexts, context) >= 0;
        }

        /// <inheritdoc/>
        public bool Equals(IAlamoLanguageDefinition? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(LanguageIdentifier, other.LanguageIdentifier, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is IAlamoLanguageDefinition other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            StringComparer.OrdinalIgnoreCase.GetHashCode(LanguageIdentifier);

        /// <inheritdoc/>
        public override string ToString() => LanguageIdentifier;
    }
}
