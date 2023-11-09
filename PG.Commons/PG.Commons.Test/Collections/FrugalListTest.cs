using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Commons.Test.Collections;

[TestClass]
public class FrugalList_Test_String : FrugalListTestBase<string>
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
public class FrugalList_Test_Int : FrugalListTestBase<int>
{
    protected override int CreateT(int seed)
    {
        var rand = new Random(seed);
        return rand.Next();
    }
}