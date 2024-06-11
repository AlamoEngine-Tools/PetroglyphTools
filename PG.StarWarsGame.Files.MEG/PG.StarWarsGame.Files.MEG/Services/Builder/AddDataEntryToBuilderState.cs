// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

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
    FailedNormalization,
    /// <summary>
    /// The file entry was not added because the source file is larger than 4GB.
    /// </summary>
    EntryFileTooLarge
}