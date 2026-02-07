namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal readonly ref struct MaxMegSizes
{
    public required uint MaxFileSize { get; init; }
    public required uint MaxEntrySize { get; init; }
}