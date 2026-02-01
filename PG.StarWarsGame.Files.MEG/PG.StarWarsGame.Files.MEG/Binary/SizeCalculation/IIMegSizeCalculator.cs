using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.SizeCalculation;

internal interface IIMegSizeCalculator
{
    /// <summary>
    /// Gets the current total size, in bytes, of the MEG file being calculated.
    /// </summary>
    ulong CurrentSize { get; }

    ulong MetadataSize { get; }

    /// <summary>
    /// Pre-calculates the total size of the MEG file metadata and content based on the provided entries
    /// and the current state of the <see cref="IIMegSizeCalculator"/>.
    /// </summary>
    /// <param name="entries">
    /// A collection of <see cref="MegFileDataEntryBuilderInfo"/> objects representing the data entries
    /// to be included in the MEG file.
    /// </param>
    /// <returns>
    /// The total calculated size of the MEG file, including header, filename table, file table, and file data.
    /// </returns>
    public ulong PreCalculateSize(IEnumerable<MegFileDataEntryBuilderInfo> entries);

    /// <summary>
    /// Pre-calculates the total size of the MEG file metadata and content for the specified entry.
    /// </summary>
    /// <param name="entry">
    /// The <see cref="MegFileDataEntryBuilderInfo"/> representing the file entry for which the size is to be calculated.
    /// </param>
    /// <returns>
    /// The total size, in bytes, of the MEG file metadata and content, including headers, filename table, file table, and file data.
    /// </returns>
    public ulong PreCalculateSize(MegFileDataEntryBuilderInfo entry);

    /// <summary>
    /// Adds a new file entry to the MEG file size calculation process.
    /// </summary>
    /// <param name="entry">
    /// The <see cref="MegFileDataEntryBuilderInfo"/> instance representing the file entry to be added.
    /// This includes metadata such as file path, size, and encryption status.
    /// </param>
    public void AddEntry(MegFileDataEntryBuilderInfo entry);

    /// <summary>
    /// Resets the internal state of the size calculation process, clearing any accumulated data
    /// and restoring the calculator to its initial state.
    /// </summary>
    void Reset();
}