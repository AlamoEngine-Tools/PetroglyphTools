// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Collections;

namespace PG.Commons.Test.Collections;

[TestClass]
public class ReadOnlyFrugalList_Test_String : ReadOnlyFrugalListTestBase<string>
{
    protected override string CreateT(int seed)
    {
        var stringLength = seed % 10 + 5;
        var rand = new Random(seed);
        var bytes = new byte[stringLength];
        rand.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}

[TestClass]
public class ReadOnlyFrugalList_Test_Int : ReadOnlyFrugalListTestBase<int>
{
    protected override int CreateT(int seed)
    {
        var rand = new Random(seed);
        return rand.Next();
    }
}


[TestClass]
public class ReadOnlyFrugalList_Test_Int_FromFrugal : ReadOnlyFrugalListTestBase<int>
{
    protected override int CreateT(int seed)
    {
        var rand = new Random(seed);
        return rand.Next();
    }

    protected override ReadOnlyFrugalList<int> GenericReadOnlyListFrugalListFactory(IEnumerable<int> enumerable)
    {
        var frugal = new FrugalList<int>(enumerable);
        return frugal.AsReadOnly();
    }
}