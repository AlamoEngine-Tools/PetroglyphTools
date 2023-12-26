// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Binary;

internal interface IDatBinaryServiceFactory
{
    IDatFileReader GetReader();
    IDatBinaryConverter GetConverter();
    IDatFileWriter GetWriter();
}