// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IMegArchive"/>
internal sealed class MegArchive : MegDataEntryHolderBase<MegFileDataEntry>, IMegArchive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegArchive"/> class
    /// by coping all elements of the given <paramref name="entries"/> list.
    /// </summary>
    /// <param name="entries">The list of entries in this archive.</param>
    internal MegArchive(IList<MegFileDataEntry> entries) : base(entries)
    {
    }
}