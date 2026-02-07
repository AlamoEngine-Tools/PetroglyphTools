// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Binary;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

public class MegFileConstantsTest
{
    [Fact]
    public void TestConstants()
    {
        Assert.Equal(uint.MaxValue, MegFileConstants.MegMaxEntrySize);
        Assert.Equal(uint.MaxValue, MegFileConstants.MegMaxFileSize);
        Assert.Equal(int.MaxValue, MegFileConstants.EawMegMaxEntrySize);
        Assert.Equal(int.MaxValue, MegFileConstants.EawMegMaxFileSize);
        Assert.Equal(ushort.MaxValue, MegFileConstants.MegMaxEntryPathLength);
        Assert.Equal(259, MegFileConstants.EawMaxEntryPathLength);
    }
}
