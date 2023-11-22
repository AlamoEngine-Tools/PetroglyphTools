// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// In-memory MEG archive whose contents may be distributed across multiple physical .MEG files.
/// <br/>
/// The PG games usually build such a model internally themselves in order to have a single Master MEG archive.
/// </summary>
/// <remarks>
/// Use the <see cref="IVirtualMegArchiveBuilder"/> service for creating this archive.
/// <br/>
/// <br/>
/// Note: A virtual MEG's data entries cannot reference data entries <em>outside</em> of .MEG files.
/// While this scenario is also supported by PG games this library is not responsible providing functionality for this.
/// <br/>
/// Example:
/// <br/>
/// <example>
/// <code>
/// Load_Order  File                                Contents
///         1   ./Data/Config.meg                   [Unit_X.xml, ...]
///         2   ./Mods/MyMod/Data/Config.meg        [Unit_X.xml, ...]
///         3   ./Mods/MyMod/Data/XML/Unit_X.xml    
/// </code>
/// </example>
/// A PG game will use the file loaded <b>last</b>, which would be Unit_X.xml of the Mods XML directory. 
/// However, a <see cref="IVirtualMegArchive"/> is not designed to reference cases shown in load order 3.
/// </remarks>
public interface IVirtualMegArchive : IMegDataEntryHolder<MegDataEntryReference>;