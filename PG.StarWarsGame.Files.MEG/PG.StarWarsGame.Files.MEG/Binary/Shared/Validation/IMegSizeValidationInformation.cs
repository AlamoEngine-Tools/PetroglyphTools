using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

interface IMegSizeValidationInformation
{
    public long BytesRead { get; }

    public long ArchiveSize { get; }

    public IMegFileMetadata Metadata { get; }
}