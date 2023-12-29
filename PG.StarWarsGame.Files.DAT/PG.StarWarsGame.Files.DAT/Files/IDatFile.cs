// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PG.Commons.Files;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <summary>
///     The provided holder for a Petroglyph <a href="https://modtools.petrolution.net/docs/DatFileFormat">.DAT file</a>.
///     <br />DAT files hold key-value pairs of strings that are used to localize the game.
/// </summary>
public interface IDatFile : IPetroglyphFileHolder<IDatModel, DatFileInformation>;


/// <summary>
/// 
/// </summary>
public interface IDatModel : IReadOnlyList<DatFileEntry>
{
    /// <summary>
    /// 
    /// </summary>
    public DatFileType Order { get; }
}

class DatModel: IDatModel
{
    private readonly IReadOnlyList<DatFileEntry> _entries;

    public int Count => _entries.Count;

    public DatFileEntry this[int index] => _entries[index];

    public DatFileType Order { get; }

    public DatModel(IList<DatFileEntry> entries, DatFileType fileType)
    {
        if (entries == null) 
            throw new ArgumentNullException(nameof(entries));

        Order = fileType;
        _entries = new ReadOnlyCollection<DatFileEntry>(entries.ToList());
    }

    public IEnumerator<DatFileEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}