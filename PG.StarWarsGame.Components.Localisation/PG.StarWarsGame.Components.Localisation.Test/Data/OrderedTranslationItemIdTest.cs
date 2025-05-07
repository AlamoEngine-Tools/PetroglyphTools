// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Test.Data;
using PG.StarWarsGame.Components.Localisation.Repository.Content;

namespace PG.StarWarsGame.Components.Localisation.Test.Data;

public class OrderedTranslationItemIdTest : IIdTestBase<OrderedTranslationItemId>
{
    protected override List<OrderedTranslationItemId?> GetConfiguredNullIIds()
    {
        return
        [
            OrderedTranslationItemId.Of(string.Empty),
            OrderedTranslationItemId.Of(""),
            OrderedTranslationItemId.Of("\t")
        ];
    }

    protected override List<object[]> GetConfiguredValidIIdInitialisers()
    {
        return
        [
            new object[] { "TEST_00" },
            new object[] { "TEST_01" },
            new object[] { "TEST_02" },
            new object[] { "TEST_03" },
            new object[] { "TEST_04" },
            new object[] { "TEST_05" }
        ];
    }

    protected override OrderedTranslationItemId CreateId(object[] keyInitialiser)
    {
        return OrderedTranslationItemId.Of((keyInitialiser[0] as string)!) ?? throw new InvalidOperationException();
    }
}
