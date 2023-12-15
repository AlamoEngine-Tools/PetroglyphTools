using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// Service to build virtual MEG archives.
/// </summary>
public interface IVirtualMegArchiveBuilder
{
    /// <summary>
    /// Builds a virtual MEG archive from a collection of MEG data references. 
    /// </summary>
    /// <remarks>
    /// The resulting archive is correctly sorted as specified.
    /// </remarks>
    /// <param name="fileEntries">The collection of data references.</param>
    /// <param name="replaceExisting">When <see langowrd="true"/>, entries with the same CRC32 checksum get replaced by the entry last entry.</param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting);

    /// <summary>
    /// Converts an <see cref="IMegFile"/> into a virtual, in-memory archive.
    /// </summary>
    /// <remarks>
    /// The resulting archive is sorted identical to the given <paramref name="megFile"/>.
    /// </remarks>
    /// <param name="megFile">The MEG file to convert.</param>
    /// <param name="replaceExisting">When <see langowrd="true"/>, entries with the same CRC32 checksum get replaced by the entry last entry.</param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive BuildFrom(IMegFile megFile, bool replaceExisting);

    /// <summary>
    /// Merges a list of physical MEG files into a virtual, in-memory archive.
    /// </summary>
    /// <param name="megFiles">The physical MEG files to merge into a virtual MEG archive.</param>
    /// <param name="replaceExisting">When <see langowrd="true"/>, entries with the same CRC32 checksum get replaced by the entry last entry.</param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive BuildFrom(IList<IMegFile> megFiles, bool replaceExisting);
}