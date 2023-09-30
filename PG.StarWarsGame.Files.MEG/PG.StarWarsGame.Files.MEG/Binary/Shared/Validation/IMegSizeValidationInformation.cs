using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

interface IMegSizeValidationInformation<out T> where T : IMegFileMetadata
{
    public long BytesRead { get; }

    public long ArchiveSize { get; }

    public T Metadata { get; }
}