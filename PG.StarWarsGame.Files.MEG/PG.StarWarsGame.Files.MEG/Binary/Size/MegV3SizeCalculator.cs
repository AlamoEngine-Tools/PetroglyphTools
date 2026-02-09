// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal sealed class MegV3SizeCalculator : MegSizeCalculator
{
    private bool _isEncrypted;

    protected override uint HeaderSize => 24u;

    protected override bool IsFilenameTableEncrypted => _isEncrypted;

    internal override ulong GetEntrySize(MegDataEntryBuilderInfo dataEntry)
    {
        return GetBinaryEntrySizeWithEncryption(dataEntry);
    }

    protected override uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry)
    {
        // Encrypted entries: 2 bytes flags + 32 bytes padded data = 34 bytes
        // Unencrypted entries: 2 bytes flags + 18 bytes data = 20 bytes
        return dataEntry.Encrypted ? 34u : 20u;
    }

    protected override bool ShouldEncryptFilenameTable(MegDataEntryBuilderInfo dataEntry)
    {
        return dataEntry.Encrypted;
    }

    protected override void OnEntryAdded(MegDataEntryBuilderInfo dataEntry)
    {
        if (dataEntry.Encrypted && !_isEncrypted)
        {
            _isEncrypted = true;
        }
    }

    protected override void OnReset()
    {
        _isEncrypted = false;
    }
}