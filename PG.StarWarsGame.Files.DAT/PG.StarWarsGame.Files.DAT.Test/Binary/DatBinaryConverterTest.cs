// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Binary;

[TestClass]
public class DatBinaryConverterTest
{
    private readonly MockFileSystem _fileSystem = new();
    private DatBinaryConverter _binaryConverter = null!;
    private ICrc32HashingService _crc32HashingService = null!;

    [TestInitialize]
    public void Prepare()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PGDomain.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();

        _binaryConverter = new DatBinaryConverter(sp);
        _crc32HashingService = sp.GetRequiredService<ICrc32HashingService>();

    }

    [TestMethod]
    public void Test_ToHolder__ValidModelCreatesValidHolder()
    {
        
        _fileSystem.Directory.CreateDirectory(@"c:\tmp\");
        var binaryModel = new DatBinaryFile(
            new DatHeader(4),
            new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
            {
                new(_crc32HashingService.GetCrc32("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY0".Length,
                    (uint)"VALUE0".Length),
                new(_crc32HashingService.GetCrc32("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY1".Length,
                    (uint)"VALUE1".Length),
                new(_crc32HashingService.GetCrc32("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY2".Length,
                    (uint)"VALUE2".Length),
                new(_crc32HashingService.GetCrc32("KEY0", DatFileConstants.TextKeyEncoding), (uint)"KEY3".Length,
                    (uint)"VALUE3".Length)
            }.AsReadOnly()),
            new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
            {
                new("VALUE0"),
                new("VALUE1"),
                new("VALUE2"),
                new("VALUE3")
            }),
            new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
            {
                new("KEY0"),
                new("KEY1"),
                new("KEY2"),
                new("KEY3")
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