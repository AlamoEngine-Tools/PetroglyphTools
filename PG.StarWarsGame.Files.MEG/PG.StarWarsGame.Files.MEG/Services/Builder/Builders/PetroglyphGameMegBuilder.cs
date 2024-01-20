using System;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// A <see cref="IMegBuilder"/> for building MEG files which are safe to be used for a Petroglyph game.
/// Entry paths get normalized, resolved and encoded. This builder only accepts valid entries paths and valid MEG file paths. 
/// <br/>
/// Duplicate entries get overwritten.
/// </summary>
public abstract class PetroglyphGameMegBuilder : MegBuilderBase
{
    private readonly IDataEntryPathResolver _pathResolver;

    /// <summary>
    /// Gets the base directory used for creating relative paths.
    /// </summary>
    public string BaseDirectory { get; }

    /// <remarks>This builder always overrides duplicate entries.</remarks>
    /// <inheritdoc/>
    public override bool OverwritesDuplicateEntries => true;

    /// <remarks>This builder automatically determines file sizes for local file-based entries.</remarks>
    /// <inheritdoc/>
    public override bool AutomaticallyAddFileSizes => true;

    /// <remarks>
    /// This builder normalizes entry paths by the following rules:
    /// <code>
    /// - Upper cases all characters using the invariant culture.
    /// - Replaces path separators ('/' or '\') by the current system's default separator, which is '\' on Windows and '/' on Linux/macOS.
    /// - Path operators ("." or "..") get rejected.
    /// </code>
    /// </remarks>
    /// <inheritdoc/>
    public override IMegDataEntryPathNormalizer DataEntryPathNormalizer => PetroglyphDataEntryPathNormalizer.Instance;

    /// <summary>
    /// 
    /// </summary>
    public override IMegFileInformationValidator MegFileInformationValidator => PetroglyphMegFileInformationValidator.Instance;

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
        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(baseDirectory);
        BaseDirectory = FileSystem.DirectoryInfo.New(baseDirectory).FullName;
        _pathResolver = services.GetRequiredService<IDataEntryPathResolver>();
    }

    /// <summary>
    /// Returns a relative path from a path and the <see cref="BaseDirectory"/> of this instance.
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
    /// <remarks>The returned path is neither normalized nor validated by the rules of this instance.</remarks>
    /// <param name="path">A path to get the relative path from.</param>
    /// <returns>The relative path.</returns>
    public string? ResolveEntryPath(string path)
    {
        try
        {
            return _pathResolver.ResolvePath(path, BaseDirectory);
        }
        catch
        {
            return null;
        }
    }
}