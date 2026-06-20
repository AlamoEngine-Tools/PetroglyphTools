// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;
using PG.StarWarsGame.Files.Binary.File;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal interface IDatFileReader : IBinaryFileReader<DatBinaryFile>
{
    DatLayoutKind PeekLayout(Stream byteStream);
}