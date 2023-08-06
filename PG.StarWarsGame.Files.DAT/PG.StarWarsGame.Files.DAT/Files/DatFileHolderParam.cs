// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <inheritdoc cref="FileHolderParamBase" />
public sealed record DatFileHolderParam : FileHolderParamBase
{
    /// <inheritdoc cref="DatFileHolder.Order" />
    public DatFileType? Order { get; set; }
}