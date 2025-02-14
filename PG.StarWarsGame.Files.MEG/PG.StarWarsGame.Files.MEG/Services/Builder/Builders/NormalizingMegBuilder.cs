// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// A <see cref="IMegBuilder"/> which normalizes and encodes entry paths.
/// However, entry paths will not be resolved nor validated whether their final encoded path contains illegal file characters.
/// <br/>
/// Duplicate entries get overwritten.
/// </summary>
/// <remarks>
/// Using this instance may produce MEG archives which are not compatible to PG games.
/// </remarks>
public sealed class NormalizingMegBuilder : MegBuilderBase
{
    /// <remarks>This builder always overrides duplicate entries.</remarks>
    /// <inheritdoc/>
    public override bool OverwritesDuplicateEntries => true;

    /// <summary>
    /// Gets the data entry path normalizer.
    /// </summary>
    /// <remarks>
    /// This builder normalizes entry paths by the following rules:
    /// <code>
    /// - Upper case all characters using the invariant culture (even on Linux systems).
    /// - Replace path separators ('/' or '\') to the current system's default separator, which is '\' on Windows and '/' on Linux/macOS.
    /// </code>
    /// <br/>
    /// Note: Path operators ("./" or "../") will <b>not</b> get resolved.
    /// </remarks>
    public override IMegDataEntryPathNormalizer DataEntryPathNormalizer => DefaultDataEntryPathNormalizer.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="NormalizingMegBuilder"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public NormalizingMegBuilder(IServiceProvider services) : base(services)
    {
    }
}