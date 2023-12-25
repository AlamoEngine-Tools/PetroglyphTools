// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// Service to create MEG files from local files or other MEG data entries ensuring custom validation and normalization rules.
/// </summary>
public interface IMegBuilder
{
    /// <summary>
    /// Gets a value indicating whether the <see cref="IMegBuilder"/> normalizes a data entry's path before adding it.
    /// </summary>
    bool NormalizesEntryPaths { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="IMegBuilder"/> overwrites an already existing data entry with the new version when trying to a data entry
    /// or does not add the new data entry.
    /// </summary>
    /// <remarks>
    /// A duplicate is determined by the data entry's file path.
    /// </remarks>
    bool OverwritesDuplicateEntries { get; }

    /// <summary>
    /// Gets a collection of all data entries which shall be packed to a .MEG file.
    /// </summary>
    public IReadOnlyCollection<MegFileDataEntryBuilderInfo> DataEntries { get; }

    /// <summary>
    /// Adds a local file as a data entry to the <see cref="IMegBuilder"/> and returns a status information to indicate whether the entry was successfully added. 
    /// </summary>
    /// <remarks>
    /// The actual data entry's file path might be different to <paramref name="filePathInMeg"/> depending whether this <see cref="IMegBuilder"/> normalizes path before adding, or not.
    /// </remarks>
    /// <param name="filePath">The local path to the file which get added as an data entry.</param>
    /// <param name="filePathInMeg">The desired file path of the data entry inside the MEG archive.</param>
    /// <param name="encrypt">Indicates whether the data entry shall be encrypted.</param>
    /// <returns>The result of this operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> or <paramref name="filePathInMeg"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="filePathInMeg"/> is empty</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid path.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePathInMeg"/> consists only of whitespace characters.</exception>
    AddDataEntryToBuilderResult AddFile(string filePath, string filePathInMeg, bool encrypt = false);

    /// <summary>
    /// Adds an existing data entry to the <see cref="IMegBuilder"/> and returns a status information to indicate whether the entry was successfully added. 
    /// </summary>
    /// <remarks>
    /// The actual data entry's file path might be different to <paramref name="overridePathInMeg"/> depending whether this <see cref="IMegBuilder"/> normalizes path before adding, or not.
    /// </remarks>
    /// <param name="entryReference">The existing data entry to add.</param>
    /// <param name="overridePathInMeg">When not <see langword="null"/>, the specified file path is used; otherwise the data entry's file path is used.</param>
    /// <param name="overrideEncrypt">When not <see langword="null"/>, the specified encryption information is used; otherwise the encryption state of the data entry is used.</param>
    /// <returns>The result of this operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="entryReference"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="overridePathInMeg"/> is empty or consists only of whitespace characters.</exception>
    AddDataEntryToBuilderResult AddEntry(MegDataEntryLocationReference entryReference, string? overridePathInMeg = null,
        bool? overrideEncrypt = null);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="IMegBuilder"/>.
    /// </summary>
    /// <param name="info">The builder information to remove from the <see cref="IMegBuilder"/>.</param>
    /// <returns>
    /// <see langword="true"/> if item was successfully removed from the <see cref="IMegBuilder"/>; otherwise, <see langword="false"/>.
    /// This method also returns false if item is not found in the original <see cref="IMegBuilder"/>.
    /// </returns>
    bool Remove(MegFileDataEntryBuilderInfo info);

    /// <summary>
    /// Build a .MEG file from all <see cref="DataEntries"/>.
    /// </summary>
    /// <param name="fileParams">The file parameters of the to be created MEG file.</param>
    /// <param name="overwrite">When set to <see langword="true"/> an existing MEG file will be overwritten; otherwise an <see cref="IOException"/> is thrown if the file already exists.</param>
    /// <exception cref="ArgumentNullException"><paramref name="fileParams"/> is <see langword="null"/>.</exception>
    void Build(MegFileInformation fileParams, bool overwrite);

    /// <summary>
    /// Checks whether a <see cref="MegFileInformation"/> is valid for this instance.
    /// </summary>
    /// <param name="fileParams">The file parameters to check.</param>
    /// <returns><see langword="true"/> if the file parameters are valid; otherwise, <see langword="false"/>.</returns>
    bool AreFileParamsValid(MegFileInformation fileParams);
}

/// <summary>
/// Status information whether a file or data entry was added to an <see cref="IMegBuilder"/>.
/// </summary>
public readonly struct AddDataEntryToBuilderResult
{
    /// <summary>
    /// Gets whether the file or data entry was added or not.
    /// </summary>
    public bool Added => Status == AddDataEntryToBuilderState.Added && AddedBuilderInfo is not null;

    /// <summary>
    /// Gets the status of the add operation.
    /// </summary>
    public AddDataEntryToBuilderState Status { get; }

    /// <summary>
    /// Indicates whether a previous data entry was overwritten.
    /// </summary>
    public bool WasOverwrite => OverwrittenBuilderInfo is not null;

    /// <summary>
    /// The data entry info which was added or <see langword="null"/> if no entry was added.
    /// </summary>
    public MegFileDataEntryBuilderInfo? AddedBuilderInfo { get; }

    /// <summary>
    /// The data entry info which was overwritten or <see langword="null"/> if no data entry was overwritten.
    /// </summary>
    public MegFileDataEntryBuilderInfo? OverwrittenBuilderInfo { get; }

    /// <summary>
    /// The data entry's file path which was added or <see langword="null"/> if no entry was added.
    /// </summary>
    public string? PathInMeg => AddedBuilderInfo?.FilePath;

    /// <summary>
    /// A user readable message why the entry was not added. <see langword="null"/> if the entry was added successfully or no message was provided.
    /// </summary>
    public string? Message { get; }

    private AddDataEntryToBuilderResult(
        AddDataEntryToBuilderState status, 
        MegFileDataEntryBuilderInfo? addedInfo, 
        MegFileDataEntryBuilderInfo? overwrittenInfo, 
        string? message)
    {
        Status = status;
        AddedBuilderInfo = addedInfo;
        OverwrittenBuilderInfo = overwrittenInfo;
        Message = message;
    }

    internal static AddDataEntryToBuilderResult EntryAdded(MegFileDataEntryBuilderInfo added, MegFileDataEntryBuilderInfo? overwrite)
    {
        if (added == null) 
            throw new ArgumentNullException(nameof(added));
        return new AddDataEntryToBuilderResult(AddDataEntryToBuilderState.Added, added, overwrite, null);
    }

    internal static AddDataEntryToBuilderResult EntryNotAdded(AddDataEntryToBuilderState status, string? message)
    {
        if (status == AddDataEntryToBuilderState.Added)
            throw new ArgumentException(nameof(status));
        return new AddDataEntryToBuilderResult(status, null, null, message);
    }
}

/// <summary>
/// Status of adding a file or data entry to an <see cref="IMegBuilder"/>. 
/// </summary>
public enum AddDataEntryToBuilderState
{
    /// <summary>
    /// The file or data entry was successfully added.
    /// </summary>
    Added,
    /// <summary>
    /// The file or data entry was not added because an entry of the same file path already exists.
    /// </summary>
    DuplicateEntry,
    /// <summary>
    /// The file or data entry was not added because it contained invalid or unsupported data.
    /// </summary>
    InvalidEntry,
    /// <summary>
    /// The file or data entry was not added because the file or data entry does not exist.
    /// </summary>
    FileOrEntryNotFound,
    /// <summary>
    /// The file or data entry was not added because the normalization of the file path failed.
    /// </summary>
    FailedNormalization
}