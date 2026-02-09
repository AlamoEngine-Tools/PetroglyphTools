// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using System;
using System.IO;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities;
using PG.StarWarsGame.Files.MEG.Binary.Size;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Represents a container with information for of an MEG entry used for building .MEG files.
/// </summary>
public sealed class MegDataEntryBuilderInfo
{
    /// <summary>
    /// The actual location of a MEG data entry.
    /// </summary>
    public MegDataEntryOriginInfo OriginInfo { get; }

    /// <summary>
    /// Gets the path of the data entry within the constructed MEG file.
    /// </summary>
    public string EntryPath { get; }

    /// <summary>
    /// Gets whether the data entry shall be encrypted or not within the constructed MEG file.
    /// </summary>
    public bool Encrypted { get; }

    /// <summary>
    /// Gets the size of the data entry.
    /// </summary>
    public uint Size { get; private set; }
    
    internal MegDataEntryBuilderInfo(MegDataEntryOriginInfo originInfo, string? overrideEntryPath = null, bool? overrideEncrypted = null)
    {
        if (overrideEntryPath?.Length is 0)
            throw new ArgumentException("The value cannot be an empty string.", nameof(overrideEntryPath));
        OriginInfo = originInfo ?? throw new ArgumentNullException(nameof(originInfo));
        EntryPath = GetEntryPath(originInfo, overrideEntryPath);
        Encrypted = GetEncryption(originInfo, overrideEncrypted);
        RefreshSize();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegDataEntryBuilderInfo"/> class from a data entry and a MEG file.
    /// </summary>
    /// <param name="megFile">The meg file.</param>
    /// <param name="dataEntry">The data entry.</param>
    /// <param name="overrideEntryPath">
    /// Overrides path of the entry within the MEG archive.
    /// When not <see langword="null"/>, the specified path will be used; otherwise the path of <paramref name="dataEntry"/> will be used.
    /// </param>
    /// <param name="overrideEncrypted">
    /// Overrides the encryption state of the entry within the MEG archive.
    /// When not <see langword="null"/>, the specified encryption information will be used; otherwise the current encryption state path will be used.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="overrideEntryPath"/> is empty
    /// -or-
    /// <paramref name="dataEntry"/> does not exist in <paramref name="megFile"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="megFile"/> or <see paramref="dataEntry"/> is <see langword="null"/>.</exception>
    public static MegDataEntryBuilderInfo FromEntry(IMegFile megFile, MegDataEntry dataEntry, string? overrideEntryPath = null, bool? overrideEncrypted = null)
    {
        if (megFile == null) 
            throw new ArgumentNullException(nameof(megFile));
        if (dataEntry == null) 
            throw new ArgumentNullException(nameof(dataEntry));
        var locationReference = new MegDataEntryLocationReference(megFile, dataEntry);
        if (!locationReference.Exists)
            throw new ArgumentException("The specified data entry does not exist in the specified MEG file.", nameof(dataEntry));
        return new MegDataEntryBuilderInfo(
            new MegDataEntryOriginInfo(locationReference), overrideEntryPath, overrideEncrypted);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegDataEntryBuilderInfo"/> class from a data entry reference.
    /// </summary>
    /// <param name="dataEntryReference">The data entry reference.</param>
    /// <param name="overrideEntryPath">
    /// Overrides path of the entry within the MEG archive.
    /// When not <see langword="null"/>, the specified path will be used; otherwise the path of <paramref name="dataEntryReference"/> will be used.
    /// </param>
    /// <param name="overrideEncrypted">
    /// Overrides the encryption state of the entry within the MEG archive.
    /// When not <see langword="null"/>, the specified encryption information will be used; otherwise the current encryption state path will be used.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="overrideEntryPath"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dataEntryReference"/> is <see langword="null"/>.</exception>
    public static MegDataEntryBuilderInfo FromEntryReference(MegDataEntryLocationReference dataEntryReference, string? overrideEntryPath = null, bool? overrideEncrypted = null)
    {
        if (dataEntryReference == null) 
            throw new ArgumentNullException(nameof(dataEntryReference));
        if (!dataEntryReference.Exists)
            throw new ArgumentException("dataEntryReference does not point to an existing entry.", nameof(dataEntryReference));
        return new MegDataEntryBuilderInfo(new MegDataEntryOriginInfo(dataEntryReference), overrideEntryPath, overrideEncrypted);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MegDataEntryBuilderInfo"/> class from a local file.
    /// </summary>
    /// <param name="file">The file to use.</param>
    /// <param name="entryPath">
    /// The path of the entry within the MEG archive.
    /// When not <see langword="null"/>, the specified path will be used; otherwise the full path of <paramref name="file"/> path will be used.
    /// </param>
    /// <param name="encrypt">Sets whether the data shall be encrypted or not. Default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> or <paramref name="entryPath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="file"/> does not exist.
    /// -or-
    /// <paramref name="entryPath"/> is empty.
    /// </exception>
    public static MegDataEntryBuilderInfo FromFile(IFileInfo file, string entryPath, bool encrypt = false)
    {
        if (file == null) 
            throw new ArgumentNullException(nameof(file));
        if (!file.Exists)
            throw new ArgumentException($"The specified file '{file.FullName}' does not exist.", nameof(file));
        ThrowHelper.ThrowIfNullOrEmpty(entryPath);
        return new MegDataEntryBuilderInfo(
            new MegDataEntryOriginInfo(file), entryPath, encrypt);
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
            if (fileInfo.Length > MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxEntrySize)
                MegThrowHelper.ThrowDataEntryExceeds4GigabyteException(fileInfo.FullName);
            var size = (uint)fileInfo.Length;
            Size = size;
        }
    }

    private static string GetEntryPath(MegDataEntryOriginInfo originInfo, string? overrideEntryPath)
    {
        if (overrideEntryPath is not null)
            return overrideEntryPath;
        return originInfo.IsLocalFile 
            ? originInfo.FileInfo.FullName 
            : originInfo.MegFileLocation!.DataEntry.Path;
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