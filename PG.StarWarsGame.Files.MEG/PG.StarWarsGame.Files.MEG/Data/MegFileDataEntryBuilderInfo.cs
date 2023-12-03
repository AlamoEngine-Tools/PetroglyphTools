// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

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

    internal uint? Size { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryBuilderInfo"/> class with a data entry origin info and optional override parameters.
    /// </summary>
    /// <param name="originInfo">The origin info of the data entry.</param>
    /// <param name="overrideFilePath">When not <see langword="null"/>, the specified file path will be used.</param>
    /// <param name="overrideEncrypted">When not <see langword="null"/>, the specified encryption information will be used.</param>
    /// <exception cref="ArgumentException"><paramref name="originInfo"/> has invalid file path data.</exception>
    /// <exception cref="ArgumentException"><paramref name="overrideFilePath"/> is empty or contains only whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="originInfo"/> is <see langword="null"/>.</exception>
    public MegFileDataEntryBuilderInfo(MegDataEntryOriginInfo originInfo, string? overrideFilePath = null, bool? overrideEncrypted = null)
    {
        if (overrideEncrypted is not null && string.IsNullOrWhiteSpace(overrideFilePath))
            throw new ArgumentException("Overriding file path must not be empty or whitespace only.", nameof(overrideEncrypted));

        OriginInfo = originInfo ?? throw new ArgumentNullException(nameof(originInfo));

        var filePath = GetFilePath(originInfo, overrideFilePath);
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File name must not be null, empty or whitespace only.", nameof(originInfo));
        FilePath = filePath;

        Encrypted = GetEncryption(originInfo, overrideEncrypted);
    }

    //public MegFileDataEntryBuilderInfo(MegDataEntryLocationReference locationReference, bool overrideEncrypted = false) 
    //    : this(locationReference, null, overrideEncrypted)
    //{
    //}

    //public MegFileDataEntryBuilderInfo(MegDataEntryLocationReference locationReference, string? overrideFilePath, bool? overrideEncrypted = null)
    //{
    //    if (locationReference == null)
    //        throw new ArgumentNullException(nameof(locationReference));

    //    // TODO: Null is allowed!
    //    //if (string.IsNullOrWhiteSpace(overrideFilePath))
    //    //    throw new ArgumentException("file path must not be whitespace only.", nameof(overrideFilePath));

    //    OriginInfo = new MegDataEntryOriginInfo(locationReference);
    //    Encrypted = overrideEncrypted ?? locationReference.DataEntry.Encrypted;
    //    Size = locationReference.DataEntry.Location.Size;
    //    FilePath = overrideFilePath ?? locationReference.DataEntry.FilePath;
    //}

    //public MegFileDataEntryBuilderInfo(string localFilePath, string entryFilePath, uint? size = null, bool encrypted = false)
    //{
    //    if (localFilePath == null)
    //        throw new ArgumentNullException(nameof(localFilePath));
    //    if (entryFilePath == null)
    //        throw new ArgumentNullException(nameof(entryFilePath));
    //    if (string.IsNullOrWhiteSpace(entryFilePath))
    //        throw new ArgumentException("file path must not be whitespace only.", nameof(entryFilePath));


    //    OriginInfo = new MegDataEntryOriginInfo(localFilePath);
    //    Size = size;
    //    FilePath = entryFilePath;
    //    Encrypted = encrypted;
    //}

    private static string GetFilePath(MegDataEntryOriginInfo originInfo, string? overrideFileName)
    {
        if (overrideFileName is not null)
            return overrideFileName;
        if (originInfo.FilePath is not null)
            return originInfo.FilePath;
        return originInfo.MegFileLocation!.DataEntry.FilePath;
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
}