// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// A <see cref="IDatBuilder"/> for building MasterText DAT files used by the
/// Petroglyph game <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
/// </summary>
public sealed class EmpireAtWarMasterTextFileBuilder : DatBuilderBase
{
    /// <inheritdoc/>
    /// <remarks>An instance of this class always returns <see cref="DatFileType.OrderedByCrc32"/>.</remarks>
    public override DatFileType TargetKeySortOrder => DatFileType.OrderedByCrc32;

    /// <inheritdoc/>
    public override BuilderOverrideKind KeyOverwriteBehavior { get; }

    /// <inheritdoc />
    public override IDatKeyValidator KeyValidator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarCreditsTextFileBuilder"/> class.
    /// </summary>
    /// <param name="overwriteDuplicates"></param>
    /// <param name="services">The service provider.</param>
    public EmpireAtWarMasterTextFileBuilder(bool overwriteDuplicates, IServiceProvider services) : base(services)
    {
        KeyOverwriteBehavior = overwriteDuplicates ? BuilderOverrideKind.Overwrite : BuilderOverrideKind.NoOverwrite;
        KeyValidator = Services.GetRequiredService<EmpireAtWarKeyValidator>();
    }
}