using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;


public class MegArchiveTest
{
    [Fact]
    public void Test_Ctor_Throw_NullArgument()
    { 
        Assert.Throws<ArgumentNullException>(() => new MegArchive(null!));
    }

    [Fact]
    public void Test_Ctor()
    {
        var entries = new List<MegDataEntry>();
        _ = new MegArchive(entries);
    }
}