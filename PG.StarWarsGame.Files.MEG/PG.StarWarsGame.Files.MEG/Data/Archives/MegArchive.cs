// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IMegArchive"/>
internal sealed class MegArchive : MegDataEntryHolderBase<MegDataEntry>, IMegArchive
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegArchive"/> class
    /// by coping all elements of the given <paramref name="files"/> list.
    /// </summary>
    /// <param name="files">The list of files in this archive.</param>
    internal MegArchive(IList<MegDataEntry> files) : base(files)
    {
    }
}