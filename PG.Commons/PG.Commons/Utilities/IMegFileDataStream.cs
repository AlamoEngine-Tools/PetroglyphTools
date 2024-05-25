namespace PG.Commons.Utilities;

/// <summary>
/// Helper interface to identify MEG data stream without directly referencing the MEG package.
/// </summary>
public interface IMegFileDataStream
{
    /// <summary>
    /// Gets the path of the entry used the MEG archive.
    /// </summary>
    string EntryPath { get; }
}