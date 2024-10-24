﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Status information whether an entry was added to an <see cref="IDatBuilder"/>.
/// </summary>
public readonly struct AddEntryResult
{
    /// <summary>
    /// Gets whether the entry was added or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(AddedEntry))]
    public bool Added => Status is AddEntryState.Added or AddEntryState.AddedDuplicate && AddedEntry is not null;

    /// <summary>
    /// Gets the status of the add operation.
    /// </summary>
    public AddEntryState Status { get; }

    /// <summary>
    /// Indicates whether a previous entry was overwritten.
    /// </summary>
    [MemberNotNullWhen(true, nameof(OverwrittenEntry))]
    public bool WasOverwrite => OverwrittenEntry is not null;

    /// <summary>
    /// The entry which was added or <see langword="null"/> if no entry was added.
    /// </summary>
    public DatStringEntry? AddedEntry { get; }

    /// <summary>
    /// The entry which was overwritten or <see langword="null"/> if no entry was overwritten.
    /// </summary>
    public DatStringEntry? OverwrittenEntry { get; }

    /// <summary>
    /// A user readable message why the entry was not added. <see langword="null"/> if the entry was added successfully or no message was provided.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public string? Message { get; }

    private AddEntryResult(AddEntryState status, DatStringEntry? addedInfo, DatStringEntry? overwrittenInfo, string? message)
    {
        Status = status;
        AddedEntry = addedInfo;
        OverwrittenEntry = overwrittenInfo;
        Message = message;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var sb = new StringBuilder(Status.ToString());
        if (string.IsNullOrEmpty(Message))
        {
            sb.Append($": {Message}");
        }
        return sb.ToString();
    }

    internal static AddEntryResult NotAdded(AddEntryState status, string? reason)
    {
        return new AddEntryResult(status, null, null, reason);
    }

    internal static AddEntryResult NotAddedDuplicate(DatStringEntry duplicateEntry)
    {
        return new AddEntryResult(AddEntryState.NotAddedDuplicate, null, null,
            $"An entry of the same key '{duplicateEntry.Key}' already exists.");
    }

    internal static AddEntryResult EntryAdded(DatStringEntry addedEntry, bool isDuplicate)
    {
        var status = isDuplicate ? AddEntryState.AddedDuplicate : AddEntryState.Added;
        return new AddEntryResult(status, addedEntry, null, null);
    }

    internal static AddEntryResult EntryAdded(DatStringEntry addedEntry, DatStringEntry oldEntry)
    {
        DatStringEntry? overwrittenEntry = IsEntryDefault(in oldEntry) ? null : oldEntry;
        return new AddEntryResult(overwrittenEntry is null ? AddEntryState.Added : AddEntryState.AddedDuplicate,
            addedEntry, overwrittenEntry, null);
    }

    // Using ref here, cause DatStringEntry is 32 byte large and
    // a ref is only IntPtr.Size (which is smaller than 32 byte on 32/64bit systems).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEntryDefault(in DatStringEntry entry)
    {
        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (entry.Crc32 == default && entry.Value is null && entry.Key is null)
            return true;
        return false;
    }
}