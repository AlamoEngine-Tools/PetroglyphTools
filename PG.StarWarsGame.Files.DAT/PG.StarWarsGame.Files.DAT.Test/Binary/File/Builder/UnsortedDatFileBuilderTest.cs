// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.DAT.Binary.File.Builder;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.File.Builder;

[TestClass]
public class UnsortedDatFileBuilderTest
{
    [TestMethod]
    public void FromHolder()
    {
        var translations = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("KEY_00", "VALUE"),
            new Tuple<string, string>("KEY_01", "VALUE"),
            new Tuple<string, string>("KEY_02", "VALUE"),
            new Tuple<string, string>("KEY_03", "VALUE"),
            new Tuple<string, string>("KEY_04", "VALUE")
        };
        var unsortedDatFileHolder = new UnsortedDatFileHolder("", "") {Content = translations};
        var builder = new UnsortedDatFileBuilder();
        var file = builder.FromHolder(unsortedDatFileHolder);
        Assert.IsNotNull(file);
        Assert.AreEqual((uint) translations.Count, file.KeyValuePairCount);
        var expectedOrder = new List<string>
        {
            "KEY_00",
            "KEY_01",
            "KEY_02",
            "KEY_03",
            "KEY_04",
        };
        for (var i = 0; i < expectedOrder.Count; i++)
        {
            Debug.Assert(file.Keys != null, "file.Keys != null");
            Debug.Assert(file.Keys[i] != null, $"file.Keys[{i}] != null");
            Assert.AreEqual(expectedOrder[i], file.Keys[i].Key);
        }
    }

    [TestMethod]
    public void FromBytes()
    {
        var unsortedDatFileHolder = new UnsortedDatFileHolder("", "")
        {
            Content = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("KEY_00", "VALUE"),
                new Tuple<string, string>("KEY_01", "VALUE"),
                new Tuple<string, string>("KEY_02", "VALUE"),
                new Tuple<string, string>("KEY_03", "VALUE"),
                new Tuple<string, string>("KEY_04", "VALUE")
            }
        };
        var builder = new UnsortedDatFileBuilder();
        var file = builder.FromHolder(unsortedDatFileHolder);
        var bytes = file.ToBytes();
        var fileFromBytes = builder.FromBytes(bytes);
        Assert.AreEqual(file, fileFromBytes);
    }
}
