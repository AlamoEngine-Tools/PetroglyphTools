// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegFileContentTable : IBinaryFile, ISizeable
    {
        public MegFileContentTable(List<MegFileContentTableRecord> megFileContentTableRecords)
        {
            MegFileContentTableRecords = megFileContentTableRecords ?? new List<MegFileContentTableRecord>();
        }

        [NotNull] internal List<MegFileContentTableRecord> MegFileContentTableRecords { get; }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            foreach (MegFileContentTableRecord megFileContentTableRecord in MegFileContentTableRecords)
            {
                b.AddRange(megFileContentTableRecord.ToBytes());
            }

            return b.ToArray();
        }

        public int Size => MegFileContentTableRecords.Aggregate(0,
            (current, megFileContentTableRecord) => current + megFileContentTableRecord.Size);
    }
}
