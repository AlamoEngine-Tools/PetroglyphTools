// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <summary>
///     The provided holder for a Petroglyph <a href="https://modtools.petrolution.net/docs/DatFileFormat">.DAT file</a>.
///     <br />DAT files hold key-value pairs of strings that are used to localize the game.
/// </summary>
public interface IDatFile : IPetroglyphFileHolder<IDatModel, DatFileInformation>;