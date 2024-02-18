// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

[TestClass]
public class MegDataEntryLocationTest
{
    [TestMethod]
    public void Test_Ctor()
    {
        var location = new MegDataEntryLocation(1, 2);
        Assert.AreEqual(1u, location.Offset);
        Assert.AreEqual(2u, location.Size);
    }

    [TestMethod]
    public void Test_CtorEmpty()
    {
        var location = new MegDataEntryLocation();
        Assert.AreEqual(0u, location.Offset);
        Assert.AreEqual(0u, location.Size);
    }

    [TestMethod]
    public void Test_Default()
    {
        var location = default(MegDataEntryLocation);
        Assert.AreEqual(0u, location.Offset);
        Assert.AreEqual(0u, location.Size);
    }

    [TestMethod]
    public void Test_Equality_HashCode()
    {
        var location1 = new MegDataEntryLocation(1, 2);
        var location2 = new MegDataEntryLocation(1, 2);
        var location3 = default(MegDataEntryLocation);

        Assert.AreEqual(location1, location2);
        Assert.AreEqual(location1.GetHashCode(), location2.GetHashCode());

        Assert.AreNotEqual(location1, location3);
        Assert.AreNotEqual(location1, new object());
        Assert.AreNotEqual(location1, (object?)null);
        Assert.AreNotEqual(location1.GetHashCode(), location3.GetHashCode());
    }
}