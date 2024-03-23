// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal class MegFileNameTable(IList<MegFileNameTableRecord> items)
    : BinaryTable<MegFileNameTableRecord>(items), IMegFileNameTable;