using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

public class UnsortedDatModelTest : DatModelTest
{
    protected override DatFileType ExpectedFileType => DatFileType.NotOrdered;

    private UnsortedDatModel CreateUnsortedModel(IList<DatStringEntry> entries)
    {
        return new UnsortedDatModel(entries);
    }

    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
    {
        return CreateUnsortedModel(entries);
    }

    protected override IList<DatStringEntry> CreateDataEntries()
    {
        var entry1 = new DatStringEntry("1", new Crc32(1), "value1");
        var entry2 = new DatStringEntry("1", new Crc32(1), "value2");
        var entry3 = new DatStringEntry("3", new Crc32(3), "value3");
        var entry4 = new DatStringEntry("4", new Crc32(4), "value4");

        var entries = new List<DatStringEntry>
        {
            entry3,
            entry4,
            entry1,
            entry2,
        };

        return entries;
    }

    [Fact]
    public void Test_ToSortedModel()
    {
        var model = CreateUnsortedModel(CreateDataEntries());
        var sorted = model.ToSortedModel();
        
        Assert.True(Crc32Utilities.IsSortedByCrc32(sorted));
        Assert.Equal(DatFileType.OrderedByCrc32, sorted.KeySortOrder);
    }
}