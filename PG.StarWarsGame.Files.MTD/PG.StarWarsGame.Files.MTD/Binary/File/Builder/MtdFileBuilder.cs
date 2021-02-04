// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary.File.Builder;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.MTD.Holder;

namespace PG.StarWarsGame.Files.MTD.Binary.File.Builder
{
    internal class MtdFileBuilder : IBinaryFileBuilder<MtdFile,MtdFileHolder>
    {
        public MtdFile FromBytes(byte[] byteStream)
        {
            throw new System.NotImplementedException();
        }

        public MtdFile FromHolder(MtdFileHolder holder)
        {
            throw new System.NotImplementedException();
        }
    }
}
