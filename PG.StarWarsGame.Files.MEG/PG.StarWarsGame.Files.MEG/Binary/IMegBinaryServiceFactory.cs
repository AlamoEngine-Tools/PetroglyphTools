// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

/// <summary>
///     Base service to handle the transformation from binary to a <see cref="IMegFile" /> and vice versa.
/// </summary>
internal interface IMegBinaryServiceFactory
{
    IMegFileBinaryReader GetReader(MegFileVersion megVersion);

    IMegFileBinaryReader GetReader(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv);

    IMegBinaryConverter GetConverter(MegFileVersion megVersion);

    IConstructingMegArchiveBuilder GetConstructionBuilder(MegFileVersion megVersion);
}