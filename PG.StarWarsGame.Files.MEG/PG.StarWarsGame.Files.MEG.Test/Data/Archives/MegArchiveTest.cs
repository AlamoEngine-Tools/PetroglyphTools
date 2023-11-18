using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

[TestClass]
public class MegArchiveTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_Throw_NullArgument()
    {
        _ = new MegArchive(null!);
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var entries = new List<MegDataEntry>();
        _ = new MegArchive(entries);
    }
}