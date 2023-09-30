using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal record MegSizeValidationInformation : IMegSizeValidationInformation
{
    public long BytesRead { get; init; }

    public long ArchiveSize { get; init; }

    public required IMegFileMetadata Metadata { get; init; }
}