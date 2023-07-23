// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Holder;

public sealed class UnsortedDatFileHolder : ADatFileHolder<List<Tuple<string, string>>, UnsortedDatAlamoFileType>
{
    public UnsortedDatFileHolder(string filePath, string fileName) : base(filePath, fileName)
    {
    }

    internal UnsortedDatFileHolder(string filePath, string fileName, DatFile file) : base(filePath, fileName)
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

    public override UnsortedDatAlamoFileType FileType { get; } = new UnsortedDatAlamoFileType();
    public override List<Tuple<string, string>> Content { get; set; } = new List<Tuple<string, string>>();
}
