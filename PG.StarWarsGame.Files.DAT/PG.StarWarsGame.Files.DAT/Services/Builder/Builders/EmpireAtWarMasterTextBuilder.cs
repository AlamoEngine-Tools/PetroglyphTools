// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Represents an <see cref="IDatBuilder"/> for building MasterText DAT files used by the
/// Petroglyph game <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
/// </summary>
public sealed class EmpireAtWarMasterTextBuilder : PetroglyphStarWarsGameDatBuilder
{
    /// <inheritdoc/>
    /// <remarks>An instance of this class always returns <see cref="DatLayoutKind.OrderedByCrc32"/>.</remarks>
    public override DatLayoutKind TargetLayout => DatLayoutKind.OrderedByCrc32;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMasterTextBuilder"/> class.
    /// </summary>
    /// <param name="overwriteDuplicates">
    /// <see langword="true"/> to overwrite an existing entry when a duplicate key is added; otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="services">The service provider.</param>
    public EmpireAtWarMasterTextBuilder(bool overwriteDuplicates, IServiceProvider services) 
        : base(overwriteDuplicates ? BuilderOverrideKind.Overwrite : BuilderOverrideKind.NoOverwrite, services)
    {
    }
}