using System;
using PG.StarWarsGame.Files.MEG.Binary;

namespace PG.StarWarsGame.Files.MEG.Services.Builders;

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
    /// <remarks>
    /// This builder normalizes entry paths by the following rules:
    /// <code>
    /// - Upper case all characters using the invariant culture.
    /// - Replace path separators ('/' or '\') by the current system's default separator, which is '\' on Windows and '/' on Linux/macOS.
    /// </code>
    /// Note: Path operators ("./" or "../") will <b>not</b> get resolved.
    /// </remarks>
    /// <inheritdoc/>
    public override bool NormalizesEntryPaths => true;

    /// <remarks>This builder encodes entry paths using <see cref="MegFileConstants.MegDataEntryPathEncoding"/>.</remarks>
    /// <inheritdoc/>
    public override bool EncodesEntryPaths => true;

    /// <remarks>This builder always overrides duplicate entries.</remarks>
    /// <inheritdoc/>
    public override bool OverwritesDuplicateEntries => true;

    /// <summary>
    /// Initializes a new instance of the <see cref="NormalizingMegBuilder"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    public NormalizingMegBuilder(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc/>
    protected override bool NormalizePath(ref string filePath, out string? message)
    {
        return base.NormalizePath(ref filePath, out message);
    }
}