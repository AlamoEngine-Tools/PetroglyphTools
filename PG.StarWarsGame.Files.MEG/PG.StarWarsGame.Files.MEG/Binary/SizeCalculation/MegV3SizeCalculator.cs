using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.SizeCalculation;

internal sealed class MegV3SizeCalculator : MegSizeCalculator
{
    private bool _isEncrypted;

    protected override uint HeaderSize => 24u;

    internal override ulong GetEntrySize(MegFileDataEntryBuilderInfo entry)
    {
        return GetBinaryEntrySizeWithEncryption(entry);
    }

    protected override uint GetFileTableRecordSize(MegFileDataEntryBuilderInfo entry)
    {
        // Encrypted entries: 2 bytes flags + 32 bytes padded data = 34 bytes
        // Unencrypted entries: 2 bytes flags + 18 bytes data = 20 bytes
        return entry.Encrypted ? 34u : 20u;
    }

    protected override ulong GetFilenameTableSize(uint rawSize)
    {
        // The ENTIRE filename table is encrypted as a single blob if ANY entry is encrypted
        return _isEncrypted
            ? RoundUpToAesBlockSize(rawSize) 
            : rawSize;
    }

    protected override void OnEntryAdded(MegFileDataEntryBuilderInfo entry)
    {
        if (entry.Encrypted && !_isEncrypted)
        {
            _isEncrypted = true;
            // IMPORTANT: Recalculate cached size because filename table padding just changed!
            RecalculateCachedSize();
        }
    }

    protected override void OnReset()
    {
        _isEncrypted = false;
    }
}