// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IMegArchive"/>
[DebuggerDisplay("{Count} entries")]
internal sealed class MegArchive : MegDataEntryHolderBase<MegDataEntry>, IMegArchive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegArchive"/> class by copying all elements of the specified list.
    /// </summary>
    /// <param name="entries">The list of entries in this archive.</param>
    internal MegArchive(IList<MegDataEntry> entries) : base(entries)
    {
    }
}