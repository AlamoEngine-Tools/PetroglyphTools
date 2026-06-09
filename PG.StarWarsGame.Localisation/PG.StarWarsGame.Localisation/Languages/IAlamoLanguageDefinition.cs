// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Languages
{
    /// <summary>
    /// Describes a language supported by the Alamo game engine.
    /// </summary>
    public interface IAlamoLanguageDefinition : IEquatable<IAlamoLanguageDefinition>
    {
        /// <summary>
        /// Gets the engine's internal language identifier (e.g. <c>"ENGLISH"</c>).
        /// </summary>
        string LanguageIdentifier { get; }

        /// <summary>
        /// Gets the .NET culture associated with this language.
        /// </summary>
        CultureInfo Culture { get; }

        /// <summary>
        /// Gets a value indicating whether this language is officially supported by the Alamo engine.
        /// </summary>
        bool IsOfficiallySupported { get; }

        /// <summary>
        /// Gets a value indicating whether this is the default game language (English).
        /// </summary>
        bool IsDefault { get; }
    }
}
