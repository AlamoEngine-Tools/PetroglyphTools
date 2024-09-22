// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Files;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <inheritdoc cref="IDatFile" />
/// <remarks>
///     This class provides direct access to the DAT file content as well as its associated meta-information.
/// </remarks>
public sealed class DatFile(IDatModel model, DatFileInformation fileInfo, IServiceProvider serviceProvider) 
    : PetroglyphFileHolder<IDatModel, DatFileInformation>(model, fileInfo, serviceProvider), IDatFile;