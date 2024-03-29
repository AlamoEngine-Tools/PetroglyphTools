// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Data.Key;
using PG.Core.Localisation;

namespace PG.StarWarsGame.Localisation.Data.Localisation
{
    public class LocalisationKey : AbstractKey<IAlamoLanguageDefinition>
    {
        public LocalisationKey(IAlamoLanguageDefinition key) : base(key)
        {
        }
    }
}
