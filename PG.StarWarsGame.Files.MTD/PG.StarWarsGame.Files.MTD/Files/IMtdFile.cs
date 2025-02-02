// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MTD.Data;

namespace PG.StarWarsGame.Files.MTD.Files;

/// <summary>
/// Represents a Petroglyph Mega Texture Directory file.
/// </summary>
public interface IMtdFile : IPetroglyphFileHolder<IMegaTextureDirectory, MtdFileInformation>;