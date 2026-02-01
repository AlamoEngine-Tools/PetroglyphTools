// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using System;
using System.IO;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.MEG.Binary;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Container with data entry information for building .MEG files.
/// </summary>
public sealed class MegFileDataEntryBuilderInfo
{
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
    /// Gets the size of the data entry.
    /// </summary>
    public uint Size { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class with a data entry origin info and optional override parameters.
    /// </summary>
    /// <param name="originInfo">The origin info of the data entry.</param>
    /// <param name="overrideFilePath">When not <see langword="null"/>, the specified file path will be used; otherwise the current file path will be used.</param>
    /// <param name="overrideEncrypted">When not <see langword="null"/>, the specified encryption information will be used; otherwise the current encryption state path will be used.</param>
    /// <exception cref="ArgumentException"><paramref name="overrideFilePath"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="originInfo"/> is <see langword="null"/>.</exception>
    public MegFileDataEntryBuilderInfo(MegDataEntryOriginInfo originInfo, string? overrideFilePath = null, bool? overrideEncrypted = null)
    {
        if (overrideFilePath is not null)
            ThrowHelper.ThrowIfNullOrEmpty(overrideFilePath);
        OriginInfo = originInfo ?? throw new ArgumentNullException(nameof(originInfo));
        FilePath = GetFilePath(originInfo, overrideFilePath);
        Encrypted = GetEncryption(originInfo, overrideEncrypted);
        RefreshSize();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from a data entry and a MEG file.
    /// </summary>
    /// <param name="megFile">The meg file.</param>
    /// <param name="dataEntry">The data entry.</param>
    /// <param name="overrideFilePath">When not <see langword="null"/>, the specified file path will be used; otherwise the current file path will be used.</param>
    /// <param name="overrideEncrypted">When not <see langword="null"/>, the specified encryption information will be used; otherwise the current encryption state path will be used.</param>
    /// <exception cref="ArgumentException"><paramref name="overrideFilePath"/> is empty or contains only whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="megFile"/> or <see paramref="dataEntry"/> is <see langword="null"/>.</exception>
    public static MegFileDataEntryBuilderInfo FromEntry(IMegFile megFile, MegDataEntry dataEntry, string? overrideFilePath = null, bool? overrideEncrypted = null)
    {
        if (megFile == null) 
            throw new ArgumentNullException(nameof(megFile));
        if (dataEntry == null) 
            throw new ArgumentNullException(nameof(dataEntry));
        return new MegFileDataEntryBuilderInfo(
            new MegDataEntryOriginInfo(new MegDataEntryLocationReference(megFile, dataEntry)), overrideFilePath, overrideEncrypted);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from a data entry reference.
    /// </summary>
    /// <param name="dataEntryReference">The data entry reference.</param>
    /// <param name="overrideFilePath">When not <see langword="null"/>, the specified file path will be used; otherwise the current file path will be used.</param>
    /// <param name="overrideEncrypted">When not <see langword="null"/>, the specified encryption information will be used; otherwise the current encryption state path will be used.</param>
    /// <exception cref="ArgumentException"><paramref name="overrideFilePath"/> is empty or contains only whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dataEntryReference"/> is <see langword="null"/>.</exception>
    public static MegFileDataEntryBuilderInfo FromEntryReference(MegDataEntryLocationReference dataEntryReference, string? overrideFilePath = null, bool? overrideEncrypted = null)
    {
        return dataEntryReference == null
            ? throw new ArgumentNullException(nameof(dataEntryReference)) 
            : new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(dataEntryReference), overrideFilePath, overrideEncrypted);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from a local file.
    /// </summary>
    /// <param name="file">The file to use.</param>
    /// <param name="filePathInMeg">When not <see langword="null"/>, the specified file path will be used; otherwise the current file path will be used.</param>
    /// <param name="encrypt">Sets whether the data shall be encrypted or not. Default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/>.</exception>
    public static MegFileDataEntryBuilderInfo FromFile(IFileInfo file, string? filePathInMeg, bool encrypt = false)
    {
        if (file == null) 
            throw new ArgumentNullException(nameof(file));
        return new MegFileDataEntryBuilderInfo(
            new MegDataEntryOriginInfo(file), filePathInMeg, encrypt);
    }

    /// <summary>
    /// Updates the size of the data entry associated with this instance.
    /// </summary>
    /// <exception cref="FileNotFoundException">The file associated with the data entry does not exist.</exception>
    /// <exception cref="MegEntrySizeException">The size of the file exceeds the maximum allowable size of 4 GB.</exception>
    public void RefreshSize()
    {
        if (OriginInfo.IsEntryReference)
            Size = OriginInfo.MegFileLocation.DataEntry.Location.Size;
        else
        {
            var fileInfo = OriginInfo.FileInfo!;
            fileInfo.Refresh();
            if (!fileInfo.Exists)
                throw new FileNotFoundException($"The file '{fileInfo.FullName}' does not exist");
            if (fileInfo.Length > MegFileConstants.MegMaxEntrySize)
                MegThrowHelper.ThrowDataEntryExceeds4GigabyteException(fileInfo.FullName);
            var size = (uint)fileInfo.Length;
            Size = size;
        }
    }

    private static string GetFilePath(MegDataEntryOriginInfo originInfo, string? overrideFileName)
    {
        if (overrideFileName is not null)
            return overrideFileName;
        return originInfo.IsLocalFile 
            ? originInfo.FileInfo.FullName 
            : originInfo.MegFileLocation!.DataEntry.FilePath;
    }

    private static bool GetEncryption(MegDataEntryOriginInfo originInfo, bool? overrideEncrypted)
    {
        if (overrideEncrypted is not null)
            return overrideEncrypted.Value; 
        // Fallback for the case, origin is a file system path but overrideEncrypted was forgotten to set explicitly.
        if (originInfo.IsLocalFile)
            return false;
        return originInfo.MegFileLocation!.DataEntry.Encrypted;
    }
}