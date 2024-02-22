using System;
using PG.StarWarsGame.Files.MEG.Services.FileSystem;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Normalizes a path in a way that path separators are unified to the backslash separator and upper-cases the path.
/// </summary>
public sealed class PetroglyphDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphDataEntryPathNormalizer"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public PetroglyphDataEntryPathNormalizer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override string NormalizePath(string filePath)
    {
        return FileSystem.Path.Normalize(filePath,
            new PathNormalizeOptions
            {
                UnifySlashes = true, SeparatorKind = DirectorySeparatorKind.Windows,
                UnifyCase = UnifyCasingKind.UpperCaseForce
            });
    }
}