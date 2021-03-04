// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Data.Etl.Stage.Bean.Impl;
using PG.Core.Localisation;

namespace PG.StarWarsGame.Localisation.Data.Etl.Transform
{
    public class SortedTranslationStage2Bean : AStageBean
    {
        public SortedTranslationStage2Bean(string key, string keyException, string value, string valueException, IAlamoLanguageDefinition languageDefinition, string languageDefinitionException)
        {
            Key = key;
            KeyException = keyException;
            Value = value;
            ValueException = valueException;
            LanguageDefinition = languageDefinition;
            LanguageDefinitionException = languageDefinitionException;
        }
        
        public IAlamoLanguageDefinition LanguageDefinition { get;}
        public string LanguageDefinitionException { get; }

        public string Key { get; }
        public string KeyException { get; }


        public string Value { get; }
        public string ValueException { get; }

        public override string Id => Key;
    }
}
