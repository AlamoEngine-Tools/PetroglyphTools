// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Represents an <see cref="IMegBuilder"/> for building MEG files compatible with Petroglyph's
/// game <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
/// </summary>
public sealed class EmpireAtWarMegBuilder : PetroglyphGameMegBuilder
{
    /// <inheritdoc />
    public override IMegDataEntryPathNormalizer DataEntryPathNormalizer { get; } = new EmpireAtWarMegDataEntryPathNormalizer();
    
    /// <summary>
    /// Gets the data entry validator to validate MEG data entries to be compliant to Empire at War
    /// </summary>
    public override IMegDataEntryValidator DataEntryValidator { get; } = new EmpireAtWarMegDataEntryValidator();

    /// <summary>
    /// Gets the validator to validate whether an <seealso cref="MegFileInformation"/> is compliant to Empire at War
    /// </summary>
    public override IMegFileInformationValidator MegFileInformationValidator { get; }

    /// <value>
    /// 2GB (2^31 - 1 bytes), which is the safe limit for Petroglyph's games
    /// <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
    /// </value>
    /// <inheritdoc />
    public override uint MaxMegFileSize { get; } = MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.EawFoc).MaxFileSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegBuilder"/> class with a specified game path.
    /// </summary>
    /// <remarks>
    /// <paramref name="baseDirectory"/> usually is a game's or mod's ./DATA/ directory, however it can be set to any other directory.
    /// </remarks>
    /// <param name="baseDirectory">The path for this <see cref="EmpireAtWarMegBuilder"/>.</param>
    /// <param name="services">The service provider.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> or <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is empty.</exception>
    public EmpireAtWarMegBuilder(string baseDirectory, IServiceProvider services) : base(baseDirectory, services)
    {
        MegFileInformationValidator = new EmpireAtWarMegFileInformationValidator(services);
    }
}