using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

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

public interface IMegDataEntry<T> : IMegDataEntry where T : notnull
{
    public T Location { get; }
}