using System;
using System.Collections.Generic;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
#if NETSTANDARD2_0 || NETFRAMEWORK
using AnakinRaW.CommonUtilities.FileSystem;
#endif

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Base class for an <see cref="IDatBuilder"/> used by the Petroglyph game <em>Star Wars: Empire at War</em> and its extension <em>Empire at War: Forces of Corruption</em>.
/// </summary>
public abstract class PetroglyphStarWarsGameDatBuilder : DatBuilderBase
{
    /// <inheritdoc />
    public sealed override IDatKeyValidator KeyValidator => EmpireAtWarKeyValidator.Instance;

    private protected PetroglyphStarWarsGameDatBuilder(BuilderOverrideKind overrideKind, IServiceProvider services) : base(overrideKind, services)
    {
    }

    /// <inheritdoc />
    protected sealed override bool ValidateFileInformationCore(DatFileInformation fileInformation, IReadOnlyList<DatStringEntry> builderData,
        out string? failedReason)
    { 
        // While for a PG Star Wars game, the name has a hardcoded pattern, we should not validate against the pattern here,
        // in order to allow users creating DAT files for non-game usage, e.g. temporary build artifacts.
        var fileName = FileSystem.Path.GetFileName(fileInformation.FilePath.AsSpan());
        if (!PGFileNameUtilities.IsValidFileName(fileName, out var result))
        {
            failedReason = $"File name is not valid: '{result}'";
            return false;
        }

        return base.ValidateFileInformationCore(fileInformation, builderData, out failedReason);
    }
}