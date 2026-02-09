using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Size;

public class MegSizeCalculatorStaticTest : CommonMegTestBase
{
    [Theory]
    [InlineData(0u, 0ul)]
    [InlineData(1u, 16ul)]
    [InlineData(15u, 16ul)]
    [InlineData(16u, 16ul)]
    [InlineData(17u, 32ul)]
    [InlineData(31u, 32ul)]
    [InlineData(32u, 32ul)]
    public void RoundUpToAesBlockSize_ReturnsExpected(uint size, ulong expected)
    {
        var rounded = MegSizeCalculator.RoundUpToAesBlockSize(size);

        Assert.Equal(expected, rounded);
    }

    [Theory]
    [InlineData(10u, false, 10ul)]
    [InlineData(10u, true, 16ul)]
    [InlineData(16u, true, 16ul)]
    [InlineData(17u, true, 32ul)]
    public void GetBinaryEntrySizeWithEncryption_ReturnsExpected(uint size, bool encrypted, ulong expected)
    {
        var entry = CreateEntry($"entry_{size}_{encrypted}", size, encrypted);

        var result = MegSizeCalculator.GetBinaryEntrySizeWithEncryption(entry);

        Assert.Equal(expected, result);
    }

    private MegDataEntryBuilderInfo CreateEntry(string path, uint size, bool encrypted)
    {
        var file = FileSystem.FileInfo.New(path);
        FileSystem.File.WriteAllBytes(file.FullName, new byte[size]);

        return MegDataEntryBuilderInfo.FromFile(file, path, encrypted);
    }
}