// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IMegBinaryConverter : IBinaryConverter<IMegFileMetadata, IMegArchive>, IDisposable;