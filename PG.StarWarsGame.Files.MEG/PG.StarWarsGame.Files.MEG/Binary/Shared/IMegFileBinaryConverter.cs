// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IMegFileBinaryConverter : IBinaryFileConverter<IMegFileMetadata, IMegFile, MegFileHolderParam>, IDisposable
{
    /// <summary>
    /// Builds a <see cref="IMegFileMetadata" /> from a given list of <see cref="MegFileDataEntryInfo"/>.
    /// </summary>
    /// <remarks>
    /// <paramref name="dataEntries"/> must be sorted already by CRC32.
    /// </remarks>
    /// <param name="dataEntries">The data model to convert</param>
    /// <param name="fileParam"></param>
    /// <returns>The converted binary file.</returns>
    public IMegFileMetadata FromModel(IReadOnlyList<MegFileDataEntryInfo> dataEntries, MegFileHolderParam fileParam);
}