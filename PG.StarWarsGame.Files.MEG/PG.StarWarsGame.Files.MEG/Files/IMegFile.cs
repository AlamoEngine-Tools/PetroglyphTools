// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
/// Represents a Petroglyph Mega File.
/// </summary>
/// <remarks>
/// Mega files are an archive type bundling files together in a RAM friendly way.
/// </remarks>
public interface IMegFile : IPetroglyphFileHolder<IMegArchive, MegFileInformation>, IMegDataSource;
