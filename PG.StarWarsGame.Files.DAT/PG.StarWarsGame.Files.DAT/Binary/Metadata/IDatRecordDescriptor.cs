// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal interface IDatRecordDescriptor : IBinary, IComparable<IDatRecordDescriptor>
{
    public Crc32 Crc32 { get; }
    public uint KeyLength { get; }
    public uint ValueLength { get; }
}