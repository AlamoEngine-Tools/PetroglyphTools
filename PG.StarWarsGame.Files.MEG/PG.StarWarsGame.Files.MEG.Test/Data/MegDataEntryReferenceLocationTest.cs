using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class MegDataEntryReferenceLocationTest
{
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegDataEntryReferenceLocation(null!, MegDataEntryTest.CreateEntry("path")));
    }
}