// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.Core.Data.Etl.Transform;
using PG.Core.Test.Data.Etl.Transform;
using PG.StarWarsGame.Localisation.Data.Etl.Stage.Bean;
using PG.StarWarsGame.Localisation.Data.Etl.Transform;

namespace PG.StarWarsGame.Localisation.Test.Data.Etl.Transform
{
    [TestClass]
    [TestCategory(TestUtility.TEST_TYPE_ETL)]
    public sealed class
        SortedTranslationTransformServiceTest : ATransformServiceTest<SortedTranslationStage1Bean,
            SortedTranslationStage2Bean>
    {
        protected override SortedTranslationStage1Bean GetMinimalPositiveStage1Bean()
        {
            return new SortedTranslationStage1Bean("KEY_00", "My Value!",
                "mastertextfile_english.dat");
        }

        protected override List<SortedTranslationStage1Bean> GetBadBeans()
        {
            List<SortedTranslationStage1Bean> badBeans = new List<SortedTranslationStage1Bean>
            {
                GetNullKeyBadBean(),
                GetInvalidKeyBadBean(),
                GetNullValueBadBean(),
                GetNullOriginFileNameBadBean(),
                GetInvalidOriginFileNameBadBean(),
                GetUnknownOriginFileNameBadBean()
            };
            return badBeans;
        }

        private SortedTranslationStage1Bean GetNullKeyBadBean()
        {
            return new SortedTranslationStage1Bean(null, "My Value!", "mastertextfile_english.dat");
        }

        private SortedTranslationStage1Bean GetInvalidKeyBadBean()
        {
            return new SortedTranslationStage1Bean("", "My Value!", "mastertextfile_english.dat");
        }

        private SortedTranslationStage1Bean GetNullValueBadBean()
        {
            return new SortedTranslationStage1Bean("KEY_00", null, "mastertextfile_english.dat");
        }

        private SortedTranslationStage1Bean GetNullOriginFileNameBadBean()
        {
            return new SortedTranslationStage1Bean("KEY_00", "My Value!", null);
        }

        private SortedTranslationStage1Bean GetInvalidOriginFileNameBadBean()
        {
            return new SortedTranslationStage1Bean("KEY_00", "My Value!", "");
        }

        private SortedTranslationStage1Bean GetUnknownOriginFileNameBadBean()
        {
            return new SortedTranslationStage1Bean("KEY_00", "My Value!", "some_random_name.txt");
        }

        protected override ITransformService<SortedTranslationStage1Bean, SortedTranslationStage2Bean> GetService(
            List<SortedTranslationStage1Bean> beans)
        {
            return new SortedTranslationTransformService(new MockFileSystem(), beans);
        }
    }
}
