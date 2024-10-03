// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Base class for alamo engine MEG data entry path normalizers.
/// </summary>
public abstract class PetroglyphDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphDataEntryPathNormalizer"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected PetroglyphDataEntryPathNormalizer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}