// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Binary.File.Builder;

internal class UnsortedDatFileBuilder : AbstractDatFileBuilder, IBinaryFileBuilder<DatFile, UnsortedDatFileHolder>
{
    public UnsortedDatFileBuilder() : base()
    {
    }

    public DatFile FromBytes(byte[] byteStream)
    {
        return FromBytesInternal(byteStream);
    }

    public DatFile FromHolder(UnsortedDatFileHolder holder)
    {
        return FromHolderInternal(holder.Content);
    }
}
