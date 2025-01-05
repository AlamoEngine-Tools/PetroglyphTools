﻿using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

public class VirtualMegArchiveTest
{
    [Fact]
    public void Ctor_Throw_NullArgument()
    {
        Assert.Throws<ArgumentNullException>(() => new VirtualMegArchive(null!));
    }

    [Fact]
    public void Ctor()
    {
        var entries = new List<MegDataEntryReference>();
        _ = new VirtualMegArchive(entries);
    }
}