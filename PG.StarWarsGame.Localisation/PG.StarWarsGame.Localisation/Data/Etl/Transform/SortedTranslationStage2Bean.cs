// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Localisation;

namespace PG.StarWarsGame.Localisation.Data.Etl.Transform
{
    public struct SortedTranslationStage2Bean
    {
        public IAlamoLanguageDefinition LanguageDefinition { get; set; }
        public string LanguageDefinitionException { get; set; }
        
        public string Key { get; set; }
        public string KeyException { get; set; }
        
        
        public string Value { get; set; }
        public string ValueException { get; set; }
    }
}
