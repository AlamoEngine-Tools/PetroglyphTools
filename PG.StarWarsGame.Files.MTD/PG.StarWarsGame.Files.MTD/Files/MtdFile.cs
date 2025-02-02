// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MTD.Data;

namespace PG.StarWarsGame.Files.MTD.Files;

/// <inheritdoc cref="IMtdFile"/>
public sealed class MtdFile(IMegaTextureDirectory model, MtdFileInformation fileInfo, IServiceProvider serviceProvider)
    : PetroglyphFileHolder<IMegaTextureDirectory, MtdFileInformation>(model, fileInfo, serviceProvider), IMtdFile;