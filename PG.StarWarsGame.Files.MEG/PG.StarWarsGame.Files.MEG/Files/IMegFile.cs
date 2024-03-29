// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
///     The provided holder for a Petroglyph
///     <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG file</a>.
///     *.MEG or Mega files are a proprietary archive type bundling files together in a RAM friendly way.
/// </summary>
public interface IMegFile : IPetroglyphFileHolder<IMegArchive, MegFileInformation>
{
    /// <summary>
    /// Gets the archive model of this MEG file.
    /// </summary>
    IMegArchive Archive { get; }
}