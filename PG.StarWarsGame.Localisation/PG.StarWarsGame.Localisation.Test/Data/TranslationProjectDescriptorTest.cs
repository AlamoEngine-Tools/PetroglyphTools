// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Data.Config;
using PG.StarWarsGame.Localisation.Data.Config.v2;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.Data;

public class TranslationProjectDescriptorTest
{
    private static readonly IReadOnlyList<IAlamoLanguageDefinition> TwoLangs =
        new IAlamoLanguageDefinition[] { new EnglishAlamoLanguageDefinition(), new GermanAlamoLanguageDefinition() };

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var d = new TranslationProjectDescriptor(GameContext.EaW, OverrideType.Core, TranslationResourceType.Dat, TwoLangs);
        Assert.Equal(GameContext.EaW, d.Game);
        Assert.Equal(OverrideType.Core, d.OverrideType);
        Assert.Equal(TranslationResourceType.Dat, d.ResourceType);
        Assert.Equal(2, d.Languages.Count);
    }

    [Fact]
    public void Constructor_Throws_WhenLanguagesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TranslationProjectDescriptor(GameContext.EaW, OverrideType.Core, TranslationResourceType.Dat, null!));
    }

    [Fact]
    public void Languages_IsImmutable()
    {
        var mutable = new List<IAlamoLanguageDefinition> { new EnglishAlamoLanguageDefinition() };
        var d = new TranslationProjectDescriptor(GameContext.EaW, OverrideType.Core, TranslationResourceType.Xml, mutable);
        mutable.Add(new GermanAlamoLanguageDefinition());
        Assert.Single(d.Languages);
    }

    [Fact]
    public void Equality_TwoDescriptorsWithSameValues_AreEqual()
    {
        var a = new TranslationProjectDescriptor(GameContext.FoC, OverrideType.Mod, TranslationResourceType.Csv, TwoLangs);
        var b = new TranslationProjectDescriptor(GameContext.FoC, OverrideType.Mod, TranslationResourceType.Csv, TwoLangs);
        Assert.Equal(a, b);
    }
}
