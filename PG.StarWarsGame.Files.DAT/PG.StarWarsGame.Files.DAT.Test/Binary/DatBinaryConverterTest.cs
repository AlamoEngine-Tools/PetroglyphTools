using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Test.Services;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary;

public class DatBinaryConverterTest : CommonDatTestBase
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly DatBinaryConverter _binaryConverter;
    private readonly ICrc32HashingService _crc32HashingService;

    public DatBinaryConverterTest()
    {
        _binaryConverter = new DatBinaryConverter(ServiceProvider);
        _crc32HashingService = ServiceProvider.GetRequiredService<ICrc32HashingService>();
    }

    [Fact]
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
                new("KEY0", "KEY0"),
                new("KEY1", "KEY1"),
                new("KEY2", "KEY2"),
                new("KEY3", "KEY3")
            }));

        var model = _binaryConverter.BinaryToModel(binaryModel);
        Assert.NotNull(model);

        Assert.Equal(binaryModel.RecordNumber, model.Count);
        for (var i = 0; i < binaryModel.RecordNumber; i++)
        {
            Assert.Equal(binaryModel.KeyTable[i].Key, model[i].Key);
            Assert.Equal(binaryModel.ValueTable[i].Value, model[i].Value);
        }
    }
}