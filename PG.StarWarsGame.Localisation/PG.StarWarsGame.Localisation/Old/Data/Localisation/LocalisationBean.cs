// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Data.Bean;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Localisation.Data.Localisation
{
    public class LocalisationBean : AbstractBean<LocalisationKey>
    {
        public string Localisation { get; set; }

        public LocalisationBean([NotNull] LocalisationKey key, string localisation) : base(key)
        {
            Localisation = localisation;
        }
    }
}
