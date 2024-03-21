// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <inheritdoc cref="PetroglyphFileInformation" />
public sealed record DatFileInformation : PetroglyphFileInformation
{
    /// <inheritdoc cref="IDatModel.Order" />
    public DatFileType Order { get; init; }
}