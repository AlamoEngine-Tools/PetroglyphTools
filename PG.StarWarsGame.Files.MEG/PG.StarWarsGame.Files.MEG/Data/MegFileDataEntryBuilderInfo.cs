// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Container with data entry information for building .MEG files.
/// </summary>
public sealed class MegFileDataEntryBuilderInfo
{
    private Crc32? _crc32;
    
    /// <summary>
    /// The actual location of a MEG data entry file.
    /// </summary>
    public MegDataEntryOriginInfo OriginInfo { get; }

    /// <summary>
    /// Gets the file path to be used for the data entry when constructing a MEG file.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets whether the data entry file shall be encrypted or not when constructing a MEG file.
    /// </summary>
    public bool Encrypted { get; }

    /// <summary>
    /// The CRC32 checksum of <see cref="FilePath"/> calculated using the MEG's file path encoding <see cref="MegFileConstants.MegContentFileNameEncoding"/>. 
    /// </summary>
    public Crc32 Crc32
    {
        get
        {
            if (_crc32.HasValue)
                return _crc32.Value;
            _crc32 = CreateCrc32();
            return _crc32.Value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class with a data entry origin info and optional override parameters.
    /// </summary>
    /// <param name="originInfo">The origin info of the data entry.</param>
    /// <param name="overrideFileName">When not <see langword="null"/>, the specified file path will be used.</param>
    /// <param name="overrideEncrypted">When not <see langword="null"/>, the specified encryption information will be used.</param>
    /// <exception cref="ArgumentException"><paramref name="originInfo"/> has invalid file path data.</exception>
    /// <exception cref="ArgumentException"><paramref name="overrideFileName"/> is empty or contains only whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="originInfo"/> is <see langword="null"/>.</exception>
    public MegFileDataEntryBuilderInfo(MegDataEntryOriginInfo originInfo, string? overrideFileName = null, bool? overrideEncrypted = null)
    {
        if (overrideEncrypted is not null && string.IsNullOrWhiteSpace(overrideFileName))
            throw new ArgumentException("Overriding file path must not be empty or whitespace only.", nameof(overrideEncrypted));
        
        OriginInfo = originInfo ?? throw new ArgumentNullException(nameof(originInfo));

        var fileName = GetFilePath(originInfo, overrideFileName);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name must not be null, empty or whitespace only.", nameof(originInfo));
        FilePath = fileName;

        Encrypted = GetEncryption(originInfo, overrideEncrypted);
    }

    private static bool GetEncryption(MegDataEntryOriginInfo originInfo, bool? overrideEncrypted)
    {
        if (overrideEncrypted is not null)
            return overrideEncrypted.Value;

        // Fallback for the case, origin is a file system path but overrideEncrypted was forgotten to set explicitly.
        if (originInfo.FilePath is not null)
            return false;

        return originInfo.MegFileLocation!.DataEntry.Encrypted;
    }

    private static string GetFilePath(MegDataEntryOriginInfo originInfo, string? overrideFileName)
    {
        if (overrideFileName is not null)
            return overrideFileName;
        if (originInfo.FilePath is not null)
            return originInfo.FilePath;
        return originInfo.MegFileLocation!.DataEntry.FilePath;
    }


    private Crc32 CreateCrc32()
    {
        return ChecksumService.Instance.GetChecksum(FilePath, MegFileConstants.MegContentFileNameEncoding);
    }
}