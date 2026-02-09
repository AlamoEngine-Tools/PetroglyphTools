// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal interface IMegSizeCalculator
{
    /// <summary>
    /// Gets the current total size, in bytes, of the MEG file being calculated.
    /// </summary>
    ulong CurrentSize { get; }

    /// <summary>
    /// Gets the size, in bytes, of the metadata associated with the MEG file.
    /// </summary>
    ulong MetadataSize { get; }

    /// <summary>
    /// Pre-calculates the total size of the MEG file metadata and content based on the provided entries
    /// and the current state of the <see cref="IMegSizeCalculator"/>.
    /// </summary>
    /// <param name="entries">
    /// A collection of <see cref="MegDataEntryBuilderInfo"/> objects representing the data entries
    /// to be included in the MEG file.
    /// </param>
    /// <returns>
    /// The total calculated size of the MEG file, including header, filename table, file table, and file data.
    /// </returns>
    public ulong PreCalculateSize(IEnumerable<MegDataEntryBuilderInfo> entries);

    /// <summary>
    /// Pre-calculates the total size of the MEG file metadata and content for the specified dataEntry.
    /// </summary>
    /// <param name="dataEntry">
    /// The <see cref="MegDataEntryBuilderInfo"/> representing the data entry for which the size is to be calculated.
    /// </param>
    /// <returns>
    /// The total size, in bytes, of the MEG file metadata and content, including headers, filename table, file table, and file data.
    /// </returns>
    public ulong PreCalculateSize(MegDataEntryBuilderInfo dataEntry);

    /// <summary>
    /// Adds a new data entry to the MEG file size calculation process.
    /// </summary>
    /// <param name="dataEntry">
    /// The <see cref="MegDataEntryBuilderInfo"/> instance representing the data entry to be added.
    /// This includes metadata such as file path, size, and encryption status.
    /// </param>
    public void AddEntry(MegDataEntryBuilderInfo dataEntry);

    /// <summary>
    /// Resets the internal state of the size calculation process, clearing any accumulated data
    /// and restoring the calculator to its initial state.
    /// </summary>
    void Reset();
}