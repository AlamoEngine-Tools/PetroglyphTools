using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IVirtualMegArchive"/>
internal sealed class VirtualMegArchive : MegDataEntryHolderBase<MegFileDataEntryReference>, IVirtualMegArchive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegArchive"/> class
    /// by coping all elements of the given <paramref name="files"/> list.
    /// </summary>
    /// <param name="files">The list of files in this archive.</param>
    internal VirtualMegArchive(IList<MegFileDataEntryReference> files) : base(files)
    {
    }
}