// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

internal class MegFileTable(IList<MegFileTableRecord> megFileContentTableRecords) : MegFileTableBase<MegFileTableRecord>(megFileContentTableRecords);