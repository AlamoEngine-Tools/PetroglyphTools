// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Data.Etl.Stage.Bean.Impl;

namespace PG.StarWarsGame.Localisation.Data.Etl.Stage.Bean
{
    public sealed class SortedTranslationStage1Bean : AStageBean
    {
        public SortedTranslationStage1Bean(string key, string value, string originFileName)
        {
            Key = key;
            Value = value;
            OriginFileName = originFileName;
        }

        public string OriginFileName { get; }
        public string Key { get; }
        public string Value { get; }
        public override string Id => Key;
    }
}
