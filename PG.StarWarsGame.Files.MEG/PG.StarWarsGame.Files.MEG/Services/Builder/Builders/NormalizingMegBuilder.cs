using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// A <see cref="IMegBuilder"/> which normalizes and encodes entry paths.
/// However, entry path will not be resolved nor validated whether the final encoded path contains illegal file characters (such as '?').
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

    /// <remarks>
    /// This builder normalizes entry paths by the following rules:
    /// <code>
    /// - Upper case all characters using the invariant culture.
    /// - Replace path separators ('/' or '\') by the current system's default separator, which is '\' on Windows and '/' on Linux/macOS.
    /// </code>
    /// Note: Path operators ("./" or "../") will <b>not</b> get resolved.
    /// </remarks>
    /// <inheritdoc/>
    public override IMegDataEntryPathNormalizer? DataEntryPathNormalizer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NormalizingMegBuilder"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public NormalizingMegBuilder(IServiceProvider services) : base(services)
    {
    }
}