// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using PG.Core.Data.Bean;

namespace PG.StarWarsGame.Localisation.Data.Translation
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
