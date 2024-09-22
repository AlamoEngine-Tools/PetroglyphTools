// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Data;

internal interface IUnsortedDatModel : IDatModel
{
    ISortedDatModel ToSortedModel();
}