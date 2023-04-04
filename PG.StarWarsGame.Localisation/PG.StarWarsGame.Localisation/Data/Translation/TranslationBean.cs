// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Data.Bean;
using PG.Core.Data.Repository;
using PG.StarWarsGame.Localisation.Data.Localisation;

namespace PG.StarWarsGame.Localisation.Data.Translation
{
    public class TranslationBean : AbstractInMemoryKeyValuePairRepository<LocalisationKey, LocalisationBean>,
        IBean<TranslationKey>
    {
        public TranslationKey Key { get; }
    }
}
