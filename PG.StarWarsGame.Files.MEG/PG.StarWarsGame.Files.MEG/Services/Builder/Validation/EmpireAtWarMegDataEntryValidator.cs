using System;
using AnakinRaW.CommonUtilities.FileSystem;
using PG.Commons.Utilities;

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
    public override bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size)
    {
        if (!base.Validate(entryPath, encrypted, size))
            return false;

        if (encrypted)
            return false;

        if (entryPath.IndexOf('/') != -1)
            return false;

        Span<char> upperBuffer = stackalloc char[260];
        var length = entryPath.ToUpperInvariant(upperBuffer);
        var upper = upperBuffer.Slice(0, length);

        if (upper.Length != entryPath.Length || !entryPath.Equals(upper, StringComparison.Ordinal))
            return false;

        try
        {
            var fileName = FileSystem.Path.GetFileName(entryPath);
            return FileNameUtilities.IsValidFileName(fileName, out _);
        }
        catch (Exception)
        {
            return false;
        }
    }
}