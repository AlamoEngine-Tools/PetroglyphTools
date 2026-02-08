using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Size;

public class MegV3SizeCalculatorTest : MegSizeCalculatorTestBase
{
    private protected override IMegSizeCalculator CreateCalculator() => new MegV3SizeCalculator();

    protected override uint ExpectedHeaderSize => 24u;

    protected override bool SupportsEncryption => true;

    protected override uint GetExpectedFileTableRecordSize(MegDataEntryBuilderInfo entry)
    {
        return entry.Encrypted ? 34u : 20u;
    }

    [Fact]
    public void PreCalculateSize_EncryptedEntry_UsesPaddedSizes()
    {
        var calculator = CreateCalculator();
        var entry = CreateEntry("abc", 5, true);

        const uint rawFilenameTableSize = 2u + 3u;
        const uint filenameTableSize = 16u;
        const uint fileTableSize = 34u;
        const uint fileDataSize = 16u;

        var expectedSize = ExpectedHeaderSize + filenameTableSize + fileTableSize + fileDataSize;

        var preCalculated = calculator.PreCalculateSize(entry);
        calculator.AddEntry(entry);

        Assert.Equal(expectedSize, preCalculated);
        Assert.Equal(expectedSize, calculator.CurrentSize);
        Assert.Equal((ExpectedHeaderSize + filenameTableSize + fileTableSize), calculator.MetadataSize);
        Assert.Equal(16u, filenameTableSize);
        Assert.Equal(2u + 3u, rawFilenameTableSize);
    }

    [Fact]
    public void AddEntry_Encrypted_UpdatesFilenameTablePadding()
    {
        var calculator = CreateCalculator();
        var entry1 = CreateEntry("a", 10);
        var entry2 = CreateEntry("bb", 5, true);

        calculator.AddEntry(entry1);

        var expectedAfterFirstMetadata = ExpectedHeaderSize + 2u + 1u + 20u;
        var expectedAfterFirstTotal = expectedAfterFirstMetadata + 10u;

        Assert.Equal(expectedAfterFirstMetadata, calculator.MetadataSize);
        Assert.Equal(expectedAfterFirstTotal, calculator.CurrentSize);

        calculator.AddEntry(entry2);

        const uint rawFilenameTableSize = 2u + 1u + 2u + 2u;
        const uint filenameTableSize = 16u;
        const uint fileTableSize = 20u + 34u;
        const uint fileDataSize = 10u + 16u;

        var expectedMetadata = ExpectedHeaderSize + filenameTableSize + fileTableSize;
        var expectedTotal = expectedMetadata + fileDataSize;

        Assert.Equal(expectedMetadata, calculator.MetadataSize);
        Assert.Equal(expectedTotal, calculator.CurrentSize);
        Assert.Equal(7u, rawFilenameTableSize);
    }
}