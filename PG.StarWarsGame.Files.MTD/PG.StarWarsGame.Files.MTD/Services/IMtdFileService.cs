// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;
using PG.StarWarsGame.Files.MTD.Files;

namespace PG.StarWarsGame.Files.MTD.Services;

public interface IMtdFileService
{
    IMtdFile Load(string filePath);

    IMtdFile Load(Stream stream);
}