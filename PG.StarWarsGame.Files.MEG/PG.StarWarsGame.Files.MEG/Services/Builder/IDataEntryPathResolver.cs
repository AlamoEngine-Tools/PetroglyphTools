// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

internal interface IDataEntryPathResolver
{
    string? ResolvePath(string path, string basePath);
}