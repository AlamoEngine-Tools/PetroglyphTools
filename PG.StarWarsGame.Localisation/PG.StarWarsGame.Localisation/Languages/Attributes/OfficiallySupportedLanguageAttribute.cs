// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Localisation.Languages.Attributes
{
    /// <summary>
    /// Marks a language definition as officially supported by the Alamo engine,
    /// optionally restricting support to specific game contexts.
    /// When no contexts are specified the language is considered supported in all game contexts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class OfficiallySupportedLanguageAttribute : Attribute
    {
        /// <summary>
        /// The game contexts in which this language is officially supported.
        /// An empty array means the language is supported in all game contexts.
        /// </summary>
        public AlamoGameContext[] SupportedContexts { get; }

        /// <inheritdoc cref="OfficiallySupportedLanguageAttribute"/>
        public OfficiallySupportedLanguageAttribute(params AlamoGameContext[] contexts)
        {
            SupportedContexts = contexts ?? Array.Empty<AlamoGameContext>();
        }
    }
}
