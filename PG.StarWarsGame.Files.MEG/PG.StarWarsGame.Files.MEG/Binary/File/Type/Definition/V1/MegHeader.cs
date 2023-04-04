// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegHeader : IBinaryFile, ISizeable
    {
        internal MegHeader(uint numFileNames, uint numFiles)
        {
            NumFileNames = numFileNames;
            NumFiles = numFiles;
        }

        internal uint NumFileNames { get; }

        internal uint NumFiles { get; }


        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(NumFileNames));
            bytes.AddRange(BitConverter.GetBytes(NumFiles));
            return bytes.ToArray();
        }

        public int Size => sizeof(uint) + sizeof(uint);
    }
}
