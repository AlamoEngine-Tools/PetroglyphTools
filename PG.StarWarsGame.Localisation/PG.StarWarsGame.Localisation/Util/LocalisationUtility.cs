// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using PG.Commons.Util;
using PG.Core.Localisation;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Util
{
    /// <summary>
    /// A utility class for working with <see cref="IAlamoLanguageDefinition"/>s.
    /// </summary>
    public static class LocalisationUtility
    {
        private static IList<Type> s_iAlamoLanguageDefinitionTypesCache = new List<Type>();

        /// <summary>
        /// Tries to guess a <see cref="IAlamoLanguageDefinition"/> implementation from a given string identifier.
        /// The given identifier string is compared against the <see cref="IAlamoLanguageDefinition.LanguageIdentifier"/>
        /// for all implementations found in the current application domain and if the match is close enough
        /// (<see cref="string.Equals(string?, StringComparison)"/>) a new instance of the implementation is returned, if
        /// the implementation provides a parameterless constructor.
        /// </summary>
        /// <param name="identifier">The string that is matched against the <see cref="IAlamoLanguageDefinition.LanguageIdentifier"/></param>
        /// <param name="languageDefinition">The out parameter. If a match is found this is set to a new instance of an
        /// <see cref="IAlamoLanguageDefinition"/> implementation or the fallback (defaults to
        /// <see cref="EnglishAlamoLanguageDefinition"/> if no override is provided) if provided and no match is found.</param>
        /// <param name="fallback">An optional fallback of a <see cref="IAlamoLanguageDefinition"/>.</param>
        /// <returns>Returns true if a matching <see cref="IAlamoLanguageDefinition"/> is found. If no match is found,
        /// the function will return true, even if a fallback is provided.</returns>
        public static bool TryGuessAlamoLanguageDefinitionByIdentifier([NotNull] string identifier,
            [CanBeNull] out IAlamoLanguageDefinition languageDefinition,
            [CanBeNull] IAlamoLanguageDefinition fallback = null)
        {
            fallback ??= new EnglishAlamoLanguageDefinition();
            languageDefinition = fallback;
            if (!StringUtility.HasText(identifier))
            {
                return false;
            }

            identifier = identifier.Trim();

            if (s_iAlamoLanguageDefinitionTypesCache == null || !s_iAlamoLanguageDefinitionTypesCache.Any())
            {
                BuildIAlamoLanguageDefinitionCache();
            }

            Debug.Assert(s_iAlamoLanguageDefinitionTypesCache != null,
                nameof(s_iAlamoLanguageDefinitionTypesCache) + " != null");
            foreach (Type probableType in s_iAlamoLanguageDefinitionTypesCache)
            {
                try
                {
                    IAlamoLanguageDefinition a = Activator.CreateInstance(probableType) as IAlamoLanguageDefinition;

                    if (a != null && !identifier.Equals(a.LanguageIdentifier, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    languageDefinition = a;
                    return true;
                }
                catch (Exception)
                {
                    // NOP - Type doesn't have a parameter-less constructor, so we're screwed anyways.
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether a given <see cref="IAlamoLanguageDefinition"/> is an officially supported language. 
        /// </summary>
        /// <param name="alamoLanguageDefinition"></param>
        /// <returns></returns>
        public static bool IsOfficiallySupportedLanguage(IAlamoLanguageDefinition alamoLanguageDefinition)
        {
            return OfficialLanguageExtension.TryParse(alamoLanguageDefinition.LanguageIdentifier, out OfficialLanguage _);
        }

        private static void BuildIAlamoLanguageDefinitionCache()
        {
            s_iAlamoLanguageDefinitionTypesCache = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()).Where(p =>
                    typeof(IAlamoLanguageDefinition).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract).ToList();
        }
    }
}
