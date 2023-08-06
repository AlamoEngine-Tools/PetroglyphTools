// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal interface IIndexTable : IBinary, IEnumerable<IDatRecordDescriptor>
{
    IDatRecordDescriptor this[int i] { get; }
}