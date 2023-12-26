// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Files;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <inheritdoc cref="IDatFile" />
/// <remarks>
///     This class provides direct access to the DAT file content as well as its associated meta-information.
/// </remarks>
public sealed class DatFileHolder : PetroglyphFileHolder<IDatModel, DatFileInformation>, IDatFile
{
    /// <inheritdoc />
    public DatFileHolder(IDatModel model, DatFileInformation fileInfo, IServiceProvider serviceProvider) : base(model, fileInfo, serviceProvider)
    {
    }
}