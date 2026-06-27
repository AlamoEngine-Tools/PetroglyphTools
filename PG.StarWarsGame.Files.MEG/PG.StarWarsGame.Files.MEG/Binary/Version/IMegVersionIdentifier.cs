// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;
using System.IO;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IMegVersionIdentifier
{
    MegVersion GetMegVersion(Stream data, out bool encrypted);
}