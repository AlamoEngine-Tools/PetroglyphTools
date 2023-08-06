// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

/// <summary>
///     Base service to handle the transformation from binary to a <see cref="IMegFile" /> and vice versa.
/// </summary>
internal interface IMegBinaryServiceFactory
{
    IBinaryFileReader<IMegFileMetadata> GetReader(MegFileVersion megVersion);

    IBinaryFileReader<IMegFileMetadata> GetReader(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);
}