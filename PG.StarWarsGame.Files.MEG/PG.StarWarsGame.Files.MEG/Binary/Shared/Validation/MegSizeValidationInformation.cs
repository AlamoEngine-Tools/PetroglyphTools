using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal record MegSizeValidationInformation<T> : IMegSizeValidationInformation<T> where T : IMegFileMetadata
{
    public long BytesRead { get; init; }

    public long ArchiveSize { get; init; }

    public required T Metadata { get; init; }
}