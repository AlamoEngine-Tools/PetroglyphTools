using System;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;

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
    public override string Normalize(string filePath)
    {
        return PathNormalizer.Normalize(filePath,
            new PathNormalizeOptions {  UnifyDirectorySeparators = true, UnifyCase = UnifyCasingKind.UpperCaseForce });
    }

    /// <inheritdoc />
    public override int Normalize(ReadOnlySpan<char> filePath, Span<char> destination)
    {
        throw new NotImplementedException();
    }
}