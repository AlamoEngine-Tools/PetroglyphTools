// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Holder;

public sealed class SortedDatFileHolder : ADatFileHolder<List<Tuple<string, string>>, SortedDatAlamoFileType>
{
    private readonly List<Tuple<string, string>> m_content = new List<Tuple<string, string>>();

    public SortedDatFileHolder(string filePath, string fileName) : base(filePath, fileName)
    {
    }

    internal SortedDatFileHolder(string filePath, string fileName, DatFile file) : base(filePath, fileName)
    {
        Debug.Assert(file != null, nameof(file) + " != null");
        Debug.Assert(file.Keys != null, "file.Keys != null");
        for (var i = 0; i < file.Keys.Count; i++)
        {
            Debug.Assert(file.Keys[i] != null, $"file.Keys[{i}] != null");
            if (!StringUtility.HasText(file.Keys[i].Key))
            {
                continue;
            }

            Debug.Assert(file.Values != null, "file.Values != null");
            Debug.Assert(file.Values[i] != null, $"file.Values[{i}] != null");
            Content.Add(new Tuple<string, string>(file.Keys[i].Key, file.Values[i].Value ?? string.Empty));
        }
    }

    public override SortedDatAlamoFileType FileType { get; } = new SortedDatAlamoFileType();

    public override List<Tuple<string, string>> Content
    {
        get
        {
            m_content.Sort((t1, t2) => ChecksumService.GetChecksum(t1?.Item1 ?? string.Empty)
                .CompareTo(ChecksumService.GetChecksum(t2?.Item1 ?? string.Empty)));
            return m_content;
        }
        set
        {
            m_content.Clear();
            m_content.AddRange(value);
        }
    }

    public static SortedDatFileHolder FromDictionary(SortedDatFileHolder holder, Dictionary<string, string> dictionary)
    {
        holder.Content.Clear();
        foreach (var entry in dictionary)
        {
            holder.Content.Add(new Tuple<string, string>(entry.Key, entry.Value));
        }

        return holder;
    }

    public Dictionary<string, string> ToDictionary()
    {
        var d = new Dictionary<string, string>();

        foreach ((var item1, var item2) in Content)
        {
#if NETSTANDARD2_1_OR_GREATER
            d.TryAdd(item1, item2);
#else
                if (d.ContainsKey(item2))
                    continue;
                d.Add(item1, item2);
#endif
        }
        return d;
    }
}
