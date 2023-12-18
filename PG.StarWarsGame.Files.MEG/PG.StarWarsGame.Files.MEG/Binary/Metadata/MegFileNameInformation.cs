using System;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal readonly struct MegFileNameInformation(string fileName, string originalFileName)
{
    public string FileName { get; } = fileName ?? throw new ArgumentNullException(nameof(fileName));

    public string OriginalFileName { get; } = originalFileName ?? throw new ArgumentNullException(nameof(originalFileName));
}