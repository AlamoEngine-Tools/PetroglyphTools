using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builders;

/// <summary>
/// A primitive <see cref="IMegBuilder"/> which performs not path normalization or encoding. 
/// </summary>
/// <remarks>
/// Using this instance may produce MEG archives which are not compatible to PG games.
/// </remarks>
public sealed class PrimitiveMegBuilder : MegBuilderBase
{
    /// <remarks>This instance never normalizes entry paths.</remarks>
    /// <inheritdoc/>
    public override bool NormalizesEntryPaths => false;

    /// <remarks>This instance never encodes entry paths.</remarks>
    /// <inheritdoc/>
    public override bool EncodesEntryPaths => false;

    /// <remarks>This instance always overrides duplicate entries.</remarks>
    /// <inheritdoc/>
    public override bool OverwritesDuplicateEntries => true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveMegBuilder"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    public PrimitiveMegBuilder(IServiceProvider services) : base(services)
    {
    }
}