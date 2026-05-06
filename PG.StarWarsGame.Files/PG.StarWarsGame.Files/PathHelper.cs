using AnakinRaW.CommonUtilities.FileSystem.Normalization;

namespace PG.StarWarsGame.Files;

internal static class PathHelper
{
    internal static readonly PathNormalizeOptions UnixMegPathNormalizationOptions = new()
    {
        UnifyDirectorySeparators = true,
        TreatBackslashAsSeparator = true,
        UnifySeparatorKind = DirectorySeparatorKind.System
    };
}