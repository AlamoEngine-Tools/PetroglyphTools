// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Status information whether a file or data entry was added to an <see cref="IMegBuilder"/>.
/// </summary>
public readonly struct MegDataEntryAddResult
{
    /// <summary>
    /// Gets whether the file or data entry was added or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(AddedBuilderInfo))]
    public bool Added => Status == MegDataEntryAddStatus.Added && AddedBuilderInfo is not null;

    /// <summary>
    /// Gets the status of the add operation.
    /// </summary>
    public MegDataEntryAddStatus Status { get; }

    /// <summary>
    /// Indicates whether a previous data entry was overwritten.
    /// </summary>
    [MemberNotNullWhen(true, nameof(OverwrittenBuilderInfo))]
    public bool WasOverwrite => OverwrittenBuilderInfo is not null;

    /// <summary>
    /// The data entry info which was added or <see langword="null"/> if no entry was added.
    /// </summary>
    public MegDataEntryBuilderInfo? AddedBuilderInfo { get; }

    /// <summary>
    /// The data entry info which was overwritten or <see langword="null"/> if no data entry was overwritten.
    /// </summary>
    public MegDataEntryBuilderInfo? OverwrittenBuilderInfo { get; }

    /// <summary>
    /// A user readable message why the entry was not added. <see langword="null"/> if the entry was added successfully or no message was provided.
    /// </summary>
    public string? Message { get; }

    private MegDataEntryAddResult(
        MegDataEntryAddStatus status,
        MegDataEntryBuilderInfo? addedInfo,
        MegDataEntryBuilderInfo? overwrittenInfo,
        string? message)
    {
        Status = status;
        AddedBuilderInfo = addedInfo;
        OverwrittenBuilderInfo = overwrittenInfo;
        Message = message;
    }

    internal static MegDataEntryAddResult EntryAdded(MegDataEntryBuilderInfo added, MegDataEntryBuilderInfo? overwrite)
    {
        if (added == null)
            throw new ArgumentNullException(nameof(added));
        return new MegDataEntryAddResult(MegDataEntryAddStatus.Added, added, overwrite, null);
    }

    internal static MegDataEntryAddResult EntryNotAdded(MegDataEntryAddStatus status, string? message)
    {
        if (status == MegDataEntryAddStatus.Added)
            throw new ArgumentException(nameof(status));
        return new MegDataEntryAddResult(status, null, null, message);
    }

    internal static MegDataEntryAddResult FromFileNotFound(string filePath)
    {
        return EntryNotAdded(MegDataEntryAddStatus.FileOrEntryNotFound, $"Source file '{filePath}' does not exist.");
    }

    internal static MegDataEntryAddResult FromEntryNotFound(MegDataEntryLocationReference entryReference)
    {
        return EntryNotAdded(MegDataEntryAddStatus.FileOrEntryNotFound, $"Source entry '{entryReference}' does not exist.");
    }

    internal static MegDataEntryAddResult FromDuplicate(string filePath)
    {
        return EntryNotAdded(MegDataEntryAddStatus.DuplicateEntry, $"A data entry of the path '{filePath}' already exists.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder(Status.ToString());
        if (!string.IsNullOrEmpty(Message)) 
            sb.Append($": {Message}");
        return sb.ToString();
    }
}