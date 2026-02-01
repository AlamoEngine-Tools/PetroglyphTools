using System.Collections.Generic;
using System.Diagnostics;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.SizeCalculation;

/// <summary>
/// Provides an abstract base class for calculating the metadata size of MEG files.
/// </summary>
/// <remarks>
/// This class defines the structure and methods required to calculate the size of metadata
/// for MEG files, including headers, filename tables, file tables, and file data.
/// Specific MEG file formats should inherit from this class and implement the required logic.
/// </remarks>
internal abstract class MegSizeCalculator : IIMegSizeCalculator
{
    private uint _currentRawFilenameTableSize;
    private ulong _currentFileTableSize;
    private ulong _currentFileDataSize;

    protected abstract uint HeaderSize { get; }

    public ulong CurrentSize { get; private set; }
    public ulong MetadataSize { get; private set;}

    protected MegSizeCalculator()
    {
        // ReSharper disable VirtualMemberCallInConstructor
        CurrentSize = HeaderSize;
        MetadataSize = HeaderSize;
    }

    /// <summary>
    /// Calculates the binary size of a MEG file entry, considering encryption.
    /// </summary>
    /// <param name="entryInfo">
    /// The <see cref="MegFileDataEntryBuilderInfo"/> containing information about the MEG file entry.
    /// </param>
    /// <returns>
    /// The size of the binary entry. If the entry is marked for encryption, the size is rounded up to the AES block size.
    /// </returns>
    /// <remarks>
    /// This method determines the size of a MEG file entry by checking if encryption is enabled.
    /// If encryption is applied, the size is adjusted to align with the AES block size.
    /// </remarks>
    public static ulong GetBinaryEntrySizeWithEncryption(MegFileDataEntryBuilderInfo entryInfo)
    {
        return !entryInfo.Encrypted ? entryInfo.Size : RoundUpToAesBlockSize(entryInfo.Size);
    }
    
    public ulong PreCalculateSize(IEnumerable<MegFileDataEntryBuilderInfo> entries)
    {
        ulong totalSize = HeaderSize;

        foreach (var entry in entries)
        {
            var entryPath = GetEntryPath(entry);
            var filenameRecordSize = (uint)MegFileNameTableRecord.GetRecordSize(entryPath);
            var rawFilenameTableSize = _currentRawFilenameTableSize + filenameRecordSize;

            var actualFilenameTableSize = GetFilenameTableSize(rawFilenameTableSize);
            var fileTableRecordSize = GetFileTableRecordSize(entry);
            var fileTableSize = _currentFileTableSize + fileTableRecordSize;

            var fileDataSize = GetEntrySize(entry);
            var megContentSize = _currentFileDataSize + fileDataSize;

            totalSize += actualFilenameTableSize + fileTableSize + megContentSize;
        }

        return totalSize;
    }
    
    public ulong PreCalculateSize(MegFileDataEntryBuilderInfo entry)
    {
        var entryPath = GetEntryPath(entry);
       
        var filenameRecord = (uint)MegFileNameTableRecord.GetRecordSize(entryPath); 
        var rawFilenameTableSize = _currentRawFilenameTableSize + filenameRecord;

        // Max value here is (ushort.Max + 2) * int.Max < long.Max
        var actualFilenameTableSize = GetFilenameTableSize(rawFilenameTableSize);

        var fileTableRecord = GetFileTableRecordSize(entry);
        
        // Max value here is (34 * ushort.Max || 20 * int.Max) < long.Max
        var fileTableSize = _currentFileTableSize + fileTableRecord;

        var fileSize = GetEntrySize(entry);

        // Max value here uint.Max * int.Max > long.Max
        var megContentSize = _currentFileDataSize + fileSize;

        // Most pessimistic file size (not actually possible):
        // 16 +
        // ((ushort.MaxValue + 2) * (long)int.MaxValue) +
        // ((long)20 * int.MaxValue) +
        // (Int128)uint.MaxValue * int.MaxValue;
        return HeaderSize +
               actualFilenameTableSize +
               fileTableSize +
               megContentSize;
    }

    public void AddEntry(MegFileDataEntryBuilderInfo entry)
    {
        var entryPath = GetEntryPath(entry);
        var filenameRecord = (uint)MegFileNameTableRecord.GetRecordSize(entryPath);
        var fileTableRecord = GetFileTableRecordSize(entry);
        var fileSize = GetEntrySize(entry);

        var oldFilenameTableSize = GetFilenameTableSize(_currentRawFilenameTableSize);

        _currentRawFilenameTableSize += filenameRecord;
        _currentFileTableSize += fileTableRecord;
        _currentFileDataSize += fileSize;

        OnEntryAdded(entry);

        var newFilenameTableSize = GetFilenameTableSize(_currentRawFilenameTableSize);

        Debug.Assert(newFilenameTableSize >= oldFilenameTableSize,
            "New filename table size must be greater than or equal to old size.");

        var metadataDelta = newFilenameTableSize - oldFilenameTableSize + fileTableRecord;

        MetadataSize += metadataDelta;
        CurrentSize += metadataDelta + fileSize;
    }

    internal virtual ulong GetEntrySize(MegFileDataEntryBuilderInfo entry)
    {
        return entry.Size;
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

    protected abstract uint GetFileTableRecordSize(MegFileDataEntryBuilderInfo entry);
    
    protected virtual ulong GetFilenameTableSize(uint rawSize)
    {
        return rawSize;
    }

    protected virtual void OnEntryAdded(MegFileDataEntryBuilderInfo entry)
    {
    }

    protected virtual void OnReset()
    {
    }

    /// <summary>
    /// Recalculates the cached current size from scratch.
    /// Call this when state changes that affect previous entries (e.g., V3 encryption state change).
    /// </summary>
    protected void RecalculateCachedSize()
    {
        var filenameTableSize = GetFilenameTableSize(_currentRawFilenameTableSize);
        MetadataSize = HeaderSize + filenameTableSize + _currentFileTableSize;
        CurrentSize = MetadataSize + _currentFileDataSize;
    }

    private static string GetEntryPath(MegFileDataEntryBuilderInfo entry)
    {
        return entry.FilePath;
    }
}