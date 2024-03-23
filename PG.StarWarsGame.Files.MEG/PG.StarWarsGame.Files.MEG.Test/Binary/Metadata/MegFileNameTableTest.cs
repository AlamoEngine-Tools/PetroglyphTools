// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;


[TestClass]
public class MegFileNameTableTest
{
    [TestMethod]
    public void Test_Enumerate_AsIMegFileNameTable()
    {
        var entry1 = MegFileNameTableRecordTest.CreateNameRecord("123");
        var entry2 = MegFileNameTableRecordTest.CreateNameRecord("456");

        var recordList = new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        };
        
        IMegFileNameTable table = new MegFileNameTable(recordList);
        var names = new List<MegFileNameTableRecord>();

        using var enumerator = table.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var currentTyped = enumerator.Current;
            var currentObj = ((IEnumerator)enumerator).Current;
            Assert.AreEqual(currentObj, currentTyped);

            names.Add(currentTyped);
        }

        CollectionAssert.AreEqual(recordList, names);
    }
}