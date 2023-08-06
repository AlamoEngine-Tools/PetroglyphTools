// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Files;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Files;
using PG.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Binary;

[TestClass]
public class DatFileConverterTest : ServiceTestBase
{
    private class TestParam : IFileHolderParam
    {
    }

    private class NullDatModel : IDatFileMetadata
    {
        public byte[] Bytes { get; } = null!;
        public int Size { get; }
        public int RecordNumber { get; }
        public IIndexTable IndexTable { get; }
        public IKeyTable KeyTable { get; }
        public IValueTable ValueTable { get; }
    }

    protected override Type GetServiceClass()
    {
        return typeof(DatFileConverter);
    }

    private DatFileConverter GetService()
    {
        var svc = GetServiceInstance();
        return (DatFileConverter)svc;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test__ToHolder__InvalidParameterShouldThrowException()
    {
        GetService().ToHolder(new TestParam(), new NullDatModel());
    }

    [TestMethod]
    public void Test__ToHolder__ValidModelCreatesValidHolder()
    {
        var fs = (IFileSystem)TestConstants.Services.GetService(typeof(IFileSystem));
        fs.Directory.CreateDirectory(@"c:\tmp\");
        var model = new DatFile(
            new DatHeader(4),
            new IndexTable(new List<IndexTableRecord>()
            {
                new(ChecksumService.Instance.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY0".Length,
                    (uint)"VALUE0".Length),
                new(ChecksumService.Instance.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY1".Length,
                    (uint)"VALUE1".Length),
                new(ChecksumService.Instance.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY2".Length,
                    (uint)"VALUE2".Length),
                new(ChecksumService.Instance.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY3".Length,
                    (uint)"VALUE3".Length)
            }.AsReadOnly()),
            new ValueTable(new List<ValueTableRecord>()
            {
                new("VALUE0"),
                new("VALUE1"),
                new("VALUE2"),
                new("VALUE3")
            }),
            new KeyTable(new List<KeyTableRecord>()
            {
                new("KEY0"),
                new("KEY1"),
                new("KEY2"),
                new("KEY3")
            }));
        var param = new DatFileHolderParam()
        {
            FilePath = "c:/tmp/file.dat",
            Order = DatFileType.NotOrdered
        };
        var holder = GetService().ToHolder(param, model);
        Assert.IsNotNull(holder);
        Assert.IsNotNull(holder.Content);
        Assert.IsFalse(string.IsNullOrWhiteSpace(holder.FilePath));
        Assert.IsFalse(string.IsNullOrWhiteSpace(holder.Directory));
        Assert.IsFalse(string.IsNullOrWhiteSpace(holder.FileName));

        Assert.AreEqual(model.RecordNumber, holder.Content.Count);
        for (var i = 0; i < model.RecordNumber; i++)
        {
            Assert.AreEqual(model.KeyTable[i].Key, holder.Content[i].Key);
            Assert.AreEqual(model.ValueTable[i].Value, holder.Content[i].Value);
        }
    }
}