// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

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
    /// Gets the size of the data entry or <see langword="null"/> if no size was specified.
    /// </summary>
    public uint? Size { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class with a data entry origin info and optional override parameters.
    /// </summary>
    /// <param name="originInfo">The origin info of the data entry.</param>
    /// <param name="overrideFilePath">When not <see langword="null"/>, the specified file path will be used; otherwise the current file path will be used.</param>
    /// <param name="fileSize">Pre-calculated size of the data entry. Parameter gets ignored when <paramref name="originInfo"/> holds an existing data entry.</param>
    /// <param name="overrideEncrypted">When not <see langword="null"/>, the specified encryption information will be used; otherwise the current encryption state path will be used.</param>
    /// <exception cref="ArgumentException"><paramref name="overrideFilePath"/> is empty or contains only whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="originInfo"/> is <see langword="null"/>.</exception>
    public MegFileDataEntryBuilderInfo(MegDataEntryOriginInfo originInfo, string? overrideFilePath = null, uint? fileSize = null, bool? overrideEncrypted = null)
    {
        if (overrideFilePath is not null)
            Commons.Utilities.ThrowHelper.ThrowIfNullOrWhiteSpace(overrideFilePath);

        OriginInfo = originInfo ?? throw new ArgumentNullException(nameof(originInfo));

        var filePath = GetFilePath(originInfo, overrideFilePath);
        FilePath = filePath;

        Size = originInfo.IsLocalFile ? fileSize : originInfo.MegFileLocation!.DataEntry.Location.Size;

        Encrypted = GetEncryption(originInfo, overrideEncrypted);
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
        return new MegFileDataEntryBuilderInfo(
            new MegDataEntryOriginInfo(new MegDataEntryLocationReference(megFile, dataEntry)), overrideFilePath, null, overrideEncrypted);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from a data entry reference.
    /// </summary>
    /// <param name="dataEntryReference">The data entry reference.</param>
    /// <param name="overrideFilePath">When not <see langword="null"/>, the specified file path will be used; otherwise the current file path will be used.</param>
    /// <param name="overrideEncrypted">When not <see langword="null"/>, the specified encryption information will be used; otherwise the current encryption state path will be used.</param>
    /// <exception cref="ArgumentException"><paramref name="overrideFilePath"/> is empty or contains only whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dataEntryReference"/> or <see paramref="dataEntry"/> is <see langword="null"/>.</exception>
    public static MegFileDataEntryBuilderInfo FromEntryReference(MegDataEntryLocationReference dataEntryReference, string? overrideFilePath = null, bool? overrideEncrypted = null)
    {
        return new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(dataEntryReference), overrideFilePath, null, overrideEncrypted);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class from a local file.
    /// </summary>
    /// <param name="filePath">The local file path.</param>
    /// <param name="filePathInMeg">When not <see langword="null"/>, the specified file path will be used; otherwise the current file path will be used.</param>
    /// <param name="size">Optional, pre-calculated size of the data entry.</param>
    /// <param name="encrypt">Sets whether the data shall be encrypted or not. Default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="filePathInMeg"/> is empty or contains only whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <see langword="null"/>.</exception>
    public static MegFileDataEntryBuilderInfo FromFile(string filePath, string? filePathInMeg, uint? size = null, bool encrypt = false)
    {
        return new MegFileDataEntryBuilderInfo(
            new MegDataEntryOriginInfo(filePath), filePathInMeg, size, encrypt);
    }

    private static string GetFilePath(MegDataEntryOriginInfo originInfo, string? overrideFileName)
    {
        if (overrideFileName is not null)
            return overrideFileName;
        if (originInfo.IsLocalFile)
            return originInfo.FilePath;
        return originInfo.MegFileLocation!.DataEntry.FilePath;
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