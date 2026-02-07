// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Represents an <see cref="IMegBuilder"/> for building MEG files compliant to the binary MEG file format,
/// but without any entry path normalization or validation or than ensuring binary compliance.
/// </summary>
/// <remarks>
/// <para>
/// Using this instance may produce MEG archives which are not compatible to PG games.
/// </para>
/// <para>
/// Duplicate entries get overwritten.
/// </para>
/// </remarks>
public sealed class BinaryMegBuilder : MegBuilderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryMegBuilder"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public BinaryMegBuilder(IServiceProvider services) : base(services)
    {
    }
}