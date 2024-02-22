using System;
using PG.StarWarsGame.Files.MEG.Services.FileSystem;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Normalizes a path in a way that path separators are unified to the current system's default separator and upper-cases the path.
/// </summary>
public sealed class DefaultDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDataEntryPathNormalizer"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public DefaultDataEntryPathNormalizer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override string NormalizePath(string filePath)
    {
        return FileSystem.Path.Normalize(filePath,
            new PathNormalizeOptions { UnifySlashes = true, UnifyCase = UnifyCasingKind.UpperCaseForce });
    }
}