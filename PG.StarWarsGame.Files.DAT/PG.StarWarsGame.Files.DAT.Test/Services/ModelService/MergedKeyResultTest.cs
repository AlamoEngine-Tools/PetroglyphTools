using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class MergedKeyResultTest
{
    [Fact]
    public void Test_Ctor()
    {

        var result = new MergedKeyResult(default);
        Assert.Equal(MergeOperation.Added, result.Status);
        Assert.Null(result.OldEntry);

        result = new MergedKeyResult(default, default(DatStringEntry));
        Assert.Equal(MergeOperation.Overwritten, result.Status);
        Assert.NotNull(result.OldEntry);
    }

    [Fact]
    public void Test_Getters()
    {
        var newEntry = new DatStringEntry("1", new Crc32(1), "a");
        var oldEntry = new DatStringEntry("2", new Crc32(2), "b");

        var result = new MergedKeyResult(newEntry, oldEntry);

        Assert.Equal(newEntry, result.NewEntry);
        Assert.Equal(oldEntry, result.OldEntry);
    }
}