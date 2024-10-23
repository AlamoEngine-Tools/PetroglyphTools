// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Components.Localisation.Languages.BuiltIn;

namespace PG.StarWarsGame.Components.Localisation.Test;

public sealed class LocalisationTestConstants
{
    public static readonly IList<Type> RegisteredLanguageDefinitions = new List<Type>
    {
        //[gruenwaldlu, 2021-04-18-12:02:51+2]: All officially supported languages are listed below.
        typeof(ChineseAlamoLanguageDefinition), typeof(EnglishAlamoLanguageDefinition),
        typeof(FrenchAlamoLanguageDefinition), typeof(GermanAlamoLanguageDefinition),
        typeof(ItalianAlamoLanguageDefinition), typeof(JapaneseAlamoLanguageDefinition),
        typeof(KoreanAlamoLanguageDefinition), typeof(PolishAlamoLanguageDefinition),
        typeof(RussianAlamoLanguageDefinition), typeof(SpanishAlamoLanguageDefinition),
        typeof(ThaiAlamoLanguageDefinition)
    };

    public static readonly Type DefaultLanguage = typeof(EnglishAlamoLanguageDefinition);

    public static readonly string XmlV1ResourcePathBase =
        $"{typeof(LocalisationTestConstants).Assembly.Location}.Resources.IO.Xml.v1.Seralizable.translation_manifest_";
}
