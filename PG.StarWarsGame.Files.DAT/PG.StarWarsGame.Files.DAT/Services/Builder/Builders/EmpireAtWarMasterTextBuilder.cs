﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// A <see cref="IDatBuilder"/> for building MasterText DAT files used by the
/// Petroglyph game <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
/// </summary>
public sealed class EmpireAtWarMasterTextBuilder : PetroglyphStarWarsGameDatBuilder
{
    /// <inheritdoc/>
    /// <remarks>An instance of this class always returns <see cref="DatFileType.OrderedByCrc32"/>.</remarks>
    public override DatFileType TargetKeySortOrder => DatFileType.OrderedByCrc32;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarCreditsTextBuilder"/> class.
    /// </summary>
    /// <param name="overwriteDuplicates"></param>
    /// <param name="services">The service provider.</param>
    public EmpireAtWarMasterTextBuilder(bool overwriteDuplicates, IServiceProvider services) 
        : base(overwriteDuplicates ? BuilderOverrideKind.Overwrite : BuilderOverrideKind.NoOverwrite, services)
    {
    }
}