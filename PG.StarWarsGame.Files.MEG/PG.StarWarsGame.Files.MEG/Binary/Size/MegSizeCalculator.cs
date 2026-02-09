// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

/// <summary>
/// Provides an abstract base class for calculating the metadata size of MEG files.
/// </summary>
/// <remarks>
/// This class defines the structure and methods required to calculate the size of metadata
/// for MEG files, including headers, filename tables, file tables, and file data.
/// Specific MEG file formats should inherit from this class and implement the required logic.
/// </remarks>
internal abstract class MegSizeCalculator : IMegSizeCalculator
{
    private uint _currentRawFilenameTableSize;
    private ulong _currentFileTableSize;
    private ulong _currentFileDataSize;

    protected abstract uint HeaderSize { get; }

    public ulong CurrentSize { get; private set; }
    
    public ulong MetadataSize { get; private set;}

    protected virtual bool IsFilenameTableEncrypted => false;

    protected MegSizeCalculator()
    {
        // ReSharper disable VirtualMemberCallInConstructor
        CurrentSize = HeaderSize;
        MetadataSize = HeaderSize;
    }

    /// <summary>
    /// Calculates the binary size of a MEG file dataEntry, considering encryption.
    /// </summary>
    /// <param name="dataEntry">
    /// The <see cref="MegDataEntryBuilderInfo"/> containing information about the MEG file dataEntry.
    /// </param>
    /// <returns>
    /// The size of the binary dataEntry. If the data entry is marked for encryption, the size is rounded up to the AES block size.
    /// </returns>
    /// <remarks>
    /// This method determines the size of a MEG data entry by checking if encryption is enabled.
    /// If encryption is applied, the size is adjusted to align with the AES block size.
    /// </remarks>
    public static ulong GetBinaryEntrySizeWithEncryption(MegDataEntryBuilderInfo dataEntry)
    {
        return !dataEntry.Encrypted ? dataEntry.Size : RoundUpToAesBlockSize(dataEntry.Size);
    }
    
    public ulong PreCalculateSize(IEnumerable<MegDataEntryBuilderInfo> entries)
    {
        var rawFilenameTableSize = _currentRawFilenameTableSize;
        var fileTableSize = _currentFileTableSize;
        var fileDataSize = _currentFileDataSize;
        var shouldEncryptFilenameTable = IsFilenameTableEncrypted;

        foreach (var entry in entries)
        {
            UpdateSizesForEntry(
                ref rawFilenameTableSize,
                ref fileTableSize,
                ref fileDataSize,
                ref shouldEncryptFilenameTable,
                entry);
        }

        return CalculateTotalSize(rawFilenameTableSize, fileTableSize, fileDataSize, shouldEncryptFilenameTable);
    }
    
    public ulong PreCalculateSize(MegDataEntryBuilderInfo dataEntry)
    {
        var rawFilenameTableSize = _currentRawFilenameTableSize;
        var fileTableSize = _currentFileTableSize;
        var fileDataSize = _currentFileDataSize;
        var shouldEncryptFilenameTable = IsFilenameTableEncrypted;

        UpdateSizesForEntry(
            ref rawFilenameTableSize,
            ref fileTableSize,
            ref fileDataSize,
            ref shouldEncryptFilenameTable,
            dataEntry);

        return CalculateTotalSize(rawFilenameTableSize, fileTableSize, fileDataSize, shouldEncryptFilenameTable);
    }

    public void AddEntry(MegDataEntryBuilderInfo dataEntry)
    {
        var oldFilenameTableSize = GetFilenameTableSize(_currentRawFilenameTableSize, IsFilenameTableEncrypted);
        var fileTableRecord = GetFileTableRecordSize(dataEntry);
        var fileSize = GetEntrySize(dataEntry);
        var shouldEncryptFilenameTable = IsFilenameTableEncrypted;

        UpdateSizesForEntry(
            ref _currentRawFilenameTableSize,
            ref _currentFileTableSize,
            ref _currentFileDataSize,
            ref shouldEncryptFilenameTable,
            dataEntry);

        OnEntryAdded(dataEntry);

        var newFilenameTableSize = GetFilenameTableSize(_currentRawFilenameTableSize, IsFilenameTableEncrypted);

        Debug.Assert(newFilenameTableSize >= oldFilenameTableSize,
            "New filename table size must be greater than or equal to old size.");

        var metadataDelta = newFilenameTableSize - oldFilenameTableSize + fileTableRecord;

        MetadataSize += metadataDelta;
        CurrentSize += metadataDelta + fileSize;
    }

    internal virtual ulong GetEntrySize(MegDataEntryBuilderInfo dataEntry)
    {
        return dataEntry.Size;
    }

    public void Reset()
    {
        _currentRawFilenameTableSize = 0;
        _currentFileTableSize = 0;
        _currentFileDataSize = 0;
        CurrentSize = HeaderSize;
        MetadataSize = HeaderSize;
        OnReset();
    }

    internal static ulong RoundUpToAesBlockSize(uint size)
    {
        // This is correct, because MEG files don't use PKCS#7 padding.
        // Thus, input: 16, result: 16 (not 32)
        return ((ulong)size + MegFileConstants.AesBlockSize - 1) / MegFileConstants.AesBlockSize * MegFileConstants.AesBlockSize;
    }

    protected abstract uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry);
    
    protected virtual ulong GetFilenameTableSize(uint rawSize, bool encrypt)
    {
        return encrypt
            ? RoundUpToAesBlockSize(rawSize)
            : rawSize;
    }
    
    protected virtual bool ShouldEncryptFilenameTable(MegDataEntryBuilderInfo dataEntry)
    {
        return false;
    }

    protected virtual void OnEntryAdded(MegDataEntryBuilderInfo dataEntry)
    {
    }

    protected virtual void OnReset()
    {
    }

    private ulong CalculateTotalSize(uint rawFilenameTableSize, ulong fileTableSize, ulong fileDataSize, bool encryptFilenameTable)
    {
        var filenameTableSize = GetFilenameTableSize(rawFilenameTableSize, encryptFilenameTable);
        return HeaderSize + filenameTableSize + fileTableSize + fileDataSize;
    }

    private void UpdateSizesForEntry(
        ref uint rawFilenameTableSize,
        ref ulong fileTableSize,
        ref ulong fileDataSize,
        ref bool shouldEncryptFilenameTable,
        MegDataEntryBuilderInfo entry)
    {
        rawFilenameTableSize += (uint)MegFileNameTableRecord.GetRecordSize(GetEntryPath(entry));
        fileTableSize += GetFileTableRecordSize(entry);
        fileDataSize += GetEntrySize(entry);
        shouldEncryptFilenameTable |= ShouldEncryptFilenameTable(entry);
    }

    private static string GetEntryPath(MegDataEntryBuilderInfo dataEntry)
    {
        return dataEntry.EntryPath;
    }
}