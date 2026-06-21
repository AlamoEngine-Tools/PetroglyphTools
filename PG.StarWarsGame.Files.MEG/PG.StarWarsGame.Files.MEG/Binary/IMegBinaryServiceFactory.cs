// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

/// <summary>
/// Provides factory methods for the services that transform between binary data and an <see cref="IMegFile" />.
/// </summary>
internal interface IMegBinaryServiceFactory
{
    IMegFileBinaryReader GetReader(MegVersion megVersion);

    IMegBinaryConverter GetConverter(MegVersion megVersion);

    IConstructingMegArchiveBuilder GetConstructionBuilder(MegVersion megVersion);
    
    IMegSizeCalculator GetMegSizeCalculator(MegVersion megVersion);
}