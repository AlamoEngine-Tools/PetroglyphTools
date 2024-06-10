using System;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that checks the passed <see cref="MegFileDataEntryBuilderInfo"/> is not <see langword="null"/>.
/// </summary>
public sealed class NotNullDataEntryValidator : IBuilderInfoValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NotNullDataEntryValidator"/> class.
    /// </summary>
    public static readonly NotNullDataEntryValidator Instance = new();

    private NotNullDataEntryValidator()
    {
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builderInfo"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Validate(MegFileDataEntryBuilderInfo? builderInfo)
    {
        if (builderInfo is null)
            return false;
        return true;
    }

    /// <inheritdoc />
    public bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size)
    {
        return true;
    }
}