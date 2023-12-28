using System;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

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

    internal static AddDataEntryToBuilderResult FromFileNotFound(string filePath)
    {
        return EntryNotAdded(AddDataEntryToBuilderState.FileOrEntryNotFound, $"Source file '{filePath}' does not exist.");
    }

    internal static AddDataEntryToBuilderResult FromEntryNotFound(MegDataEntryLocationReference entryReference)
    {
        return EntryNotAdded(AddDataEntryToBuilderState.FileOrEntryNotFound, $"Source entry '{entryReference}' does not exist.");
    }

    internal static AddDataEntryToBuilderResult FromDuplicate(string filePath)
    {
        return EntryNotAdded(AddDataEntryToBuilderState.DuplicateEntry, $"A data entry of the path '{filePath}' already exists.");
    }
}