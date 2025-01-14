// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// A primitive <see cref="IMegBuilder"/> which performs no entry path normalization.
/// <br/>
/// Duplicate entries get overwritten.
/// </summary>
/// <remarks>
/// Using this instance may produce MEG archives which are not compatible to PG games.
/// </remarks>
public sealed class PrimitiveMegBuilder : MegBuilderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveMegBuilder"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public PrimitiveMegBuilder(IServiceProvider services) : base(services)
    {
    }
}