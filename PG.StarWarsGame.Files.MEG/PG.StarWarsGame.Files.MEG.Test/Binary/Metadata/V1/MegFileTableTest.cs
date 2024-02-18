// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;


[TestClass]
public class MegFileTableTest : MegFileTableBaseTest
{
    private static MegFileTable CreateFileTableV1(IList<MegFileTableRecord> files)
    {
        return new MegFileTable(files);
    }

    private protected override IMegFileTable CreateFileTable(IList<IMegFileDescriptor> files)
    {
        return CreateFileTableV1(files.Select(CreateFileRecordV1).ToList());
    }

    private static MegFileTableRecord CreateFileRecordV1(IMegFileDescriptor file)
    {
        return new MegFileTableRecord(file.Crc32, (uint)file.Index, file.FileSize, file.FileOffset, (uint)file.FileNameIndex);
    }

    private protected override IMegFileDescriptor CreateFile(uint index, uint seed)
    {
        return new MegFileTableRecord(new Crc32(seed), index, seed, seed, index);
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileTable(null!));
    }

    [TestMethod]
    public void Test_Index()
    {
        var entry1 = CreateFileRecordV1(CreateFile(0, 1));
        var entry2 = CreateFileRecordV1(CreateFile(1, 2));
        var table = CreateFileTableV1(new List<MegFileTableRecord>
        {
            entry1,
            entry2
        });

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(entry1, table[0]);
        Assert.AreEqual(entry2, table[1]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => table[2]);
    }
}