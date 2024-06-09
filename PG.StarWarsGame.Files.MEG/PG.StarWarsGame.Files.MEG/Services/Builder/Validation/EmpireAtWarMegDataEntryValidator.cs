using System;
using AnakinRaW.CommonUtilities.FileSystem;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a MEG data entry whether it is compliant to the Petroglyph game Empire at War. 
/// </summary>
public sealed class EmpireAtWarMegDataEntryValidator : PetroglyphMegDataEntryValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegDataEntryValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EmpireAtWarMegDataEntryValidator(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override bool Validate(MegFileDataEntryBuilderInfo? builderInfo)
    {
        if (!base.Validate(builderInfo))
            return false;

        if (builderInfo!.Encrypted)
            return false;

        var pathSpan = builderInfo.FilePath.AsSpan();

        if (pathSpan.IndexOf('/') != -1)
            return false;

        Span<char> upperBuffer = stackalloc char[260];
        var length = pathSpan.ToUpperInvariant(upperBuffer);
        var upper = upperBuffer.Slice(0, length);

        if (upper.Length != pathSpan.Length || !pathSpan.Equals(upper, StringComparison.Ordinal))
            return false;

        try
        {
            var fileName = FileSystem.Path.GetFileName(pathSpan);
            return FileNameUtilities.IsValidFileName(fileName, out _);
        }
        catch (Exception)
        {
            return false;
        }
    }
}