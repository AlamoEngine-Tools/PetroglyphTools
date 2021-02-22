// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Util;
using PG.Core.Localisation;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Util
{
    public static class LocalisationUtility
    {
        public static bool TryGuessAlamoLanguageDefinitionByIdentifier(string identifier,
            out IAlamoLanguageDefinition languageDefinition, IAlamoLanguageDefinition fallback = null)
        {
            fallback ??= new EnglishAlamoLanguageDefinition();
            languageDefinition = fallback;
            if (!StringUtility.HasText(identifier))
            {
                return false;
            }

            identifier = identifier.Trim();

            List<Type> probableTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p =>
                typeof(IAlamoLanguageDefinition).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract).ToList();

            foreach (Type probableType in probableTypes)
            {
                try
                {
                    IAlamoLanguageDefinition a = Activator.CreateInstance(probableType) as IAlamoLanguageDefinition;
                    if (a == null)
                    {
                        continue;
                    }
                    if (!identifier.Equals(a.LanguageIdentifier, StringComparison.InvariantCultureIgnoreCase))
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
    }
}
