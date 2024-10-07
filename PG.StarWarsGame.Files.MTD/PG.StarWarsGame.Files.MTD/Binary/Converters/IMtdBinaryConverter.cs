// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using PG.StarWarsGame.Files.MTD.Data;

namespace PG.StarWarsGame.Files.MTD.Binary;

internal interface IMtdBinaryConverter : IBinaryConverter<MtdBinaryFile, IMegaTextureDirectory>;