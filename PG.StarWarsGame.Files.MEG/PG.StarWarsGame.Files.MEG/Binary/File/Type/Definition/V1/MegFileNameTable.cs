// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEGs.Test")]

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegFileNameTable : IBinaryFile, ISizeable
    {
        public MegFileNameTable(List<MegFileNameTableRecord> megFileNameTableRecords)
        {
            MegFileNameTableRecords = megFileNameTableRecords ?? new List<MegFileNameTableRecord>();
        }

        [NotNull] internal List<MegFileNameTableRecord> MegFileNameTableRecords { get; }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            foreach (MegFileNameTableRecord megFileNameTableRecord in MegFileNameTableRecords)
            {
                b.AddRange(megFileNameTableRecord.ToBytes());
            }

            return b.ToArray();
        }

        public int Size => ToBytes().Length;
    }
}
