using System.Collections.Generic;
using System.Drawing;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MTD.Binary;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using PG.StarWarsGame.Files.MTD.Data;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.Converter;

public class MtdBinaryConverterTest
{
    private readonly MtdBinaryConverter _binaryConverter;
    private readonly ICrc32HashingService _hashingService;

    public MtdBinaryConverterTest()
    {
        var fs = new MockFileSystem();
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(fs);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        new PGServiceContribution().ContributeServices(sc);
        sc.AddMtdServices();

        var sp = sc.BuildServiceProvider();
        _binaryConverter = new MtdBinaryConverter(sp);
        _hashingService = sp.GetRequiredService<ICrc32HashingService>();

    }

    [Fact]
    public void BinaryToModel_DuplicateEntriesByName_ThrowsBinaryCorruptedException()
    {
        var header = new MtdHeader(2);
        var entries = new List<MtdBinaryFileInfo>
        {
            new("name", 1, 2, 3, 4, true),
            new("name", 4, 3, 2, 1, false)
        };
        Assert.Throws<BinaryCorruptedException>(() => _binaryConverter.BinaryToModel(new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>(entries))));
    }

    [Fact]
    public void BinaryToModel_ModelsAdded_Empty()
    {
        var header = new MtdHeader(0);
        var model = _binaryConverter.BinaryToModel(new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>([])));
        Assert.Empty(model);
    }

    [Fact]
    public void BinaryToModel_ModelsAdded()
    {
        var header = new MtdHeader(2);
        var entries = new List<MtdBinaryFileInfo>
        {
            new("this.tga", 1, 2, 3, 4, true),
            new("other.tga", 4, 3, 2, 1, false)
        };

        var thisCrc = _hashingService.GetCrc32("this.tga", Encoding.ASCII);
        var otherCrc = _hashingService.GetCrc32("other.tga", Encoding.ASCII);

        var model = _binaryConverter.BinaryToModel(new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>(entries)));
        Assert.Equal(2, model.Count);

        var thisFile = model.First(x => x.FileName == "this.tga");
        var otherFile = model.First(x => x.FileName == "other.tga");

        Assert.Equal(new MegaTextureFileIndex("this.tga", thisCrc, new Rectangle(1, 2, 3, 4), true), thisFile);
        Assert.Equal(new MegaTextureFileIndex("other.tga", otherCrc, new Rectangle(4, 3, 2, 1), false), otherFile);
    }


    [Fact]
    public void ModelToBinary_Empty()
    {
        var binary = _binaryConverter.ModelToBinary(new MegaTextureDirectory([]));

        Assert.Empty(binary.Items);
        Assert.Equal(0u, binary.Header.Count);
    }

    [Fact]
    public void ModelToBinary()
    {
        var entries = new List<MegaTextureFileIndex>
        {
            new("name", new Crc32(123), new Rectangle(1,2,3,4), true),
            new("other", new Crc32(456), new Rectangle(4,3,2,1), false),
        };
        var binary = _binaryConverter.ModelToBinary(new MegaTextureDirectory(entries));

        Assert.Equal(2, binary.Items.Count);
        Assert.Equal(2u, binary.Header.Count);

        for (var i = 0; i < entries.Count; i++)
        {
            var expected = entries[i];
            var actual = binary.Items[i];
            
            Assert.Equal(expected.FileName, actual.Name);
            Assert.Equal((uint)expected.Area.X, actual.X);
            Assert.Equal((uint)expected.Area.Y, actual.Y);
            Assert.Equal((uint)expected.Area.Width, actual.Width);
            Assert.Equal((uint)expected.Area.Height, actual.Height);
            Assert.Equal(expected.HasAlpha, actual.Alpha);
        }

    }

}