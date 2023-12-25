// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Files;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <inheritdoc cref="IDatFile" />
/// <remarks>
///     This class provides direct access to the DAT file content as well as its associated meta-information.
/// </remarks>
public sealed class DatFileHolder : PetroglyphFileHolder<DatFileHolderParam, IReadOnlyList<DatFileEntry>, DatAlamoFileType>,
    IDatFile
{
    /// <inheritdoc />
    public DatFileType Order { get; }

    /// <inheritdoc />
    public DatFileHolder(IReadOnlyList<DatFileEntry> model, DatFileHolderParam param, IServiceProvider serviceProvider)
        : base(model, param, serviceProvider)
    {
        Order = param.Order;
    }
}