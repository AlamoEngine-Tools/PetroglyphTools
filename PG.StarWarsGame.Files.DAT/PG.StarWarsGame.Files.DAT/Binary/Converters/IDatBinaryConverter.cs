// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal interface IDatBinaryConverter : IBinaryConverter<DatBinaryFile, IDatModel>;