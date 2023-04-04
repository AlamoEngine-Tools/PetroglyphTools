// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.Core.Attributes;
using PG.Core.Localisation;
using PG.StarWarsGame.Localisation.Commons;

namespace PG.StarWarsGame.Localisation.Util
{
    public static class LocalisationUtility
    {
        private static readonly IEnumerable<Type> ALAMO_LANGUAGE_DEFINITION_TYPE_CACHE;

        static LocalisationUtility()
        {
            ALAMO_LANGUAGE_DEFINITION_TYPE_CACHE = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(p =>
                    typeof(IAlamoLanguageDefinition).IsAssignableFrom(p) &&
                    typeof(AbstractAlamoLanguageDefinition).IsAssignableFrom(p) &&
                    p.IsClass &&
                    !p.IsAbstract &&
                    !p.IsInterface
                ).ToList();
        }

        public static IList<IAlamoLanguageDefinition> GetAllAlamoLanguageDefinitions()
        {
            List<IAlamoLanguageDefinition> alamoLanguageDefinitions = new List<IAlamoLanguageDefinition>();
            foreach (Type type in ALAMO_LANGUAGE_DEFINITION_TYPE_CACHE)
            {
                try
                {
                    IAlamoLanguageDefinition languageDefinition =
                        Activator.CreateInstance(type) as IAlamoLanguageDefinition;
                    alamoLanguageDefinitions.Add(languageDefinition);
                }
                catch
                {
                    // NOP - this definition is faulty.
                }
            }
            return alamoLanguageDefinitions;
        }

        public static IAlamoLanguageDefinition GetDefaultAlamoLanguageDefinition()
        {
            foreach (Type type in ALAMO_LANGUAGE_DEFINITION_TYPE_CACHE)
            {
                if (!type.GetAttributeValueOrDefault((DefaultAttribute d) => d.IsDefault))
                {
                    continue;
                }

                try
                {
                    return Activator.CreateInstance(type) as IAlamoLanguageDefinition;
                }
                catch (Exception e)
                {
                    throw new DefaultAlamoLanguageDefinitionException(
                        $"Could not create an instance of the default {nameof(IAlamoLanguageDefinition)} specification \"{type}\".", e);
                }
            }

            throw new DefaultAlamoLanguageDefinitionException(
                $"No default {nameof(IAlamoLanguageDefinition)} specified.");
        }
    }
}
