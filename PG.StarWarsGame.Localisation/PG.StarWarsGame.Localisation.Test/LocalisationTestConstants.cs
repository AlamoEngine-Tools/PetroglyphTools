// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Core.Test;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Test
{
    public sealed class LocalisationTestConstants : TestConstants
    {
        public static readonly IList<Type> REGISTERED_LANGUAGE_DEFINITIONS = new List<Type>
        {
            //[gruenwaldlu, 2021-04-18-12:02:51+2]: All officially supported languages are listed below. 
            typeof(ChineseAlamoLanguageDefinition)
            , typeof(EnglishAlamoLanguageDefinition)
            , typeof(FrenchAlamoLanguageDefinition)
            , typeof(GermanAlamoLanguageDefinition)
            , typeof(ItalianAlamoLanguageDefinition)
            , typeof(JapaneseAlamoLanguageDefinition)
            , typeof(KoreanAlamoLanguageDefinition)
            , typeof(PolishAlamoLanguageDefinition)
            , typeof(RussianAlamoLanguageDefinition)
            , typeof(SpanishAlamoLanguageDefinition)
            , typeof(ThaiAlamoLanguageDefinition)
        };

        public static readonly Type DEFAULT_LANGUAGE = typeof(EnglishAlamoLanguageDefinition);
    }
}
