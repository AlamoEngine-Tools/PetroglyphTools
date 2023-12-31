using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// A <see cref="IMegBuilder"/> for building MEG files which are safe to be used for a Petroglyph game.
/// Entry paths get normalized, resolved and encoded. This builder only accepts valid entries paths and valid MEG file paths. 
/// <br/>
/// Duplicate entries get overwritten.
/// </summary>
public abstract class PetroglyphGameMegBuilder : MegBuilderBase
{
    /// <summary>
    /// TODO
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
    /// - Resolves path operators ("./" or "../") and rejects entries which traverse out of <see cref="BaseDirectory"/>.
    /// </code>
    /// </remarks>
    /// <inheritdoc/>
    public override IMegDataEntryPathNormalizer? DataEntryPathNormalizer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphGameMegBuilder"/> class.
    /// </summary>
    /// <param name="baseDirectory"></param>
    /// <param name="services">The service provider.</param>
    protected PetroglyphGameMegBuilder(string baseDirectory, IServiceProvider services) : base(services)
    {
        if (baseDirectory == null) 
            throw new ArgumentNullException(nameof(baseDirectory));
        BaseDirectory = FileSystem.Path.GetFullPath(baseDirectory);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The returned path is not normalized by the rules of this instance.</remarks>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public string ResolveEntryPath(string path)
    {
        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(path);

        return path;
    }
}