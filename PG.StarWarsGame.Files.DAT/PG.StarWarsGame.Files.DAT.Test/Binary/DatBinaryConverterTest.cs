// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Test.Binary;

[TestClass]
public class DatBinaryConverterTest
{
    private readonly MockFileSystem _fileSystem = new();
    private DatBinaryConverter _binaryConverter = null!;
    private readonly IChecksumService _checksumService = new ChecksumService();

    [TestInitialize]
    public void Prepare()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        _binaryConverter = new DatBinaryConverter(sc.BuildServiceProvider());
    }

    [TestMethod]
    public void Test__ToHolder__ValidModelCreatesValidHolder()
    {
        _fileSystem.Directory.CreateDirectory(@"c:\tmp\");
        var binaryModel = new DatFile(
            new DatHeader(4),
            new IndexTable(new List<IndexTableRecord>
            {
                new(_checksumService.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY0".Length,
                    (uint)"VALUE0".Length),
                new(_checksumService.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY1".Length,
                    (uint)"VALUE1".Length),
                new(_checksumService.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY2".Length,
                    (uint)"VALUE2".Length),
                new(_checksumService.GetChecksum("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY3".Length,
                    (uint)"VALUE3".Length)
            }.AsReadOnly()),
            new ValueTable(new List<ValueTableRecord>
            {
                new("VALUE0"),
                new("VALUE1"),
                new("VALUE2"),
                new("VALUE3")
            }),
            new KeyTable(new List<KeyTableRecord>
            {
                new("KEY0", new Crc32(1)),
                new("KEY1", new Crc32(2)),
                new("KEY2", new Crc32(3)),
                new("KEY3", new Crc32(4))
            }));

        var model = _binaryConverter.BinaryToModel(binaryModel);
        Assert.IsNotNull(model);

        Assert.AreEqual(binaryModel.RecordNumber, model.Count);
        for (var i = 0; i < binaryModel.RecordNumber; i++)
        {
            Assert.AreEqual(binaryModel.KeyTable[i].Key, model[i].Key);
            Assert.AreEqual(binaryModel.ValueTable[i].Value, model[i].Value);
        }
    }
}