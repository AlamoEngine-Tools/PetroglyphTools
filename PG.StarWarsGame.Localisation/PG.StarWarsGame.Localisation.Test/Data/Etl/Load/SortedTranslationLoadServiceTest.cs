// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.Core.Data.Etl.Load;
using PG.Core.Test.Data.Etl.Load;
using PG.StarWarsGame.Localisation.Data.Etl.Load;
using PG.StarWarsGame.Localisation.Data.Etl.Stage.Bean;
using PG.StarWarsGame.Localisation.Data.Translation;
using PG.StarWarsGame.Localisation.Languages;
using PG.StarWarsGame.Localisation.Util;

namespace PG.StarWarsGame.Localisation.Test.Data.Etl.Load
{
    [TestClass]
    [TestCategory(TestUtility.TEST_TYPE_ETL)]
    public class SortedTranslationLoadServiceTest : ALoadServiceTest<SortedTranslationStage2Bean, TranslationItem>
    {
        protected override SortedTranslationStage2Bean GetMinimalPositiveStage2Bean()
        {
            return new SortedTranslationStage2Bean("MY_TEST_KEY_", null, "My test value!", null,
                LocalisationUtility.GetDefaultAlamoLanguageDefinition(), null);
        }

        protected override ILoadService<SortedTranslationStage2Bean, TranslationItem> GetService(
            List<SortedTranslationStage2Bean> beans)
        {
            return new SortedTranslationLoadService(null, GetStage2Beans());
        }
    }
}
