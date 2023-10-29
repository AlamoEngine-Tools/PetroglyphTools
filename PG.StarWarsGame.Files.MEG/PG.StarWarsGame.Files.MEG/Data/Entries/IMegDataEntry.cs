using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

/// <summary>
/// Represents an archived file within a MEG archive.
/// </summary>
/// <remarks>
/// Note: Comparison is based on the CRC32 checksum only. Equality checks may include more properties.
/// </remarks>
public interface IMegDataEntry : IComparable<IMegDataEntry>
{
    /// <summary>
    /// Gets the relative file path as defined in the *.MEG file.<br />
    /// Usually this file path is relative to the game or mod's DATA directory, e.g. Data/My/file.xml
    /// </summary>
    /// <remarks>
    /// Path operators such as ./ or ../ are permitted. It's the application's responsibility to resolve paths and
    /// deal with potential dangerous file paths.
    /// </remarks>
    public string FilePath { get; }

    /// <summary>
    /// Gets the <see cref="Crc32"/> of the file name.
    /// </summary>
    public Crc32 FileNameCrc32 { get; }
}

/// <inheritdoc/>
/// <typeparam name="T">The type of the entry's location information.</typeparam>
public interface IMegDataEntry<out T> : IMegDataEntry where T : notnull
{
    /// <summary>
    /// Get the location information of this data entry.
    /// </summary>
    public T Location { get; }
}