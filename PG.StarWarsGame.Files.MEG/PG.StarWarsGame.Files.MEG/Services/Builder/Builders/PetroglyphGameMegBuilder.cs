// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using AnakinRaW.CommonUtilities;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Represents an <see cref="IMegBuilder"/> for building MEG files compatible a Petroglyph game.
/// </summary>
/// <remarks>
/// <para>
/// Entry paths get normalized. The builder only accepts valid entries paths and valid MEG file paths. 
/// </para>
/// <para>
/// Duplicate entries get overwritten.
/// </para>
/// </remarks>
public abstract class PetroglyphGameMegBuilder : MegBuilderBase
{
    private readonly IDataEntryPathResolver _pathResolver;

    /// <summary>
    /// Gets the base directory used for creating relative paths.
    /// </summary>
    public string BaseDirectory { get; }

    /// <remarks>This builder always overrides duplicate entries.</remarks>
    /// <inheritdoc/>
    public sealed override bool OverwritesDuplicateEntries => true;

    /// <summary>
    /// Gets the data entry path normalizer.
    /// </summary>
    /// <remarks>
    /// This builder normalizes entry paths by the following rules:
    /// <code>
    /// - Upper cases all characters using the invariant culture.
    /// - Replaces path separators ('/' or '\') to the Windows path separator, which is '\'.
    /// </code>
    /// <br/>
    /// Note: Path operators ("./" or "../") will <b>not</b> get resolved.
    /// <br/>
    /// Note: As the normalized path will always use the backslash as path operator ('\'),
    /// on Linux systems the path cannot be treated correctly anymore.
    /// </remarks>
    /// <inheritdoc />
    public override IMegDataEntryPathNormalizer DataEntryPathNormalizer { get; } =
        new PetroglyphMegDataEntryPathNormalizer();

    /// <inheritdoc/>
    public abstract override IMegDataEntryValidator DataEntryValidator { get; }

    /// <inheritdoc/>
    public abstract override IMegFileInformationValidator MegFileInformationValidator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphGameMegBuilder"/> class with a specified game path.
    /// </summary>
    /// <remarks>
    /// <paramref name="baseDirectory"/> usually is a game's or mod's ./DATA/ directory, however it can be set to any other directory.
    /// </remarks>
    /// <param name="baseDirectory">The path for this <see cref="PetroglyphGameMegBuilder"/>.</param>
    /// <param name="services">The service provider.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> or <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is empty.</exception>
    protected PetroglyphGameMegBuilder(string baseDirectory, IServiceProvider services) : base(services)
    {
        ThrowHelper.ThrowIfNullOrEmpty(baseDirectory);
        baseDirectory = PathNormalizer.Normalize(FileSystem.Path.GetFullPath(baseDirectory), PathNormalizeOptions.EnsureTrailingSeparator);
        BaseDirectory = baseDirectory;
        _pathResolver = services.GetRequiredService<IDataEntryPathResolver>();
    }

    /// <summary>
    /// Returns a relative path from a path and the <see cref="BaseDirectory"/> of the <see cref="PetroglyphGameMegBuilder"/>.
    /// Returns <see langword="null"/> if <paramref name="path"/> is invalid or not a part of <see cref="BaseDirectory"/>.
    /// <br/>
    /// <br/>
    /// For example:
    /// <br/>
    /// <code>"file.txt" --> "file.txt"</code>
    /// <code>"xml/file.txt" --> "xml/file.txt"</code>
    /// <code>"/gameBasePath/xml/file.xml" --> "xml/file.xml"</code>
    /// <code>"/NOTgamePath/xml/file.xml" --> null</code>
    /// <code>"../xml/file.xml" --> null</code>
    /// </summary>
    /// <remarks>The returned path is neither fully normalized nor validated by the rules of the <see cref="PetroglyphGameMegBuilder"/>.</remarks>
    /// <param name="path">The path to get the relative path from.</param>
    /// <returns>The resolved, relative entry path.</returns>
    public string? ResolveEntryPath(string? path)
    {
        return _pathResolver.ResolvePath(path, BaseDirectory);
    }
}